Imports WebProj.FranchiseeDataExporter
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Namespace MobizApplication
    Public Class CincoCorporationController
        Inherits System.Web.Mvc.Controller

        Shared systemwide As String
        Shared systemwideDatetime As Date
        Shared ownershiptype As String
        Shared ownershiptypeDatetime As Date
        Shared franchisereport
        Shared franchisereportDate As Date
        Shared franchisereportViewDate As Date
        Shared storeutilstatus As DataTable
        Shared fewesttrans As DataTable
        Shared monthlyutil

        Function Index() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function GetSalesReport() As String
            Dim cinco As New CincoCorporation
            Dim pDTSales As DataTable

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            'cinco.DateFrom = "2020-01-01"
            'cinco.DateTo = "2021-12-31"
            'cinco.FOWSTID = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupFOWSTID"))
            'cinco.FOWSTID = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupFOWSTID"))
            'cinco.FOWSTID = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"
#End If


            If CStr2(WebRequest.Instance.OrgID).ToLower = "d362ffdb-15d1-451f-97dc-c567b94fb410" Then 'Potato Corner CO
                pDTSales = cinco.GetSalesReport()
            Else
                pDTSales = cinco.GetSalesReportFranchisee()
            End If


            Return JsonDictionary.Serialize("Results", pDTSales)
        End Function

        Function GetSalesReportDetails() As String
            Dim cinco As New CincoCorporation
            Dim pDTSales As DataTable

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FowstID"))

            If CStr2(WebRequest.Instance.OrgID).ToLower = "d362ffdb-15d1-451f-97dc-c567b94fb410" Then 'Potato Corner CO
                pDTSales = cinco.GetSalesReport()
            Else
                pDTSales = cinco.GetSalesReportFranchisee()
            End If

            Return JsonDictionary.Serialize("Results", pDTSales)
        End Function

        Function GetESalesReport() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            cinco.DateFrom = CStr2(Common.WebRequest.Data("DateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("DateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.DeviceID = CStr2(Common.WebRequest.Data("DeviceID"))
            'cinco.SelectedMonth = CStr2(Common.WebRequest.Data("Month"))
            'cinco.SelectedYear = CDec2(Common.WebRequest.Data("Year"))
            Common.WebRequest.ProData.CommandTimeout = 600

            Return JsonDictionary.Serialize("Results", cinco.GetESalesReport())
        End Function

        Function GetInvoiceListReport() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FowstID"))
            cinco.DeviceID = CStr2(Common.WebRequest.Data("DeviceID"))
            cinco.DateFrom = CStr2(Common.WebRequest.Data("Date"))

            Return JsonDictionary.Serialize("Results", cinco.GetInvoiceListReport())
        End Function

        Function GetInvoiceDetails()
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            cinco.InvoiceID = CStr2(Common.WebRequest.Data("InvoiceID"))

            Return cinco.GetInvoiceDetails()
        End Function

        Function ExportInvoice()
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            cinco.InvoiceID = CStr2(Request.Params.Get("InvoiceID"))
            cinco.FOWSTID = CStr2(Request.Params.Get("FowstID"))
            cinco.DeviceID = CStr2(Request.Params.Get("DeviceID"))
            Dim file As New System.IO.FileInfo(cinco.ExportInvoice())

            Response.Clear()
            Response.AddHeader("Content-Disposition", "attachment; filename=" & file.Name)
            Response.AddHeader("Content-Length", file.Length.ToString())
            Response.ContentType = "application/octet-stream"
            Response.WriteFile(file.FullName)
            Response.End()

            Return JsonDictionary.Serialize("Result", "")
        End Function

        Function GetVoidItems() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FowstID"))
            cinco.DateFrom = CStr2(Common.WebRequest.Data("Date"))

            Return JsonDictionary.Serialize("Results", cinco.GetVoidItems())
        End Function

        Function GetDeviceUtilizationSummary()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim trig As New Triggers
            Dim pOut As New JsonDictionary
            pOut.Add("Results", trig.GetDeviceUtilizationSummary(Common.WebRequest.Data("dateFrom"), Common.WebRequest.Data("dateTo"), False, Common.WebRequest.Data("GroupID")))

            Return pOut.Serialize()
        End Function

        Function GetHourlySalesReport() As String
            Dim cinco As New CincoCorporation
            Dim pDTHourlyReport As New DataTable
            Dim pDTHourlyReportTemp As DataTable
            Dim hasdata As Boolean

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            'cinco.DateFrom = "2020-01-01"
            'cinco.DateTo = "2021-12-31"
            'cinco.FOWSTID = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupFOWSTID"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupFOWSTID"))
#End If

            pDTHourlyReportTemp = cinco.GetHourlySalesReport()

            pDTHourlyReport.Columns.Add("HourOfDay")
            pDTHourlyReport.Columns.Add("TransactionCount")
            pDTHourlyReport.Columns.Add("ItemCount")
            pDTHourlyReport.Columns.Add("GrossSales")
            pDTHourlyReport.Columns.Add("SalesReturn")
            pDTHourlyReport.Columns.Add("Discount")
            pDTHourlyReport.Columns.Add("HourOfDayInInteger")

            For i As Integer = 0 To 23
                Dim dr As DataRow

                hasdata = False
                dr = pDTHourlyReport.NewRow

                For j = 0 To pDTHourlyReportTemp.Rows.Count - 1
                    If CInt2(pDTHourlyReportTemp.Rows(j).Item("HourOfDayInInteger")) = i Then
                        hasdata = True
                        dr("HourOfDay") = pDTHourlyReportTemp.Rows(j).Item("HourOfDay")
                        dr("TransactionCount") = pDTHourlyReportTemp.Rows(j).Item("TransactionCount")
                        dr("ItemCount") = pDTHourlyReportTemp.Rows(j).Item("ItemCount")
                        dr("GrossSales") = pDTHourlyReportTemp.Rows(j).Item("GrossSales")
                        dr("SalesReturn") = pDTHourlyReportTemp.Rows(j).Item("SalesReturn")
                        dr("Discount") = pDTHourlyReportTemp.Rows(j).Item("Discount")
                        dr("HourOfDayInInteger") = pDTHourlyReportTemp.Rows(j).Item("HourOfDayInInteger")
                    End If
                Next

                If Not hasdata Then
                    dr("HourOfDay") = CStr2(i) + ":00"
                    dr("TransactionCount") = 0
                    dr("ItemCount") = 0
                    dr("GrossSales") = 0
                    dr("SalesReturn") = 0
                    dr("Discount") = 0
                    dr("HourOfDayInInteger") = i
                End If

                pDTHourlyReport.Rows.Add(dr)
            Next

            Return JsonDictionary.Serialize("Results", pDTHourlyReport)
        End Function

        Function GetInventoryMovement() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            'cinco.DateFrom = "2020-01-01"
            'cinco.DateTo = "2021-12-31"
            'cinco.FOWSTID = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetInventoryMovement())
        End Function

        Function InventoryReportCrewVSAMPerGroupDate() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))


            Return JsonDictionary.Serialize("Results", cinco.InventoryReportCrewVSAMPerGroupDate())
        End Function

        Function InventoryReportCrewVSAMPerGroup() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            'cinco.DateFrom = "2020-01-01"
            'cinco.DateTo = "2021-12-31"
            'cinco.FOWSTID = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.InventoryReportCrewVSAMPerGroup())
        End Function

        Function InventoryReportCrewVSAMDate() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))


            Return JsonDictionary.Serialize("Results", cinco.InventoryReportCrewVSAMDate())
        End Function

        Function InventoryReportCrewVSAM() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            'cinco.DateFrom = "2020-01-01"
            'cinco.DateTo = "2021-12-31"
            'cinco.FOWSTID = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.InventoryReportCrewVSAM())
        End Function

        Function GetDeliveryReport() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            'cinco.DateFrom = "2020-01-01"
            'cinco.DateTo = "2021-12-31"
            'cinco.FOWSTID = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
            cinco.Filter = CStr2(Common.WebRequest.Data("Filter"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
            cinco.Filter = CStr2(Common.WebRequest.Data("Filter"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetDeliveryReport())
        End Function

        Function GetMonthlySalesReport() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim pDTSales As DataTable

#If DEBUG Then
            'cinco.DateFrom = "2020-01-01"
            'cinco.DateTo = "2021-12-31"
            'cinco.FOWSTID = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupFOWSTID"))
#Else
             cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupFOWSTID"))
#End If
            If CStr2(WebRequest.Instance.OrgID).ToLower = "d362ffdb-15d1-451f-97dc-c567b94fb410" Then 'Potato Corner CO
                pDTSales = cinco.GetMonthlySalesReport()
            Else
                pDTSales = cinco.GetMonthlySalesReportFranchisee()
            End If

            Return JsonDictionary.Serialize("Results", pDTSales)

        End Function

        Function GetCashHandlingReport() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupFOWSTID"))
#Else
             cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupFOWSTID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetCashHandlingReport())
        End Function

        Function GetSpecialPromoReport() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            'cinco.DateFrom = "2020-01-01"
            'cinco.DateTo = "2021-12-31"
            'cinco.FOWSTID = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupFOWSTID"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupFOWSTID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetSpecialPromoReport())
        End Function

        Function GetSpecialPromoReportDetails() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))

            Return JsonDictionary.Serialize("Results", cinco.GetSpecialPromoReportDetails())
        End Function

        Function GetConsolidatedPromoReport() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            'cinco.DateFrom = "2020-01-01"
            'cinco.DateTo = "2021-12-31"
            'cinco.FOWSTID = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetConsolidatedPromoReport())
        End Function

        Function GetSpecialDiscountReport() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            'cinco.DateFrom = "2020-01-01"
            'cinco.DateTo = "2021-12-31"
            'cinco.FOWSTID = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupFOWSTID"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupFOWSTID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetSpecialDiscountReport())
        End Function

        Function GetConsolidatedSCDReport() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            'cinco.DateFrom = "2020-01-01"
            'cinco.DateTo = "2021-12-31"
            'cinco.FOWSTID = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetConsolidatedSCDReport())
        End Function

        Function GetSalesByProduct() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim pDTSales As DataTable

#If DEBUG Then
            'cinco.DateFrom = "2020-01-01"
            'cinco.DateTo = "2021-12-31"
            'cinco.FOWSTID = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
             cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            If CStr2(WebRequest.Instance.OrgID).ToLower = "d362ffdb-15d1-451f-97dc-c567b94fb410" Then 'Potato Corner CO
                pDTSales = cinco.GetSalesByProduct()
            Else
                pDTSales = cinco.GetSalesByProductFranchisee()
            End If

            Return JsonDictionary.Serialize("Results", pDTSales)

        End Function

        Function GetCombosProductMix() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
             cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetCombosProductMix())
        End Function

        Function GetProductMix() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
             cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetProductMix())
        End Function

        Function GetProductMixPerWeek() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
             cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If
            Return JsonDictionary.Serialize("Results", cinco.GetProductMixPerWeek())

        End Function

        Function GetFlavorMix() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
             cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetFlavorMix())
        End Function

        Function ProductPerformanceTracker() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            'cinco.DateFrom = "2020-01-01"
            'cinco.DateTo = "2021-12-31"
            'cinco.FOWSTID = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
             cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.ProductPerformanceTracker())
        End Function

        Function ProductPerformanceTrackerComboMix() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            'cinco.DateFrom = "2020-01-01"
            'cinco.DateTo = "2021-12-31"
            'cinco.FOWSTID = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
             cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.ProductPerformanceTrackerComboMix())
        End Function

        Function GetConsumptionReport() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            'cinco.DateFrom = "2020-01-01"
            'cinco.DateTo = "2021-12-31"
            'cinco.FOWSTID = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
             cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetConsumptionReport())
        End Function

        Function GetDailySalesPerSKUReport() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            'cinco.DateFrom = "2020-01-01"
            'cinco.DateTo = "2021-12-31"
            'cinco.FOWSTID = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
             cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetDailySalesPerSKUReport())
        End Function

        Function GetDailySalesPerStoreReport() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
             cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetDailySalesPerStoreReport())
        End Function

        Function GetFoodAggregatorsReport() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
             cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            Try
                Return JsonDictionary.Serialize("Results", cinco.GetFoodAggregatorsReport())
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try

        End Function

        Function GetDailyHourlySalesReport() As String
            Dim cinco As New CincoCorporation
            Dim pDTHourlyReport As New DataTable
            Dim pDTHourlyReportTemp As DataTable
            Dim hasdata As Boolean

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            'cinco.DateFrom = "2020-01-01"
            'cinco.DateTo = "2021-12-31"
            'cinco.FOWSTID = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            pDTHourlyReportTemp = cinco.GetDailyHourlySalesReport()

            Return JsonDictionary.Serialize("Results", pDTHourlyReportTemp)
        End Function

        Function GetHourlySalesByWeekCategoryReport() As String
            Dim cinco As New CincoCorporation
            Dim pDTHourlyReport As New DataTable
            Dim pDTHourlyReportTemp As DataTable
            Dim hasdata As Boolean

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            'cinco.DateFrom = "2020-01-01"
            'cinco.DateTo = "2021-12-31"
            'cinco.FOWSTID = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            pDTHourlyReportTemp = cinco.GetHourlySalesByWeekCategoryReport()

            Return JsonDictionary.Serialize("Results", pDTHourlyReportTemp)
        End Function

        Function GetDisplayDailyProductMixReport() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            'cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
             cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            'cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetDisplayDailyProductMixReport())
        End Function

        Function GetDailyProductMixPerStoreReport() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
             cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetDailyProductMixPerStoreReport())
        End Function

        Function GetProductMonitoringReport() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
             cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetProductMonitoringReport())
        End Function

        Function GetFlavorMonitoringReport() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
             cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetFlavorMonitoringReport())
        End Function

        Function GetProductMonitoringPerStoreProductReport() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
             cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetProductMonitoringPerStoreProductReport())
        End Function

        Function GetProductProductMonitoringMonthlySummary() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
             cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetProductProductMonitoringMonthlySummary())
        End Function

        Function GetSalesTransactionTypeAndTransactionCount() As String
            Dim cinco As New CincoCorporation
            Dim pDTSalesTransactionTypeAndTransactionCount As DataTable
            Dim hasdata As Boolean

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            'cinco.DateFrom = "2020-01-01"
            'cinco.DateTo = "2021-12-31"
            'cinco.FOWSTID = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            pDTSalesTransactionTypeAndTransactionCount = cinco.GetSalesTransactionTypeAndTransactionCount()

            Return JsonDictionary.Serialize("Results", pDTSalesTransactionTypeAndTransactionCount)
        End Function

        Function GetSalesSummaryReport() As String
            Dim cinco As New CincoCorporation
            Dim pDTSalesTransactionTypeAndTransactionCount As DataTable
            Dim hasdata As Boolean

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            'cinco.DateFrom = "2020-01-01"
            'cinco.DateTo = "2021-12-31"
            'cinco.FOWSTID = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            pDTSalesTransactionTypeAndTransactionCount = cinco.GetSalesSummaryReport()

            Return JsonDictionary.Serialize("Results", pDTSalesTransactionTypeAndTransactionCount)
        End Function

        Function GetSalesAndTransactionCountMixReport() As String
            Dim cinco As New CincoCorporation
            Dim pDTSalesTransactionTypeAndTransactionCount As DataTable
            Dim hasdata As Boolean

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            'cinco.DateFrom = "2020-01-01"
            'cinco.DateTo = "2021-12-31"
            'cinco.FOWSTID = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            pDTSalesTransactionTypeAndTransactionCount = cinco.GetSalesAndTransactionCountMixReport()

            Return JsonDictionary.Serialize("Results", pDTSalesTransactionTypeAndTransactionCount)
        End Function

        Function GetADSandATCperStoreReport() As String
            Dim cinco As New CincoCorporation
            Dim pDTSalesTransactionTypeAndTransactionCount As DataTable
            Dim hasdata As Boolean

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            'cinco.DateFrom = "2020-01-01"
            'cinco.DateTo = "2021-12-31"
            'cinco.FOWSTID = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            pDTSalesTransactionTypeAndTransactionCount = cinco.GetADSandATCperStoreReport()

            Return JsonDictionary.Serialize("Results", pDTSalesTransactionTypeAndTransactionCount)
        End Function

        Function GetAverageDailySalesAndTransactionSales() As String
            Dim cinco As New CincoCorporation
            Dim pDTSalesTransactionTypeAndTransactionCount As DataTable
            Dim hasdata As Boolean

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            'cinco.DateFrom = "2020-01-01"
            'cinco.DateTo = "2021-12-31"
            'cinco.FOWSTID = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            pDTSalesTransactionTypeAndTransactionCount = cinco.GetSalesReport()

            Return JsonDictionary.Serialize("Results", pDTSalesTransactionTypeAndTransactionCount)
        End Function


        Function GetConsumptionReportPerType() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            'cinco.DateFrom = "2020-01-01"
            'cinco.DateTo = "2021-12-31"
            'cinco.FOWSTID = "FE639D1F-CD83-4B3E-8B0A-586731F479CB"
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
             cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetConsumptionReportPerType())
        End Function

        Function GetAverageDailySales() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
#Else
             cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetAverageDailySales())
        End Function

        Function GetTopAndBottomRankingStores() As String
            Dim cinco As New CincoCorporation
            Dim pOut As JsonDictionary

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut = New JsonDictionary

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupFOWSTID"))
#Else
             cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupFOWSTID"))
#End If

            pOut.Add("ASC", cinco.GetTopAndBottomRankingStores(True))

            pOut.Add("DESC", cinco.GetTopAndBottomRankingStores(False))

            Return pOut.Serialize

            'Return JsonDictionary.Serialize(pOut)
        End Function

        Function GetSalesReportSliceAndDice() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupFOWSTID"))
            cinco.SliceBy = CStr2(Common.WebRequest.Data("SliceBy"))
            cinco.SliceByFilter = CStr2(Common.WebRequest.Data("SliceByFilter"))
            cinco.DiceBy = CStr2(Common.WebRequest.Data("DiceBy"))
            cinco.DiceByFilter = CStr2(Common.WebRequest.Data("DiceByFilter"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupFOWSTID"))
            cinco.SliceBy = CStr2(Common.WebRequest.Data("SliceBy"))
            cinco.SliceByFilter = CStr2(Common.WebRequest.Data("SliceByFilter"))
            cinco.DiceBy = CStr2(Common.WebRequest.Data("DiceBy"))
            cinco.DiceByFilter = CStr2(Common.WebRequest.Data("DiceByFilter"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetSalesReportSliceAndDice())
        End Function

        Function GetDeviceMonitoringData() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupFOWSTID"))
#Else
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupFOWSTID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetDeviceMonitoringData())
        End Function

        Function GetDeviceAvailability() As String
            Dim cinco As New CincoCorporation
            Dim dtDeviceAvailability As DataTable
            Dim dtDeviceAvailabilitySummary As DataTable
            Dim pOut As JsonDictionary

            pOut = New JsonDictionary

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            dtDeviceAvailability = cinco.GetDeviceAvailability()
            dtDeviceAvailabilitySummary = cinco.GetDeviceAvailabilitySummary

            pOut.Add("DeviceAvailability", dtDeviceAvailability)
            pOut.Add("DeviceAvailabilitySummary", dtDeviceAvailabilitySummary)

            Return pOut.Serialize
        End Function

        Function GetDeviceStatus()
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", cinco.GetDeviceStatus())
        End Function

        Function GetClosedStores()
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", cinco.GetClosedStores())
        End Function

        Function SaveFowstOpHrs()
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", cinco.SaveFowstOpHrs())
        End Function

        Function GetDeviceFOWSTAndOperatingHours()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String
                Dim deviceid As String = Common.WebRequest.Data("DeviceID")

                pSql = " SELECT fowst.ID, fowst.Name as FowstName " & vbCrLf &
                        " FROM MobizLibraryOrganizationFOWST as fowst " & vbCrLf &
                        " INNER JOIN LibraryDeviceFowst as dev ON dev.FowstID = fowst.ID " & vbCrLf &
                        " WHERE dev.DeviceID = " & ReadyString(deviceid)
                Dim pRows As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

                For Each fowst As Object In pRows
                    pSql = "SELECT WeekDay FROM MobizLibraryOrganizationFOWSTOperationHRs WHERE ISNULL(IsClosed, 0) = 1 AND FOWSTID = " & ReadyString(fowst("ID").ToString())
                    fowst.Add("Closed", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))
                Next

                Return Json(New With {Key .Details = pRows})
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function TagAsDummyDevice() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", cinco.TagAsDummyDevice())
        End Function

        Function SetReasonforUnavailability() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", cinco.SetReasonforUnavailability())
        End Function

        Function DeactivateDeviceMonitoringData() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", cinco.DeactivateDeviceMonitoringData())
        End Function

        Function GetDeviceFlushingQueueMonitoring() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupFOWSTID"))
#Else
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupFOWSTID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetDeviceFlushingQueueMonitoring())
        End Function

        Function GetDeviceFlushingQueue() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupFOWSTID"))
#Else
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupFOWSTID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetDeviceFlushingQueue())
        End Function

        Function GetStoreUnflushedCount() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", cinco.GetStoreUnflushedCount())
        End Function

        Function GetSystemWideFlushingHistory() As String
            Dim cinco As New CincoCorporation

            Return JsonDictionary.Serialize("Results", cinco.GetSystemWideFlushingHistory())
        End Function

        Function StoreUnflushedCountHistory() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", cinco.StoreUnflushedCountHistory())
        End Function

        Function FlushingBreakdown() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", cinco.FlushingBreakdown())
        End Function


        Function GetDeviceUtilization() As String
            Dim cinco As New CincoCorporation
            Dim pDTHourlyReport As New DataTable
            Dim pDTHourlyReportTemp As DataTable
            Dim hasdata As Boolean

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
            cinco.StoreStatus = CStr2(Common.WebRequest.Data("StoreStatus"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
            cinco.StoreStatus = CStr2(Common.WebRequest.Data("StoreStatus"))
#End If

            Dim pOut As New JsonDictionary
            Dim pSql = " SELECT ISNULL(Status, 'Others') as Status, COUNT(ISNULL(Status, 'Others')) as Count " & vbCrLf &
                        " FROM MobizLibraryOrganizationFOWST " & vbCrLf &
                        " WHERE OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                        " AND IsActive = 1 " & vbCrLf &
                        " AND FOWSTTypeID <> 'DFF0F087-4C2B-4B67-AA1D-502F02773E2B' -- FOWST for Mobiz Project" & vbCrLf &
                        " GROUP BY Status " & vbCrLf

            'pOut.Add("Summary", Common.WebRequest.ProData.Execute(pSql))
            pOut.Add("Results", cinco.GetDeviceUtilization())

            Return pOut.Serialize()
        End Function

        Function GetDeviceUtilizationByHour() As String
            Dim cinco As New CincoCorporation
            Dim pDTHourlyReport As New DataTable
            Dim pDTHourlyReportTemp As DataTable
            Dim hasdata As Boolean

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetDeviceUtilizationByHour())
        End Function

        Function GetSalesPaymentInformationReport() As String
            Dim cinco As New CincoCorporation
            Dim pDTHourlyReport As New DataTable
            Dim pDTHourlyReportTemp As DataTable
            Dim hasdata As Boolean

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetSalesPaymentInformationReport())
        End Function

        Function GetCrewOnDutyReport() As String
            Dim cinco As New CincoCorporation
            Dim pDTHourlyReport As New DataTable
            Dim pDTHourlyReportTemp As DataTable
            Dim hasdata As Boolean

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetCrewOnDutyReport())
        End Function

        Function GetFoodCostReport() As String
            Dim cinco As New CincoCorporation
            Dim pDTHourlyReport As New DataTable
            Dim pDTHourlyReportTemp As DataTable
            Dim hasdata As Boolean

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
#End If

            Return JsonDictionary.Serialize("Results", cinco.GetFoodCostReport())
        End Function

        'Management Reports 12/17/2023

        Function GetSalesByDate() As String
            Dim cinco As New CincoCorporation
            Dim dtSalesYesterday As DataTable
            Dim dtLastYearSalesYesterday As DataTable

            Dim dtSalesMonthToDate As DataTable
            Dim dtLastYearSalesMonthToDate As DataTable

            Dim dtSalesYearToDate As DataTable
            Dim dtLastYearSalesYearToDate As DataTable


            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = Date.Now.AddDays(-1)
            cinco.DateTo = Date.Now.AddDays(-1)
#Else
            cinco.DateFrom = Date.Now.AddDays(-1)
            cinco.DateTo = Date.Now.AddDays(-1)
#End If

            dtSalesYesterday = cinco.GetSalesByDate

#If DEBUG Then
            cinco.DateFrom = Date.Now.AddDays(-366)
            cinco.DateTo = Date.Now.AddDays(-366)
#Else
            cinco.DateFrom = Date.Now.AddDays(-366)
            cinco.DateTo = Date.Now.AddDays(-366)
#End If

            dtLastYearSalesYesterday = cinco.GetSalesByDate
            

#If DEBUG Then
            cinco.DateFrom = Date.Now.AddDays((Date.Now.Day - 1) * -1)
            cinco.DateTo = Date.Now.AddDays(-1)
#Else
             cinco.DateFrom = Date.Now.AddDays((Date.Now.Day - 1) * -1)
            cinco.DateTo = Date.Now.AddDays(-1)
#End If

            dtSalesMonthToDate = cinco.GetSalesByDate


#If DEBUG Then
            cinco.DateFrom = Date.Now.AddDays((Date.Now.Day - 1) * -1).AddDays(-365)
            cinco.DateTo = Date.Now.AddDays(-366)
#Else
             cinco.DateFrom = Date.Now.AddDays((Date.Now.Day - 1) * -1).AddDays(-365)
            cinco.DateTo = Date.Now.AddDays(-366)
#End If

            dtLastYearSalesMonthToDate = cinco.GetSalesByDate

#If DEBUG Then
            cinco.DateFrom = "01/01/" & Now.Year
            cinco.DateTo = Date.Now.AddDays(-1)
#Else
            cinco.DateFrom = Date.Now.AddDays(-365)
            cinco.DateTo = Date.Now.AddDays(-1)
#End If

            dtSalesYearToDate = cinco.GetSalesByDate

#If DEBUG Then
            cinco.DateFrom = "01/01/" & Now.Year - 1
            cinco.DateTo = Date.Now.AddDays(-1 * 366)
#Else
            cinco.DateFrom = Date.Now.AddDays(-365*2)
            cinco.DateTo = Date.Now.AddDays(-1*366)
#End If

            dtLastYearSalesYearToDate = cinco.GetSalesByDate


            Dim pOut As JsonDictionary

            pOut = New JsonDictionary
            pOut.Add("YesterdaySales", dtSalesYesterday)
            pOut.Add("LastYearYesterdaySales", dtLastYearSalesYesterday)
            pOut.Add("MTD", dtSalesMonthToDate)
            pOut.Add("LastYearMTD", dtLastYearSalesMonthToDate)
            pOut.Add("YTD", dtSalesYearToDate)
            pOut.Add("LastYearYTD", dtLastYearSalesYearToDate)

            Return pOut.Serialize


        End Function

        Function GetSalesMixByDate() As String
            Dim cinco As New CincoCorporation

            Dim dtYesterdayTakeOut As DataTable
            Dim dtYesterdayDelivery As DataTable

            Dim dtMonthToDateTakeOut As DataTable
            Dim dtMonthToDateDelivery As DataTable

            Dim dtYearToDateTakeOut As DataTable
            Dim dtYearToDateDelivery As DataTable


            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = Date.Now.AddDays(-1)
            cinco.DateTo = Date.Now.AddDays(-1)
#Else
            cinco.DateFrom = Date.Now.AddDays(-1)
            cinco.DateTo = Date.Now.AddDays(-1)
#End If

            dtYesterdayTakeOut = cinco.GetTakeOutSales
            dtYesterdayDelivery = cinco.GetDeliverySales

#If DEBUG Then
            cinco.DateFrom = Date.Now.AddDays((Date.Now.Day - 1) * -1)
            cinco.DateTo = Date.Now.AddDays(-1)
#Else
             cinco.DateFrom = Date.Now.AddDays((Date.Now.Day - 1) * -1)
            cinco.DateTo = Date.Now.AddDays(-1)
#End If

            dtMonthToDateTakeOut = cinco.GetTakeOutSales
            dtMonthToDateDelivery = cinco.GetDeliverySales

#If DEBUG Then
            cinco.DateFrom = "01/01/" & Now.Year
            cinco.DateTo = Date.Now.AddDays(-1)
#Else
            cinco.DateFrom = Date.Now.AddDays(-365)
            cinco.DateTo = Date.Now.AddDays(-1)
#End If

            dtYearToDateTakeOut = cinco.GetTakeOutSales
            dtYearToDateDelivery = cinco.GetDeliverySales


            Dim pOut As JsonDictionary

            pOut = New JsonDictionary
            pOut.Add("YesterdayTakeOut", dtYesterdayTakeOut)
            pOut.Add("YesterdayDelivery", dtYesterdayDelivery)

            pOut.Add("MTDTakeOut", dtMonthToDateTakeOut)
            pOut.Add("MTDDelivery", dtMonthToDateDelivery)

            pOut.Add("YTDTakeOut", dtYearToDateTakeOut)
            pOut.Add("YTDDelivery", dtYearToDateDelivery)

            Return pOut.Serialize


        End Function

        Function GetWeeklySalesReport() As String
            Dim cinco As New CincoCorporation
            Dim dtSalesperOwnership As DataTable
            Dim dtDeliverySalesperOwnershipDelivery As DataTable
            Dim dtTakeOutSalesperOwnershipTakeOut As DataTable
            Dim dtSalesperRegion As DataTable
            Dim dtSalesperLocationType As DataTable
            Dim pOut As JsonDictionary

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))            
#End If

            dtSalesperOwnership = cinco.GetSalesperOwnership
            dtDeliverySalesperOwnershipDelivery = cinco.GetSalesperOwnershipDelivery
            dtTakeOutSalesperOwnershipTakeOut = cinco.GetSalesperOwnershipTakeOut
            dtSalesperRegion = cinco.GetSalesperRegion
            dtSalesperLocationType = cinco.GetSalesperLocationType

            pOut = New JsonDictionary
            pOut.Add("SalesperOwnership", dtSalesperOwnership)
            pOut.Add("DeliverySalesperOwnershipDelivery", dtDeliverySalesperOwnershipDelivery)
            pOut.Add("TakeOutSalesperOwnershipTakeOut", dtTakeOutSalesperOwnershipTakeOut)
            pOut.Add("SalesperRegion", dtSalesperRegion)
            pOut.Add("SalesperLocationType", dtSalesperLocationType)

            Return pOut.Serialize
        End Function

        Function GetFourWeeklySalesReports() As String
            Dim cinco As New CincoCorporation
            Dim dtSalesperOwnership As DataTable
            Dim dtDeliverySalesperOwnershipDelivery As DataTable
            Dim dtTakeOutSalesperOwnershipTakeOut As DataTable
            Dim dtSalesperRegion As DataTable
            Dim dtSalesperLocationType As DataTable
            Dim pOut As JsonDictionary
            Dim dtMain As New DataTable
            Dim DateFrom As Date
            Dim DateTo As Date
            Dim SelectedWeek As String
            Dim SelectedWeekNo As Integer
            Dim dr As DataRow
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))            
#End If
            SelectedWeek = CStr2(Common.WebRequest.Data("SelectedWeek"))
            SelectedWeekNo = CInt2(Common.WebRequest.Data("WeekNo"))
            DateFrom = CDate(Common.WebRequest.Data("dateFrom"))
            DateTo = CDate(Common.WebRequest.Data("dateTo"))

            dtMain = CreateTableHeader("Sales")

            Dim DateHeaderFrom As String
            Dim DateHeaderTo As String
            Dim DateHeader As String

            For i As Integer = 0 To 3

                dtSalesperOwnership = cinco.GetSalesperOwnership
                dtDeliverySalesperOwnershipDelivery = cinco.GetSalesperOwnershipDelivery
                dtTakeOutSalesperOwnershipTakeOut = cinco.GetSalesperOwnershipTakeOut
                dtSalesperRegion = cinco.GetSalesperRegion
                dtSalesperLocationType = cinco.GetSalesperLocationType

                Dim SalesperOwnershipCO As Double = 0
                Dim SalesperOwnershipFO As Double = 0
                Dim TotalSalesperOwnership As Double = 0

                DateHeaderFrom = DateFrom.ToString("MMM d")
                DateHeaderTo = DateTo.ToString("MMM d")
                DateHeader = DateHeaderFrom & "-" & DateHeaderTo
                dtMain.Columns.Add(DateHeader)

                dr = dtMain.NewRow

                For j As Integer = 0 To dtSalesperOwnership.Rows.Count - 1
                    If CBool2(dtSalesperOwnership.Rows(j).Item("FromFranchisee")) Then
                        SalesperOwnershipFO = CDec2(dtSalesperOwnership.Rows(j).Item("GrossSales"))
                    Else
                        SalesperOwnershipCO = CDec2(dtSalesperOwnership.Rows(j).Item("GrossSales"))
                    End If

                    TotalSalesperOwnership += CDec2(dtSalesperOwnership.Rows(j).Item("GrossSales"))
                Next

                dtMain.Rows(0).Item(DateHeader) = "Week " & SelectedWeekNo
                dtMain.Rows(1).Item(DateHeader) = Format(SalesperOwnershipCO, "#,###,###.##")
                dtMain.Rows(2).Item(DateHeader) = Format(SalesperOwnershipFO, "#,###,###.##")
                dtMain.Rows(3).Item(DateHeader) = Format(TotalSalesperOwnership, "#,###,###.##")
                dtMain.Rows(4).Item(DateHeader) = ""
                dtMain.Rows(5).Item(DateHeader) = ""

                Dim DeliverySalesperOwnershipCO As Double = 0
                Dim DeliverySalesperOwnershipFO As Double = 0
                Dim TotalDeliverySalesperOwnership As Double = 0

                For j As Integer = 0 To dtDeliverySalesperOwnershipDelivery.Rows.Count - 1
                    If CBool2(dtDeliverySalesperOwnershipDelivery.Rows(j).Item("FromFranchisee")) Then
                        DeliverySalesperOwnershipFO = CDec2(dtDeliverySalesperOwnershipDelivery.Rows(j).Item("GrossSales"))
                    Else
                        DeliverySalesperOwnershipCO = CDec2(dtDeliverySalesperOwnershipDelivery.Rows(j).Item("GrossSales"))
                    End If

                    TotalDeliverySalesperOwnership += CDec2(dtDeliverySalesperOwnershipDelivery.Rows(j).Item("GrossSales"))
                Next

                dtMain.Rows(6).Item(DateHeader) = Format(DeliverySalesperOwnershipCO, "#,###,###.##")
                dtMain.Rows(7).Item(DateHeader) = Format(DeliverySalesperOwnershipFO, "#,###,###.##")
                dtMain.Rows(8).Item(DateHeader) = Format(TotalDeliverySalesperOwnership, "#,###,###.##")
                dtMain.Rows(9).Item(DateHeader) = ""
                dtMain.Rows(10).Item(DateHeader) = ""

                Dim TakeOutSalesperOwnershipCO As Double = 0
                Dim TakeOutSalesperOwnershipFO As Double = 0
                Dim TotalTakeOutSalesperOwnership As Double = 0

                For j As Integer = 0 To dtTakeOutSalesperOwnershipTakeOut.Rows.Count - 1
                    If CBool2(dtTakeOutSalesperOwnershipTakeOut.Rows(j).Item("FromFranchisee")) Then
                        TakeOutSalesperOwnershipFO = CDec2(dtTakeOutSalesperOwnershipTakeOut.Rows(j).Item("GrossSales"))
                    Else
                        TakeOutSalesperOwnershipCO = CDec2(dtTakeOutSalesperOwnershipTakeOut.Rows(j).Item("GrossSales"))
                    End If

                    TotalTakeOutSalesperOwnership += CDec2(dtTakeOutSalesperOwnershipTakeOut.Rows(j).Item("GrossSales"))
                Next

                dtMain.Rows(11).Item(DateHeader) = Format(TakeOutSalesperOwnershipCO, "#,###,###.##")
                dtMain.Rows(12).Item(DateHeader) = Format(TakeOutSalesperOwnershipFO, "#,###,###.##")
                dtMain.Rows(13).Item(DateHeader) = Format(TotalTakeOutSalesperOwnership, "#,###,###.##")
                dtMain.Rows(14).Item(DateHeader) = ""
                dtMain.Rows(15).Item(DateHeader) = ""

                Dim COGMAN As Double = 0
                Dim COGMAS As Double = 0
                Dim CONorthLuzon As Double = 0
                Dim COSouthLuzon As Double = 0
                Dim COTotalLuzon As Double = 0
                Dim COVisayas As Double = 0
                Dim COMindanao As Double = 0
                Dim COTotalVisMin As Double = 0

                Dim FOGMAN As Double = 0
                Dim FOGMAS As Double = 0
                Dim FONorthLuzon As Double = 0
                Dim FOSouthLuzon As Double = 0
                Dim FOTotalLuzon As Double = 0
                Dim FOVisayas As Double = 0
                Dim Cebu As Double = 0
                Dim FOMindanao As Double = 0
                Dim FOTotalVisMin As Double = 0

                For j As Integer = 0 To dtSalesperRegion.Rows.Count - 1
                    Select Case CStr2(dtSalesperRegion.Rows(j).Item("GroupName"))
                        Case "CO - GMAN"
                            COGMAN = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                        Case "CO - GMAS"
                            COGMAS = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                        Case "CO - North Luzon"
                            CONorthLuzon = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                        Case "CO - South Luzon"
                            COSouthLuzon = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                        Case "CO - Visayas"
                            COVisayas = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                        Case "CO - Mindanao"
                            COMindanao = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                        Case "FO - GMAN"
                            FOGMAN = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                        Case "FO - GMAS"
                            FOGMAS = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                        Case "FO - North Luzon"
                            FONorthLuzon = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                        Case "FO - South Luzon"
                            FOSouthLuzon = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                        Case "FO - Visayas"
                            FOVisayas = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                        Case "Cebu"
                            Cebu = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                        Case "Cebu"
                            COGMAN = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                        Case "FO - Mindanao"
                            FOMindanao = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                    End Select
                Next

                COTotalLuzon = COGMAN + COGMAS + CONorthLuzon + COSouthLuzon
                COTotalVisMin = COVisayas + COMindanao

                FOTotalLuzon = FOGMAN + FOGMAS + FONorthLuzon + FOSouthLuzon
                FOTotalVisMin = FOVisayas + Cebu + FOMindanao

                dtMain.Rows(16).Item(DateHeader) = Format(COGMAN, "#,###,###.##")
                dtMain.Rows(17).Item(DateHeader) = Format(COGMAS, "#,###,###.##")
                dtMain.Rows(18).Item(DateHeader) = Format(CONorthLuzon, "#,###,###.##")
                dtMain.Rows(19).Item(DateHeader) = Format(COSouthLuzon, "#,###,###.##")
                dtMain.Rows(20).Item(DateHeader) = Format(COTotalLuzon, "#,###,###.##")
                dtMain.Rows(21).Item(DateHeader) = ""
                dtMain.Rows(22).Item(DateHeader) = Format(COVisayas, "#,###,###.##")
                dtMain.Rows(23).Item(DateHeader) = ""
                dtMain.Rows(24).Item(DateHeader) = Format(COMindanao, "#,###,###.##")
                dtMain.Rows(25).Item(DateHeader) = Format(COTotalVisMin, "#,###,###.##")
                dtMain.Rows(26).Item(DateHeader) = ""
                dtMain.Rows(27).Item(DateHeader) = ""
                dtMain.Rows(28).Item(DateHeader) = Format(FOGMAN, "#,###,###.##")
                dtMain.Rows(29).Item(DateHeader) = Format(FOGMAS, "#,###,###.##")
                dtMain.Rows(30).Item(DateHeader) = Format(FONorthLuzon, "#,###,###.##")
                dtMain.Rows(31).Item(DateHeader) = Format(FOSouthLuzon, "#,###,###.##")
                dtMain.Rows(32).Item(DateHeader) = Format(FOTotalLuzon, "#,###,###.##")
                dtMain.Rows(33).Item(DateHeader) = ""
                dtMain.Rows(34).Item(DateHeader) = Format(FOVisayas, "#,###,###.##")
                dtMain.Rows(35).Item(DateHeader) = Format(Cebu, "#,###,###.##")
                dtMain.Rows(36).Item(DateHeader) = Format(FOMindanao, "#,###,###.##")
                dtMain.Rows(37).Item(DateHeader) = Format(FOTotalVisMin, "#,###,###.##")
                dtMain.Rows(38).Item(DateHeader) = ""
                dtMain.Rows(39).Item(DateHeader) = ""
                dtMain.Rows(40).Item(DateHeader) = Format(COGMAN + FOGMAN, "#,###,###.##")
                dtMain.Rows(41).Item(DateHeader) = Format(COGMAS + FOGMAS, "#,###,###.##")
                dtMain.Rows(42).Item(DateHeader) = Format(CONorthLuzon + FONorthLuzon, "#,###,###.##")
                dtMain.Rows(43).Item(DateHeader) = Format(COSouthLuzon + FOSouthLuzon, "#,###,###.##")
                dtMain.Rows(44).Item(DateHeader) = Format(COTotalLuzon + FOTotalLuzon, "#,###,###.##")
                dtMain.Rows(45).Item(DateHeader) = ""
                dtMain.Rows(46).Item(DateHeader) = Format(COVisayas + FOVisayas, "#,###,###.##")
                dtMain.Rows(47).Item(DateHeader) = Format(Cebu, "#,###,###.##")
                dtMain.Rows(48).Item(DateHeader) = Format(COMindanao + FOMindanao, "#,###,###.##")
                dtMain.Rows(49).Item(DateHeader) = Format(COTotalVisMin + FOTotalVisMin, "#,###,###.##")
                dtMain.Rows(50).Item(DateHeader) = Format(COTotalLuzon + FOTotalLuzon + COTotalVisMin + FOTotalVisMin, "#,###,###.##")

                dtMain.Rows(51).Item(DateHeader) = ""
                dtMain.Rows(52).Item(DateHeader) = ""

                Dim COAirport As Double = 0
                Dim COAmusementPark As Double = 0
                Dim COChurch As Double = 0
                Dim COCinema As Double = 0
                Dim COCommercial As Double = 0
                Dim COEntertainment As Double = 0
                Dim COFoodCourt As Double = 0
                Dim COGasolineStation As Double = 0
                Dim COHospital As Double = 0
                Dim COMall As Double = 0
                Dim COMallCinema As Double = 0
                Dim COMallEntertainment As Double = 0
                Dim COMallFoodcourt As Double = 0
                Dim COMallProper As Double = 0
                Dim COMallSupermarket As Double = 0
                Dim COOffices As Double = 0
                Dim COPublicMarket As Double = 0
                Dim COResidential As Double = 0
                Dim COResort As Double = 0
                Dim COSchool As Double = 0
                Dim COSupermarket As Double = 0
                Dim COTerminal As Double = 0
                Dim TotalCOSalesperLocationType As Double = 0

                Dim FOAirport As Double = 0
                Dim FOAmusementPark As Double = 0
                Dim FOChurch As Double = 0
                Dim FOCinema As Double = 0
                Dim FOCommercial As Double = 0
                Dim FOEntertainment As Double = 0
                Dim FOFoodCourt As Double = 0
                Dim FOGasolineStation As Double = 0
                Dim FOHospital As Double = 0
                Dim FOMall As Double = 0
                Dim FOMallCinema As Double = 0
                Dim FOMallEntertainment As Double = 0
                Dim FOMallFoodcourt As Double = 0
                Dim FOMallProper As Double = 0
                Dim FOMallSupermarket As Double = 0
                Dim FOOffices As Double = 0
                Dim FOPublicMarket As Double = 0
                Dim FOResidential As Double = 0
                Dim FOResort As Double = 0
                Dim FOSchool As Double = 0
                Dim FOSupermarket As Double = 0
                Dim FOTerminal As Double = 0
                Dim TotalFOSalesperLocationType As Double = 0

                Dim TotalAirport As Double = 0
                Dim TotalAmusementPark As Double = 0
                Dim TotalChurch As Double = 0
                Dim TotalCinema As Double = 0
                Dim TotalCommercial As Double = 0
                Dim TotalEntertainment As Double = 0
                Dim TotalFoodCourt As Double = 0
                Dim TotalGasolineStation As Double = 0
                Dim TotalHospital As Double = 0
                Dim TotalMall As Double = 0
                Dim TotalMallCinema As Double = 0
                Dim TotalMallEntertainment As Double = 0
                Dim TotalMallFoodcourt As Double = 0
                Dim TotalMallProper As Double = 0
                Dim TotalMallSupermarket As Double = 0
                Dim TotalOffices As Double = 0
                Dim TotalPublicMarket As Double = 0
                Dim TotalResidential As Double = 0
                Dim TotalResort As Double = 0
                Dim TotalSchool As Double = 0
                Dim TotalSupermarket As Double = 0
                Dim TotalTerminal As Double = 0
                Dim TotalSalesperLocationType As Double = 0

                For j As Integer = 0 To dtSalesperLocationType.Rows.Count - 1
                    Select Case CStr2(dtSalesperLocationType.Rows(j).Item("StoreLocation"))
                        Case "Airport"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOAirport = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOAirport
                            Else
                                COAirport = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COAirport
                            End If
                            TotalAirport += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Amusement Park"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOAmusementPark = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOAmusementPark
                            Else
                                COAmusementPark = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COAmusementPark
                            End If
                            TotalAmusementPark += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Commercial"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOCommercial = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOCommercial
                            Else
                                COCommercial = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COCommercial
                            End If
                            TotalCommercial += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Church"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOChurch = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOChurch
                            Else
                                COChurch = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COChurch
                            End If
                            TotalChurch += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Cinema"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOCinema = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOCinema
                            Else
                                COCinema = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COCinema
                            End If
                            TotalCinema += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Entertainment"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOEntertainment = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOEntertainment
                            Else
                                COEntertainment = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COEntertainment
                            End If
                            TotalEntertainment += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Food Court"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOFoodCourt = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOFoodCourt
                            Else
                                COFoodCourt = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COFoodCourt
                            End If
                            TotalFoodCourt += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Gasoline Station"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOGasolineStation = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOGasolineStation
                            Else
                                COGasolineStation = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COGasolineStation
                            End If
                            TotalGasolineStation += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Hospital"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOHospital = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOHospital
                            Else
                                COHospital = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COHospital
                            End If
                            TotalHospital += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Mall"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMall = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOMall
                            Else
                                COMall = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COMall
                            End If
                            TotalMall += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Mall-Cinema"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMallCinema = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOMallCinema
                            Else
                                COMallCinema = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COMallCinema
                            End If
                            TotalMallCinema += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Mall-Entertainment"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMallEntertainment = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOMallEntertainment
                            Else
                                COMallEntertainment = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COMallEntertainment
                            End If
                            TotalMallEntertainment += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Mall-Foodcourt"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMallFoodcourt = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOMallFoodcourt
                            Else
                                COMallFoodcourt = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COMallFoodcourt
                            End If
                            TotalMallFoodcourt += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Mall-Proper"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMallProper = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOMallProper
                            Else
                                COMallProper = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COMallProper
                            End If
                            TotalMallProper += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Mall-Supermarket"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMallSupermarket = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOMallSupermarket
                            Else
                                COMallSupermarket = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COMallSupermarket
                            End If
                            TotalMallSupermarket += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Offices"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOOffices = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOOffices
                            Else
                                COOffices = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COOffices
                            End If
                            TotalOffices += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "PublicMarket"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOPublicMarket = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOPublicMarket
                            Else
                                COPublicMarket = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COPublicMarket
                            End If
                            TotalPublicMarket += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Residential"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOResidential = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOResidential
                            Else
                                COResidential = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COResidential
                            End If
                            TotalResidential += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Resort"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOResort = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOResort
                            Else
                                COResort = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COResort
                            End If
                            TotalResort += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "School"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOSchool = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOSchool
                            Else
                                COSchool = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COSchool
                            End If
                            TotalSchool += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Supermarket"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOSupermarket = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOSupermarket
                            Else
                                COSupermarket = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COSupermarket
                            End If
                            TotalSupermarket += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Terminal"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOTerminal = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOTerminal
                            Else
                                COTerminal = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COTerminal
                            End If
                            TotalTerminal += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                    End Select
                Next

                dtMain.Rows(53).Item(DateHeader) = Format(COAirport, "#,###,###.##")
                dtMain.Rows(54).Item(DateHeader) = Format(COAmusementPark, "#,###,###.##")
                dtMain.Rows(55).Item(DateHeader) = Format(COCommercial, "#,###,###.##")
                dtMain.Rows(56).Item(DateHeader) = Format(COChurch, "#,###,###.##")
                dtMain.Rows(57).Item(DateHeader) = Format(COCinema, "#,###,###.##")
                dtMain.Rows(58).Item(DateHeader) = Format(COEntertainment, "#,###,###.##")
                dtMain.Rows(59).Item(DateHeader) = Format(COFoodCourt, "#,###,###.##")
                dtMain.Rows(60).Item(DateHeader) = Format(COGasolineStation, "#,###,###.##")
                dtMain.Rows(61).Item(DateHeader) = Format(COHospital, "#,###,###.##")
                dtMain.Rows(62).Item(DateHeader) = Format(COMall, "#,###,###.##")
                dtMain.Rows(63).Item(DateHeader) = Format(COMallCinema, "#,###,###.##")
                dtMain.Rows(64).Item(DateHeader) = Format(COMallEntertainment, "#,###,###.##")
                dtMain.Rows(65).Item(DateHeader) = Format(COMallFoodcourt, "#,###,###.##")
                dtMain.Rows(66).Item(DateHeader) = Format(COMallProper, "#,###,###.##")
                dtMain.Rows(67).Item(DateHeader) = Format(COMallSupermarket, "#,###,###.##")
                dtMain.Rows(68).Item(DateHeader) = Format(COOffices, "#,###,###.##")
                dtMain.Rows(69).Item(DateHeader) = Format(COPublicMarket, "#,###,###.##")
                dtMain.Rows(70).Item(DateHeader) = Format(COResidential, "#,###,###.##")
                dtMain.Rows(71).Item(DateHeader) = Format(COResort, "#,###,###.##")
                dtMain.Rows(72).Item(DateHeader) = Format(COSchool, "#,###,###.##")
                dtMain.Rows(73).Item(DateHeader) = Format(COSupermarket, "#,###,###.##")
                dtMain.Rows(74).Item(DateHeader) = Format(COTerminal, "#,###,###.##")
                dtMain.Rows(75).Item(DateHeader) = Format(TotalCOSalesperLocationType, "#,###,###.##")
                dtMain.Rows(76).Item(DateHeader) = ""
                dtMain.Rows(77).Item(DateHeader) = ""

                dtMain.Rows(78).Item(DateHeader) = Format(FOAirport, "#,###,###.##")
                dtMain.Rows(79).Item(DateHeader) = Format(FOAmusementPark, "#,###,###.##")
                dtMain.Rows(80).Item(DateHeader) = Format(FOCommercial, "#,###,###.##")
                dtMain.Rows(81).Item(DateHeader) = Format(FOChurch, "#,###,###.##")
                dtMain.Rows(82).Item(DateHeader) = Format(FOCinema, "#,###,###.##")
                dtMain.Rows(83).Item(DateHeader) = Format(FOEntertainment, "#,###,###.##")
                dtMain.Rows(84).Item(DateHeader) = Format(FOFoodCourt, "#,###,###.##")
                dtMain.Rows(85).Item(DateHeader) = Format(FOGasolineStation, "#,###,###.##")
                dtMain.Rows(86).Item(DateHeader) = Format(FOHospital, "#,###,###.##")
                dtMain.Rows(87).Item(DateHeader) = Format(FOMall, "#,###,###.##")
                dtMain.Rows(88).Item(DateHeader) = Format(FOMallCinema, "#,###,###.##")
                dtMain.Rows(89).Item(DateHeader) = Format(FOMallEntertainment, "#,###,###.##")
                dtMain.Rows(90).Item(DateHeader) = Format(FOMallFoodcourt, "#,###,###.##")
                dtMain.Rows(91).Item(DateHeader) = Format(FOMallProper, "#,###,###.##")
                dtMain.Rows(92).Item(DateHeader) = Format(FOMallSupermarket, "#,###,###.##")
                dtMain.Rows(93).Item(DateHeader) = Format(FOOffices, "#,###,###.##")
                dtMain.Rows(94).Item(DateHeader) = Format(FOPublicMarket, "#,###,###.##")
                dtMain.Rows(95).Item(DateHeader) = Format(FOResidential, "#,###,###.##")
                dtMain.Rows(96).Item(DateHeader) = Format(FOResort, "#,###,###.##")
                dtMain.Rows(97).Item(DateHeader) = Format(FOSchool, "#,###,###.##")
                dtMain.Rows(98).Item(DateHeader) = Format(FOSupermarket, "#,###,###.##")
                dtMain.Rows(99).Item(DateHeader) = Format(FOTerminal, "#,###,###.##")
                dtMain.Rows(100).Item(DateHeader) = Format(TotalFOSalesperLocationType, "#,###,###.##")
                dtMain.Rows(101).Item(DateHeader) = ""
                dtMain.Rows(102).Item(DateHeader) = ""

                dtMain.Rows(103).Item(DateHeader) = Format(TotalAirport, "#,###,###.##")
                dtMain.Rows(104).Item(DateHeader) = Format(TotalAmusementPark, "#,###,###.##")
                dtMain.Rows(105).Item(DateHeader) = Format(TotalCommercial, "#,###,###.##")
                dtMain.Rows(106).Item(DateHeader) = Format(TotalChurch, "#,###,###.##")
                dtMain.Rows(107).Item(DateHeader) = Format(TotalCinema, "#,###,###.##")
                dtMain.Rows(108).Item(DateHeader) = Format(TotalEntertainment, "#,###,###.##")
                dtMain.Rows(109).Item(DateHeader) = Format(TotalFoodCourt, "#,###,###.##")
                dtMain.Rows(110).Item(DateHeader) = Format(TotalGasolineStation, "#,###,###.##")
                dtMain.Rows(111).Item(DateHeader) = Format(TotalHospital, "#,###,###.##")
                dtMain.Rows(112).Item(DateHeader) = Format(TotalMall, "#,###,###.##")
                dtMain.Rows(113).Item(DateHeader) = Format(TotalMallCinema, "#,###,###.##")
                dtMain.Rows(114).Item(DateHeader) = Format(TotalMallEntertainment, "#,###,###.##")
                dtMain.Rows(115).Item(DateHeader) = Format(TotalMallFoodcourt, "#,###,###.##")
                dtMain.Rows(116).Item(DateHeader) = Format(TotalMallProper, "#,###,###.##")
                dtMain.Rows(117).Item(DateHeader) = Format(TotalMallSupermarket, "#,###,###.##")
                dtMain.Rows(118).Item(DateHeader) = Format(TotalOffices, "#,###,###.##")
                dtMain.Rows(119).Item(DateHeader) = Format(TotalPublicMarket, "#,###,###.##")
                dtMain.Rows(120).Item(DateHeader) = Format(TotalResidential, "#,###,###.##")
                dtMain.Rows(121).Item(DateHeader) = Format(TotalResort, "#,###,###.##")
                dtMain.Rows(122).Item(DateHeader) = Format(TotalSchool, "#,###,###.##")
                dtMain.Rows(123).Item(DateHeader) = Format(TotalSupermarket, "#,###,###.##")
                dtMain.Rows(124).Item(DateHeader) = Format(TotalTerminal, "#,###,###.##")
                dtMain.Rows(125).Item(DateHeader) = Format(TotalCOSalesperLocationType + TotalFOSalesperLocationType, "#,###,###.##")

                DateFrom = DateFrom.AddDays(7)
                DateTo = DateTo.AddDays(7)

                cinco.DateFrom = CStr2(DateFrom)
                cinco.DateTo = CStr2(DateTo)

                SelectedWeekNo += 1
            Next

            pOut = New JsonDictionary
            pOut.Add("SalesperOwnership", dtSalesperOwnership)
            pOut.Add("DeliverySalesperOwnershipDelivery", dtDeliverySalesperOwnershipDelivery)
            pOut.Add("TakeOutSalesperOwnershipTakeOut", dtTakeOutSalesperOwnershipTakeOut)
            pOut.Add("SalesperRegion", dtSalesperRegion)
            pOut.Add("SalesperLocationType", dtSalesperLocationType)
            pOut.Add("MainTable", dtMain)

            Return pOut.Serialize
        End Function

        Function GetFourWeeklyAverageDailySalesReports() As String
            Dim cinco As New CincoCorporation
            Dim dtSalesperOwnership As DataTable
            Dim dtDeliverySalesperOwnershipDelivery As DataTable
            Dim dtTakeOutSalesperOwnershipTakeOut As DataTable
            Dim dtSalesperRegion As DataTable
            Dim dtSalesperLocationType As DataTable
            Dim pOut As JsonDictionary
            Dim dtMain As New DataTable
            Dim DateFrom As Date
            Dim DateTo As Date
            Dim SelectedWeek As String
            Dim SelectedWeekNo As Integer
            Dim dr As DataRow
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))            
#End If
            SelectedWeek = CStr2(Common.WebRequest.Data("SelectedWeek"))
            SelectedWeekNo = CInt2(Common.WebRequest.Data("WeekNo"))
            DateFrom = CDate(Common.WebRequest.Data("dateFrom"))
            DateTo = CDate(Common.WebRequest.Data("dateTo"))

            dtMain = CreateTableHeader("ADS")

            Dim DateHeaderFrom As String
            Dim DateHeaderTo As String
            Dim DateHeader As String

            For i As Integer = 0 To 3

                dtSalesperOwnership = cinco.GetSalesperOwnership
                dtDeliverySalesperOwnershipDelivery = cinco.GetSalesperOwnershipDelivery
                dtTakeOutSalesperOwnershipTakeOut = cinco.GetSalesperOwnershipTakeOut
                dtSalesperRegion = cinco.GetSalesperRegion
                dtSalesperLocationType = cinco.GetSalesperLocationType

                Dim SalesperOwnershipCO As Double = 0
                Dim SalesperOwnershipFO As Double = 0
                Dim TotalSalesperOwnership As Double = 0

                DateHeaderFrom = DateFrom.ToString("MMM d")
                DateHeaderTo = DateTo.ToString("MMM d")
                DateHeader = DateHeaderFrom & "-" & DateHeaderTo
                dtMain.Columns.Add(DateHeader)

                dr = dtMain.NewRow

                For j As Integer = 0 To dtSalesperOwnership.Rows.Count - 1
                    If CBool2(dtSalesperOwnership.Rows(j).Item("FromFranchisee")) Then
                        SalesperOwnershipFO = CDec2(dtSalesperOwnership.Rows(j).Item("GrossSales")) / 7
                    Else
                        SalesperOwnershipCO = CDec2(dtSalesperOwnership.Rows(j).Item("GrossSales")) / 7
                    End If

                    TotalSalesperOwnership += CDec2(dtSalesperOwnership.Rows(j).Item("GrossSales")) / 7
                Next

                dtMain.Rows(0).Item(DateHeader) = "Week " & SelectedWeekNo
                dtMain.Rows(1).Item(DateHeader) = Format(SalesperOwnershipCO, "#,###,###.##")
                dtMain.Rows(2).Item(DateHeader) = Format(SalesperOwnershipFO, "#,###,###.##")
                dtMain.Rows(3).Item(DateHeader) = Format(TotalSalesperOwnership, "#,###,###.##")
                dtMain.Rows(4).Item(DateHeader) = ""
                dtMain.Rows(5).Item(DateHeader) = ""

                Dim DeliverySalesperOwnershipCO As Double = 0
                Dim DeliverySalesperOwnershipFO As Double = 0
                Dim TotalDeliverySalesperOwnership As Double = 0

                For j As Integer = 0 To dtDeliverySalesperOwnershipDelivery.Rows.Count - 1
                    If CBool2(dtDeliverySalesperOwnershipDelivery.Rows(j).Item("FromFranchisee")) Then
                        DeliverySalesperOwnershipFO = CDec2(dtDeliverySalesperOwnershipDelivery.Rows(j).Item("GrossSales")) / 7
                    Else
                        DeliverySalesperOwnershipCO = CDec2(dtDeliverySalesperOwnershipDelivery.Rows(j).Item("GrossSales")) / 7
                    End If

                    TotalDeliverySalesperOwnership += CDec2(dtDeliverySalesperOwnershipDelivery.Rows(j).Item("GrossSales")) / 7
                Next

                dtMain.Rows(6).Item(DateHeader) = Format(DeliverySalesperOwnershipCO, "#,###,###.##")
                dtMain.Rows(7).Item(DateHeader) = Format(DeliverySalesperOwnershipFO, "#,###,###.##")
                dtMain.Rows(8).Item(DateHeader) = Format(TotalDeliverySalesperOwnership, "#,###,###.##")
                dtMain.Rows(9).Item(DateHeader) = ""
                dtMain.Rows(10).Item(DateHeader) = ""

                Dim TakeOutSalesperOwnershipCO As Double = 0
                Dim TakeOutSalesperOwnershipFO As Double = 0
                Dim TotalTakeOutSalesperOwnership As Double = 0

                For j As Integer = 0 To dtTakeOutSalesperOwnershipTakeOut.Rows.Count - 1
                    If CBool2(dtTakeOutSalesperOwnershipTakeOut.Rows(j).Item("FromFranchisee")) Then
                        TakeOutSalesperOwnershipFO = CDec2(dtTakeOutSalesperOwnershipTakeOut.Rows(j).Item("GrossSales")) / 7
                    Else
                        TakeOutSalesperOwnershipCO = CDec2(dtTakeOutSalesperOwnershipTakeOut.Rows(j).Item("GrossSales")) / 7
                    End If

                    TotalTakeOutSalesperOwnership += CDec2(dtTakeOutSalesperOwnershipTakeOut.Rows(j).Item("GrossSales")) / 7
                Next

                dtMain.Rows(11).Item(DateHeader) = Format(TakeOutSalesperOwnershipCO, "#,###,###.##")
                dtMain.Rows(12).Item(DateHeader) = Format(TakeOutSalesperOwnershipFO, "#,###,###.##")
                dtMain.Rows(13).Item(DateHeader) = Format(TotalTakeOutSalesperOwnership, "#,###,###.##")
                dtMain.Rows(14).Item(DateHeader) = ""
                dtMain.Rows(15).Item(DateHeader) = ""

                Dim COGMAN As Double = 0
                Dim COGMAS As Double = 0
                Dim CONorthLuzon As Double = 0
                Dim COSouthLuzon As Double = 0
                Dim COTotalLuzon As Double = 0
                Dim COVisayas As Double = 0
                Dim COMindanao As Double = 0
                Dim COTotalVisMin As Double = 0

                Dim FOGMAN As Double = 0
                Dim FOGMAS As Double = 0
                Dim FONorthLuzon As Double = 0
                Dim FOSouthLuzon As Double = 0
                Dim FOTotalLuzon As Double = 0
                Dim FOVisayas As Double = 0
                Dim Cebu As Double = 0
                Dim FOMindanao As Double = 0
                Dim FOTotalVisMin As Double = 0

                For j As Integer = 0 To dtSalesperRegion.Rows.Count - 1
                    Select Case CStr2(dtSalesperRegion.Rows(j).Item("GroupName"))
                        Case "CO - GMAN"
                            COGMAN = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / 7
                        Case "CO - GMAS"
                            COGMAS = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / 7
                        Case "CO - North Luzon"
                            CONorthLuzon = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / 7
                        Case "CO - South Luzon"
                            COSouthLuzon = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / 7
                        Case "CO - Visayas"
                            COVisayas = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / 7
                        Case "CO - Mindanao"
                            COMindanao = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / 7
                        Case "FO - GMAN"
                            FOGMAN = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / 7
                        Case "FO - GMAS"
                            FOGMAS = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / 7
                        Case "FO - North Luzon"
                            FONorthLuzon = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / 7
                        Case "FO - South Luzon"
                            FOSouthLuzon = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / 7
                        Case "FO - Visayas"
                            FOVisayas = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / 7
                        Case "Cebu"
                            Cebu = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / 7
                        Case "Cebu"
                            COGMAN = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / 7
                        Case "FO - Mindanao"
                            FOMindanao = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / 7
                    End Select
                Next

                COTotalLuzon = COGMAN + COGMAS + CONorthLuzon + COSouthLuzon
                COTotalVisMin = COVisayas + COMindanao

                FOTotalLuzon = FOGMAN + FOGMAS + FONorthLuzon + FOSouthLuzon
                FOTotalVisMin = FOVisayas + Cebu + FOMindanao

                dtMain.Rows(16).Item(DateHeader) = Format(COGMAN, "#,###,###.##")
                dtMain.Rows(17).Item(DateHeader) = Format(COGMAS, "#,###,###.##")
                dtMain.Rows(18).Item(DateHeader) = Format(CONorthLuzon, "#,###,###.##")
                dtMain.Rows(19).Item(DateHeader) = Format(COSouthLuzon, "#,###,###.##")
                dtMain.Rows(20).Item(DateHeader) = Format(COTotalLuzon, "#,###,###.##")
                dtMain.Rows(21).Item(DateHeader) = ""
                dtMain.Rows(22).Item(DateHeader) = Format(COVisayas, "#,###,###.##")
                dtMain.Rows(23).Item(DateHeader) = ""
                dtMain.Rows(24).Item(DateHeader) = Format(COMindanao, "#,###,###.##")
                dtMain.Rows(25).Item(DateHeader) = Format(COTotalVisMin, "#,###,###.##")
                dtMain.Rows(26).Item(DateHeader) = ""
                dtMain.Rows(27).Item(DateHeader) = ""
                dtMain.Rows(28).Item(DateHeader) = Format(FOGMAN, "#,###,###.##")
                dtMain.Rows(29).Item(DateHeader) = Format(FOGMAS, "#,###,###.##")
                dtMain.Rows(30).Item(DateHeader) = Format(FONorthLuzon, "#,###,###.##")
                dtMain.Rows(31).Item(DateHeader) = Format(FOSouthLuzon, "#,###,###.##")
                dtMain.Rows(32).Item(DateHeader) = Format(FOTotalLuzon, "#,###,###.##")
                dtMain.Rows(33).Item(DateHeader) = ""
                dtMain.Rows(34).Item(DateHeader) = Format(FOVisayas, "#,###,###.##")
                dtMain.Rows(35).Item(DateHeader) = Format(Cebu, "#,###,###.##")
                dtMain.Rows(36).Item(DateHeader) = Format(FOMindanao, "#,###,###.##")
                dtMain.Rows(37).Item(DateHeader) = Format(FOTotalVisMin, "#,###,###.##")
                dtMain.Rows(38).Item(DateHeader) = ""
                dtMain.Rows(39).Item(DateHeader) = ""
                dtMain.Rows(40).Item(DateHeader) = Format(COGMAN + FOGMAN, "#,###,###.##")
                dtMain.Rows(41).Item(DateHeader) = Format(COGMAS + FOGMAS, "#,###,###.##")
                dtMain.Rows(42).Item(DateHeader) = Format(CONorthLuzon + FONorthLuzon, "#,###,###.##")
                dtMain.Rows(43).Item(DateHeader) = Format(COSouthLuzon + FOSouthLuzon, "#,###,###.##")
                dtMain.Rows(44).Item(DateHeader) = Format(COTotalLuzon + FOTotalLuzon, "#,###,###.##")
                dtMain.Rows(45).Item(DateHeader) = ""
                dtMain.Rows(46).Item(DateHeader) = Format(COVisayas + FOVisayas, "#,###,###.##")
                dtMain.Rows(47).Item(DateHeader) = Format(Cebu, "#,###,###.##")
                dtMain.Rows(48).Item(DateHeader) = Format(COMindanao + FOMindanao, "#,###,###.##")
                dtMain.Rows(49).Item(DateHeader) = Format(COTotalVisMin + FOTotalVisMin, "#,###,###.##")
                dtMain.Rows(50).Item(DateHeader) = Format(COTotalLuzon + FOTotalLuzon + COTotalVisMin + FOTotalVisMin, "#,###,###.##")

                dtMain.Rows(51).Item(DateHeader) = ""
                dtMain.Rows(52).Item(DateHeader) = ""

                Dim COAirport As Double = 0
                Dim COAmusementPark As Double = 0
                Dim COChurch As Double = 0
                Dim COCinema As Double = 0
                Dim COCommercial As Double = 0
                Dim COEntertainment As Double = 0
                Dim COFoodCourt As Double = 0
                Dim COGasolineStation As Double = 0
                Dim COHospital As Double = 0
                Dim COMall As Double = 0
                Dim COMallCinema As Double = 0
                Dim COMallEntertainment As Double = 0
                Dim COMallFoodcourt As Double = 0
                Dim COMallProper As Double = 0
                Dim COMallSupermarket As Double = 0
                Dim COOffices As Double = 0
                Dim COPublicMarket As Double = 0
                Dim COResidential As Double = 0
                Dim COResort As Double = 0
                Dim COSchool As Double = 0
                Dim COSupermarket As Double = 0
                Dim COTerminal As Double = 0
                Dim TotalCOSalesperLocationType As Double = 0

                Dim FOAirport As Double = 0
                Dim FOAmusementPark As Double = 0
                Dim FOChurch As Double = 0
                Dim FOCinema As Double = 0
                Dim FOCommercial As Double = 0
                Dim FOEntertainment As Double = 0
                Dim FOFoodCourt As Double = 0
                Dim FOGasolineStation As Double = 0
                Dim FOHospital As Double = 0
                Dim FOMall As Double = 0
                Dim FOMallCinema As Double = 0
                Dim FOMallEntertainment As Double = 0
                Dim FOMallFoodcourt As Double = 0
                Dim FOMallProper As Double = 0
                Dim FOMallSupermarket As Double = 0
                Dim FOOffices As Double = 0
                Dim FOPublicMarket As Double = 0
                Dim FOResidential As Double = 0
                Dim FOResort As Double = 0
                Dim FOSchool As Double = 0
                Dim FOSupermarket As Double = 0
                Dim FOTerminal As Double = 0
                Dim TotalFOSalesperLocationType As Double = 0

                Dim TotalAirport As Double = 0
                Dim TotalAmusementPark As Double = 0
                Dim TotalChurch As Double = 0
                Dim TotalCinema As Double = 0
                Dim TotalCommercial As Double = 0
                Dim TotalEntertainment As Double = 0
                Dim TotalFoodCourt As Double = 0
                Dim TotalGasolineStation As Double = 0
                Dim TotalHospital As Double = 0
                Dim TotalMall As Double = 0
                Dim TotalMallCinema As Double = 0
                Dim TotalMallEntertainment As Double = 0
                Dim TotalMallFoodcourt As Double = 0
                Dim TotalMallProper As Double = 0
                Dim TotalMallSupermarket As Double = 0
                Dim TotalOffices As Double = 0
                Dim TotalPublicMarket As Double = 0
                Dim TotalResidential As Double = 0
                Dim TotalResort As Double = 0
                Dim TotalSchool As Double = 0
                Dim TotalSupermarket As Double = 0
                Dim TotalTerminal As Double = 0
                Dim TotalSalesperLocationType As Double = 0

                For j As Integer = 0 To dtSalesperLocationType.Rows.Count - 1
                    Select Case CStr2(dtSalesperLocationType.Rows(j).Item("StoreLocation"))
                        Case "Airport"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOAirport = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalFOSalesperLocationType += FOAirport
                            Else
                                COAirport = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalCOSalesperLocationType += COAirport
                            End If
                            TotalAirport += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                        Case "Amusement Park"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOAmusementPark = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalFOSalesperLocationType += FOAmusementPark
                            Else
                                COAmusementPark = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalCOSalesperLocationType += COAmusementPark
                            End If
                            TotalAmusementPark += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                        Case "Commercial"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOCommercial = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalFOSalesperLocationType += FOCommercial
                            Else
                                COCommercial = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalCOSalesperLocationType += COCommercial
                            End If
                            TotalCommercial += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                        Case "Church"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOChurch = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalFOSalesperLocationType += FOChurch
                            Else
                                COChurch = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalCOSalesperLocationType += COChurch
                            End If
                            TotalChurch += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                        Case "Cinema"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOCinema = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalFOSalesperLocationType += FOCinema
                            Else
                                COCinema = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalCOSalesperLocationType += COCinema
                            End If
                            TotalCinema += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                        Case "Entertainment"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOEntertainment = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalFOSalesperLocationType += FOEntertainment
                            Else
                                COEntertainment = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalCOSalesperLocationType += COEntertainment
                            End If
                            TotalEntertainment += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                        Case "Food Court"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOFoodCourt = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalFOSalesperLocationType += FOFoodCourt
                            Else
                                COFoodCourt = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalCOSalesperLocationType += COFoodCourt
                            End If
                            TotalFoodCourt += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                        Case "Gasoline Station"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOGasolineStation = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalFOSalesperLocationType += FOGasolineStation
                            Else
                                COGasolineStation = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalCOSalesperLocationType += COGasolineStation
                            End If
                            TotalGasolineStation += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                        Case "Hospital"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOHospital = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalFOSalesperLocationType += FOHospital
                            Else
                                COHospital = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalCOSalesperLocationType += COHospital
                            End If
                            TotalHospital += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                        Case "Mall"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMall = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalFOSalesperLocationType += FOMall
                            Else
                                COMall = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalCOSalesperLocationType += COMall
                            End If
                            TotalMall += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                        Case "Mall-Cinema"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMallCinema = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalFOSalesperLocationType += FOMallCinema
                            Else
                                COMallCinema = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalCOSalesperLocationType += COMallCinema
                            End If
                            TotalMallCinema += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                        Case "Mall-Entertainment"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMallEntertainment = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalFOSalesperLocationType += FOMallEntertainment
                            Else
                                COMallEntertainment = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalCOSalesperLocationType += COMallEntertainment
                            End If
                            TotalMallEntertainment += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                        Case "Mall-Foodcourt"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMallFoodcourt = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalFOSalesperLocationType += FOMallFoodcourt
                            Else
                                COMallFoodcourt = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalCOSalesperLocationType += COMallFoodcourt
                            End If
                            TotalMallFoodcourt += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                        Case "Mall-Proper"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMallProper = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalFOSalesperLocationType += FOMallProper
                            Else
                                COMallProper = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalCOSalesperLocationType += COMallProper
                            End If
                            TotalMallProper += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                        Case "Mall-Supermarket"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMallSupermarket = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalFOSalesperLocationType += FOMallSupermarket
                            Else
                                COMallSupermarket = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalCOSalesperLocationType += COMallSupermarket
                            End If
                            TotalMallSupermarket += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                        Case "Offices"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOOffices = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalFOSalesperLocationType += FOOffices
                            Else
                                COOffices = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalCOSalesperLocationType += COOffices
                            End If
                            TotalOffices += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                        Case "PublicMarket"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOPublicMarket = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalFOSalesperLocationType += FOPublicMarket
                            Else
                                COPublicMarket = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalCOSalesperLocationType += COPublicMarket
                            End If
                            TotalPublicMarket += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                        Case "Residential"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOResidential = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalFOSalesperLocationType += FOResidential
                            Else
                                COResidential = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalCOSalesperLocationType += COResidential
                            End If
                            TotalResidential += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                        Case "Resort"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOResort = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalFOSalesperLocationType += FOResort
                            Else
                                COResort = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalCOSalesperLocationType += COResort
                            End If
                            TotalResort += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                        Case "School"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOSchool = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalFOSalesperLocationType += FOSchool
                            Else
                                COSchool = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalCOSalesperLocationType += COSchool
                            End If
                            TotalSchool += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                        Case "Supermarket"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOSupermarket = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalFOSalesperLocationType += FOSupermarket
                            Else
                                COSupermarket = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalCOSalesperLocationType += COSupermarket
                            End If
                            TotalSupermarket += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                        Case "Terminal"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOTerminal = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalFOSalesperLocationType += FOTerminal
                            Else
                                COTerminal = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                                TotalCOSalesperLocationType += COTerminal
                            End If
                            TotalTerminal += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / 7
                    End Select
                Next

                dtMain.Rows(53).Item(DateHeader) = Format(COAirport, "#,###,###.##")
                dtMain.Rows(54).Item(DateHeader) = Format(COAmusementPark, "#,###,###.##")
                dtMain.Rows(55).Item(DateHeader) = Format(COCommercial, "#,###,###.##")
                dtMain.Rows(56).Item(DateHeader) = Format(COChurch, "#,###,###.##")
                dtMain.Rows(57).Item(DateHeader) = Format(COCinema, "#,###,###.##")
                dtMain.Rows(58).Item(DateHeader) = Format(COEntertainment, "#,###,###.##")
                dtMain.Rows(59).Item(DateHeader) = Format(COFoodCourt, "#,###,###.##")
                dtMain.Rows(60).Item(DateHeader) = Format(COGasolineStation, "#,###,###.##")
                dtMain.Rows(61).Item(DateHeader) = Format(COHospital, "#,###,###.##")
                dtMain.Rows(62).Item(DateHeader) = Format(COMall, "#,###,###.##")
                dtMain.Rows(63).Item(DateHeader) = Format(COMallCinema, "#,###,###.##")
                dtMain.Rows(64).Item(DateHeader) = Format(COMallEntertainment, "#,###,###.##")
                dtMain.Rows(65).Item(DateHeader) = Format(COMallFoodcourt, "#,###,###.##")
                dtMain.Rows(66).Item(DateHeader) = Format(COMallProper, "#,###,###.##")
                dtMain.Rows(67).Item(DateHeader) = Format(COMallSupermarket, "#,###,###.##")
                dtMain.Rows(68).Item(DateHeader) = Format(COOffices, "#,###,###.##")
                dtMain.Rows(69).Item(DateHeader) = Format(COPublicMarket, "#,###,###.##")
                dtMain.Rows(70).Item(DateHeader) = Format(COResidential, "#,###,###.##")
                dtMain.Rows(71).Item(DateHeader) = Format(COResort, "#,###,###.##")
                dtMain.Rows(72).Item(DateHeader) = Format(COSchool, "#,###,###.##")
                dtMain.Rows(73).Item(DateHeader) = Format(COSupermarket, "#,###,###.##")
                dtMain.Rows(74).Item(DateHeader) = Format(COTerminal, "#,###,###.##")
                dtMain.Rows(75).Item(DateHeader) = Format(TotalCOSalesperLocationType, "#,###,###.##")
                dtMain.Rows(76).Item(DateHeader) = ""
                dtMain.Rows(77).Item(DateHeader) = ""

                dtMain.Rows(78).Item(DateHeader) = Format(FOAirport, "#,###,###.##")
                dtMain.Rows(79).Item(DateHeader) = Format(FOAmusementPark, "#,###,###.##")
                dtMain.Rows(80).Item(DateHeader) = Format(FOCommercial, "#,###,###.##")
                dtMain.Rows(81).Item(DateHeader) = Format(FOChurch, "#,###,###.##")
                dtMain.Rows(82).Item(DateHeader) = Format(FOCinema, "#,###,###.##")
                dtMain.Rows(83).Item(DateHeader) = Format(FOEntertainment, "#,###,###.##")
                dtMain.Rows(84).Item(DateHeader) = Format(FOFoodCourt, "#,###,###.##")
                dtMain.Rows(85).Item(DateHeader) = Format(FOGasolineStation, "#,###,###.##")
                dtMain.Rows(86).Item(DateHeader) = Format(FOHospital, "#,###,###.##")
                dtMain.Rows(87).Item(DateHeader) = Format(FOMall, "#,###,###.##")
                dtMain.Rows(88).Item(DateHeader) = Format(FOMallCinema, "#,###,###.##")
                dtMain.Rows(89).Item(DateHeader) = Format(FOMallEntertainment, "#,###,###.##")
                dtMain.Rows(90).Item(DateHeader) = Format(FOMallFoodcourt, "#,###,###.##")
                dtMain.Rows(91).Item(DateHeader) = Format(FOMallProper, "#,###,###.##")
                dtMain.Rows(92).Item(DateHeader) = Format(FOMallSupermarket, "#,###,###.##")
                dtMain.Rows(93).Item(DateHeader) = Format(FOOffices, "#,###,###.##")
                dtMain.Rows(94).Item(DateHeader) = Format(FOPublicMarket, "#,###,###.##")
                dtMain.Rows(95).Item(DateHeader) = Format(FOResidential, "#,###,###.##")
                dtMain.Rows(96).Item(DateHeader) = Format(FOResort, "#,###,###.##")
                dtMain.Rows(97).Item(DateHeader) = Format(FOSchool, "#,###,###.##")
                dtMain.Rows(98).Item(DateHeader) = Format(FOSupermarket, "#,###,###.##")
                dtMain.Rows(99).Item(DateHeader) = Format(FOTerminal, "#,###,###.##")
                dtMain.Rows(100).Item(DateHeader) = Format(TotalFOSalesperLocationType, "#,###,###.##")
                dtMain.Rows(101).Item(DateHeader) = ""
                dtMain.Rows(102).Item(DateHeader) = ""

                dtMain.Rows(103).Item(DateHeader) = Format(TotalAirport, "#,###,###.##")
                dtMain.Rows(104).Item(DateHeader) = Format(TotalAmusementPark, "#,###,###.##")
                dtMain.Rows(105).Item(DateHeader) = Format(TotalCommercial, "#,###,###.##")
                dtMain.Rows(106).Item(DateHeader) = Format(TotalChurch, "#,###,###.##")
                dtMain.Rows(107).Item(DateHeader) = Format(TotalCinema, "#,###,###.##")
                dtMain.Rows(108).Item(DateHeader) = Format(TotalEntertainment, "#,###,###.##")
                dtMain.Rows(109).Item(DateHeader) = Format(TotalFoodCourt, "#,###,###.##")
                dtMain.Rows(110).Item(DateHeader) = Format(TotalGasolineStation, "#,###,###.##")
                dtMain.Rows(111).Item(DateHeader) = Format(TotalHospital, "#,###,###.##")
                dtMain.Rows(112).Item(DateHeader) = Format(TotalMall, "#,###,###.##")
                dtMain.Rows(113).Item(DateHeader) = Format(TotalMallCinema, "#,###,###.##")
                dtMain.Rows(114).Item(DateHeader) = Format(TotalMallEntertainment, "#,###,###.##")
                dtMain.Rows(115).Item(DateHeader) = Format(TotalMallFoodcourt, "#,###,###.##")
                dtMain.Rows(116).Item(DateHeader) = Format(TotalMallProper, "#,###,###.##")
                dtMain.Rows(117).Item(DateHeader) = Format(TotalMallSupermarket, "#,###,###.##")
                dtMain.Rows(118).Item(DateHeader) = Format(TotalOffices, "#,###,###.##")
                dtMain.Rows(119).Item(DateHeader) = Format(TotalPublicMarket, "#,###,###.##")
                dtMain.Rows(120).Item(DateHeader) = Format(TotalResidential, "#,###,###.##")
                dtMain.Rows(121).Item(DateHeader) = Format(TotalResort, "#,###,###.##")
                dtMain.Rows(122).Item(DateHeader) = Format(TotalSchool, "#,###,###.##")
                dtMain.Rows(123).Item(DateHeader) = Format(TotalSupermarket, "#,###,###.##")
                dtMain.Rows(124).Item(DateHeader) = Format(TotalTerminal, "#,###,###.##")
                dtMain.Rows(125).Item(DateHeader) = Format(TotalCOSalesperLocationType + TotalFOSalesperLocationType, "#,###,###.##")

                DateFrom = DateFrom.AddDays(7)
                DateTo = DateTo.AddDays(7)

                cinco.DateFrom = CStr2(DateFrom)
                cinco.DateTo = CStr2(DateTo)

                SelectedWeekNo += 1
            Next

            pOut = New JsonDictionary
            pOut.Add("SalesperOwnership", dtSalesperOwnership)
            pOut.Add("DeliverySalesperOwnershipDelivery", dtDeliverySalesperOwnershipDelivery)
            pOut.Add("TakeOutSalesperOwnershipTakeOut", dtTakeOutSalesperOwnershipTakeOut)
            pOut.Add("SalesperRegion", dtSalesperRegion)
            pOut.Add("SalesperLocationType", dtSalesperLocationType)
            pOut.Add("MainTable", dtMain)

            Return pOut.Serialize
        End Function

        Function GetFourWeeklyTransactionCountReports() As String
            Dim cinco As New CincoCorporation
            Dim dtSalesperOwnership As DataTable
            Dim dtDeliverySalesperOwnershipDelivery As DataTable
            Dim dtTakeOutSalesperOwnershipTakeOut As DataTable
            Dim dtSalesperRegion As DataTable
            Dim dtSalesperLocationType As DataTable
            Dim pOut As JsonDictionary
            Dim dtMain As New DataTable
            Dim DateFrom As Date
            Dim DateTo As Date
            Dim SelectedWeek As String
            Dim SelectedWeekNo As Integer
            Dim dr As DataRow
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))            
#End If
            SelectedWeek = CStr2(Common.WebRequest.Data("SelectedWeek"))
            SelectedWeekNo = CInt2(Common.WebRequest.Data("WeekNo"))
            DateFrom = CDate(Common.WebRequest.Data("dateFrom"))
            DateTo = CDate(Common.WebRequest.Data("dateTo"))

            dtMain = CreateTableHeader("ADS")

            Dim DateHeaderFrom As String
            Dim DateHeaderTo As String
            Dim DateHeader As String

            For i As Integer = 0 To 3

                dtSalesperOwnership = cinco.GetTransactionCountperOwnership
                dtDeliverySalesperOwnershipDelivery = cinco.GetTransactionCountperOwnershipDelivery
                dtTakeOutSalesperOwnershipTakeOut = cinco.GetTransactionCountperOwnershipTakeOut
                dtSalesperRegion = cinco.GetTransactionCountperRegion
                dtSalesperLocationType = cinco.GetTransactionCountperLocationType

                Dim SalesperOwnershipCO As Double = 0
                Dim SalesperOwnershipFO As Double = 0
                Dim TotalSalesperOwnership As Double = 0

                DateHeaderFrom = DateFrom.ToString("MMM d")
                DateHeaderTo = DateTo.ToString("MMM d")
                DateHeader = DateHeaderFrom & "-" & DateHeaderTo
                dtMain.Columns.Add(DateHeader)

                dr = dtMain.NewRow

                For j As Integer = 0 To dtSalesperOwnership.Rows.Count - 1
                    If CBool2(dtSalesperOwnership.Rows(j).Item("FromFranchisee")) Then
                        SalesperOwnershipFO = CDec2(dtSalesperOwnership.Rows(j).Item("TransactionCount"))
                    Else
                        SalesperOwnershipCO = CDec2(dtSalesperOwnership.Rows(j).Item("TransactionCount"))
                    End If

                    TotalSalesperOwnership += CDec2(dtSalesperOwnership.Rows(j).Item("TransactionCount"))
                Next

                dtMain.Rows(0).Item(DateHeader) = "Week " & SelectedWeekNo
                dtMain.Rows(1).Item(DateHeader) = Format(SalesperOwnershipCO, "#,###,###.##")
                dtMain.Rows(2).Item(DateHeader) = Format(SalesperOwnershipFO, "#,###,###.##")
                dtMain.Rows(3).Item(DateHeader) = Format(TotalSalesperOwnership, "#,###,###.##")
                dtMain.Rows(4).Item(DateHeader) = ""
                dtMain.Rows(5).Item(DateHeader) = ""

                Dim DeliverySalesperOwnershipCO As Double = 0
                Dim DeliverySalesperOwnershipFO As Double = 0
                Dim TotalDeliverySalesperOwnership As Double = 0

                For j As Integer = 0 To dtDeliverySalesperOwnershipDelivery.Rows.Count - 1
                    If CBool2(dtDeliverySalesperOwnershipDelivery.Rows(j).Item("FromFranchisee")) Then
                        DeliverySalesperOwnershipFO = CDec2(dtDeliverySalesperOwnershipDelivery.Rows(j).Item("TransactionCount"))
                    Else
                        DeliverySalesperOwnershipCO = CDec2(dtDeliverySalesperOwnershipDelivery.Rows(j).Item("TransactionCount"))
                    End If

                    TotalDeliverySalesperOwnership += CDec2(dtDeliverySalesperOwnershipDelivery.Rows(j).Item("TransactionCount"))
                Next

                dtMain.Rows(6).Item(DateHeader) = Format(DeliverySalesperOwnershipCO, "#,###,###.##")
                dtMain.Rows(7).Item(DateHeader) = Format(DeliverySalesperOwnershipFO, "#,###,###.##")
                dtMain.Rows(8).Item(DateHeader) = Format(TotalDeliverySalesperOwnership, "#,###,###.##")
                dtMain.Rows(9).Item(DateHeader) = ""
                dtMain.Rows(10).Item(DateHeader) = ""

                Dim TakeOutSalesperOwnershipCO As Double = 0
                Dim TakeOutSalesperOwnershipFO As Double = 0
                Dim TotalTakeOutSalesperOwnership As Double = 0

                For j As Integer = 0 To dtTakeOutSalesperOwnershipTakeOut.Rows.Count - 1
                    If CBool2(dtTakeOutSalesperOwnershipTakeOut.Rows(j).Item("FromFranchisee")) Then
                        TakeOutSalesperOwnershipFO = CDec2(dtTakeOutSalesperOwnershipTakeOut.Rows(j).Item("TransactionCount"))
                    Else
                        TakeOutSalesperOwnershipCO = CDec2(dtTakeOutSalesperOwnershipTakeOut.Rows(j).Item("TransactionCount"))
                    End If

                    TotalTakeOutSalesperOwnership += CDec2(dtTakeOutSalesperOwnershipTakeOut.Rows(j).Item("TransactionCount"))
                Next

                dtMain.Rows(11).Item(DateHeader) = Format(TakeOutSalesperOwnershipCO, "#,###,###.##")
                dtMain.Rows(12).Item(DateHeader) = Format(TakeOutSalesperOwnershipFO, "#,###,###.##")
                dtMain.Rows(13).Item(DateHeader) = Format(TotalTakeOutSalesperOwnership, "#,###,###.##")
                dtMain.Rows(14).Item(DateHeader) = ""
                dtMain.Rows(15).Item(DateHeader) = ""

                Dim COGMAN As Double = 0
                Dim COGMAS As Double = 0
                Dim CONorthLuzon As Double = 0
                Dim COSouthLuzon As Double = 0
                Dim COTotalLuzon As Double = 0
                Dim COVisayas As Double = 0
                Dim COMindanao As Double = 0
                Dim COTotalVisMin As Double = 0

                Dim FOGMAN As Double = 0
                Dim FOGMAS As Double = 0
                Dim FONorthLuzon As Double = 0
                Dim FOSouthLuzon As Double = 0
                Dim FOTotalLuzon As Double = 0
                Dim FOVisayas As Double = 0
                Dim Cebu As Double = 0
                Dim FOMindanao As Double = 0
                Dim FOTotalVisMin As Double = 0

                For j As Integer = 0 To dtSalesperRegion.Rows.Count - 1
                    Select Case CStr2(dtSalesperRegion.Rows(j).Item("GroupName"))
                        Case "CO - GMAN"
                            COGMAN = CDec2(dtSalesperRegion.Rows(j).Item("TransactionCount"))
                        Case "CO - GMAS"
                            COGMAS = CDec2(dtSalesperRegion.Rows(j).Item("TransactionCount"))
                        Case "CO - North Luzon"
                            CONorthLuzon = CDec2(dtSalesperRegion.Rows(j).Item("TransactionCount"))
                        Case "CO - South Luzon"
                            COSouthLuzon = CDec2(dtSalesperRegion.Rows(j).Item("TransactionCount"))
                        Case "CO - Visayas"
                            COVisayas = CDec2(dtSalesperRegion.Rows(j).Item("TransactionCount"))
                        Case "CO - Mindanao"
                            COMindanao = CDec2(dtSalesperRegion.Rows(j).Item("TransactionCount"))
                        Case "FO - GMAN"
                            FOGMAN = CDec2(dtSalesperRegion.Rows(j).Item("TransactionCount"))
                        Case "FO - GMAS"
                            FOGMAS = CDec2(dtSalesperRegion.Rows(j).Item("TransactionCount"))
                        Case "FO - North Luzon"
                            FONorthLuzon = CDec2(dtSalesperRegion.Rows(j).Item("TransactionCount"))
                        Case "FO - South Luzon"
                            FOSouthLuzon = CDec2(dtSalesperRegion.Rows(j).Item("TransactionCount"))
                        Case "FO - Visayas"
                            FOVisayas = CDec2(dtSalesperRegion.Rows(j).Item("TransactionCount"))
                        Case "Cebu"
                            Cebu = CDec2(dtSalesperRegion.Rows(j).Item("TransactionCount"))
                        Case "Cebu"
                            COGMAN = CDec2(dtSalesperRegion.Rows(j).Item("TransactionCount"))
                        Case "FO - Mindanao"
                            FOMindanao = CDec2(dtSalesperRegion.Rows(j).Item("TransactionCount"))
                    End Select
                Next

                COTotalLuzon = COGMAN + COGMAS + CONorthLuzon + COSouthLuzon
                COTotalVisMin = COVisayas + COMindanao

                FOTotalLuzon = FOGMAN + FOGMAS + FONorthLuzon + FOSouthLuzon
                FOTotalVisMin = FOVisayas + Cebu + FOMindanao

                dtMain.Rows(16).Item(DateHeader) = Format(COGMAN, "#,###,###.##")
                dtMain.Rows(17).Item(DateHeader) = Format(COGMAS, "#,###,###.##")
                dtMain.Rows(18).Item(DateHeader) = Format(CONorthLuzon, "#,###,###.##")
                dtMain.Rows(19).Item(DateHeader) = Format(COSouthLuzon, "#,###,###.##")
                dtMain.Rows(20).Item(DateHeader) = Format(COTotalLuzon, "#,###,###.##")
                dtMain.Rows(21).Item(DateHeader) = ""
                dtMain.Rows(22).Item(DateHeader) = Format(COVisayas, "#,###,###.##")
                dtMain.Rows(23).Item(DateHeader) = ""
                dtMain.Rows(24).Item(DateHeader) = Format(COMindanao, "#,###,###.##")
                dtMain.Rows(25).Item(DateHeader) = Format(COTotalVisMin, "#,###,###.##")
                dtMain.Rows(26).Item(DateHeader) = ""
                dtMain.Rows(27).Item(DateHeader) = ""
                dtMain.Rows(28).Item(DateHeader) = Format(FOGMAN, "#,###,###.##")
                dtMain.Rows(29).Item(DateHeader) = Format(FOGMAS, "#,###,###.##")
                dtMain.Rows(30).Item(DateHeader) = Format(FONorthLuzon, "#,###,###.##")
                dtMain.Rows(31).Item(DateHeader) = Format(FOSouthLuzon, "#,###,###.##")
                dtMain.Rows(32).Item(DateHeader) = Format(FOTotalLuzon, "#,###,###.##")
                dtMain.Rows(33).Item(DateHeader) = ""
                dtMain.Rows(34).Item(DateHeader) = Format(FOVisayas, "#,###,###.##")
                dtMain.Rows(35).Item(DateHeader) = Format(Cebu, "#,###,###.##")
                dtMain.Rows(36).Item(DateHeader) = Format(FOMindanao, "#,###,###.##")
                dtMain.Rows(37).Item(DateHeader) = Format(FOTotalVisMin, "#,###,###.##")
                dtMain.Rows(38).Item(DateHeader) = ""
                dtMain.Rows(39).Item(DateHeader) = ""
                dtMain.Rows(40).Item(DateHeader) = Format(COGMAN + FOGMAN, "#,###,###.##")
                dtMain.Rows(41).Item(DateHeader) = Format(COGMAS + FOGMAS, "#,###,###.##")
                dtMain.Rows(42).Item(DateHeader) = Format(CONorthLuzon + FONorthLuzon, "#,###,###.##")
                dtMain.Rows(43).Item(DateHeader) = Format(COSouthLuzon + FOSouthLuzon, "#,###,###.##")
                dtMain.Rows(44).Item(DateHeader) = Format(COTotalLuzon + FOTotalLuzon, "#,###,###.##")
                dtMain.Rows(45).Item(DateHeader) = ""
                dtMain.Rows(46).Item(DateHeader) = Format(COVisayas + FOVisayas, "#,###,###.##")
                dtMain.Rows(47).Item(DateHeader) = Format(Cebu, "#,###,###.##")
                dtMain.Rows(48).Item(DateHeader) = Format(COMindanao + FOMindanao, "#,###,###.##")
                dtMain.Rows(49).Item(DateHeader) = Format(COTotalVisMin + FOTotalVisMin, "#,###,###.##")
                dtMain.Rows(50).Item(DateHeader) = Format(COTotalLuzon + FOTotalLuzon + COTotalVisMin + FOTotalVisMin, "#,###,###.##")

                dtMain.Rows(51).Item(DateHeader) = ""
                dtMain.Rows(52).Item(DateHeader) = ""

                Dim COAirport As Double = 0
                Dim COAmusementPark As Double = 0
                Dim COChurch As Double = 0
                Dim COCinema As Double = 0
                Dim COCommercial As Double = 0
                Dim COEntertainment As Double = 0
                Dim COFoodCourt As Double = 0
                Dim COGasolineStation As Double = 0
                Dim COHospital As Double = 0
                Dim COMall As Double = 0
                Dim COMallCinema As Double = 0
                Dim COMallEntertainment As Double = 0
                Dim COMallFoodcourt As Double = 0
                Dim COMallProper As Double = 0
                Dim COMallSupermarket As Double = 0
                Dim COOffices As Double = 0
                Dim COPublicMarket As Double = 0
                Dim COResidential As Double = 0
                Dim COResort As Double = 0
                Dim COSchool As Double = 0
                Dim COSupermarket As Double = 0
                Dim COTerminal As Double = 0
                Dim TotalCOSalesperLocationType As Double = 0

                Dim FOAirport As Double = 0
                Dim FOAmusementPark As Double = 0
                Dim FOChurch As Double = 0
                Dim FOCinema As Double = 0
                Dim FOCommercial As Double = 0
                Dim FOEntertainment As Double = 0
                Dim FOFoodCourt As Double = 0
                Dim FOGasolineStation As Double = 0
                Dim FOHospital As Double = 0
                Dim FOMall As Double = 0
                Dim FOMallCinema As Double = 0
                Dim FOMallEntertainment As Double = 0
                Dim FOMallFoodcourt As Double = 0
                Dim FOMallProper As Double = 0
                Dim FOMallSupermarket As Double = 0
                Dim FOOffices As Double = 0
                Dim FOPublicMarket As Double = 0
                Dim FOResidential As Double = 0
                Dim FOResort As Double = 0
                Dim FOSchool As Double = 0
                Dim FOSupermarket As Double = 0
                Dim FOTerminal As Double = 0
                Dim TotalFOSalesperLocationType As Double = 0

                Dim TotalAirport As Double = 0
                Dim TotalAmusementPark As Double = 0
                Dim TotalChurch As Double = 0
                Dim TotalCinema As Double = 0
                Dim TotalCommercial As Double = 0
                Dim TotalEntertainment As Double = 0
                Dim TotalFoodCourt As Double = 0
                Dim TotalGasolineStation As Double = 0
                Dim TotalHospital As Double = 0
                Dim TotalMall As Double = 0
                Dim TotalMallCinema As Double = 0
                Dim TotalMallEntertainment As Double = 0
                Dim TotalMallFoodcourt As Double = 0
                Dim TotalMallProper As Double = 0
                Dim TotalMallSupermarket As Double = 0
                Dim TotalOffices As Double = 0
                Dim TotalPublicMarket As Double = 0
                Dim TotalResidential As Double = 0
                Dim TotalResort As Double = 0
                Dim TotalSchool As Double = 0
                Dim TotalSupermarket As Double = 0
                Dim TotalTerminal As Double = 0
                Dim TotalSalesperLocationType As Double = 0

                For j As Integer = 0 To dtSalesperLocationType.Rows.Count - 1
                    Select Case CStr2(dtSalesperLocationType.Rows(j).Item("StoreLocation"))
                        Case "Airport"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOAirport = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOAirport
                            Else
                                COAirport = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COAirport
                            End If
                            TotalAirport += CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Amusement Park"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOAmusementPark = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOAmusementPark
                            Else
                                COAmusementPark = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COAmusementPark
                            End If
                            TotalAmusementPark += CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Commercial"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOCommercial = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOCommercial
                            Else
                                COCommercial = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COCommercial
                            End If
                            TotalCommercial += CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Church"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOChurch = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOChurch
                            Else
                                COChurch = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COChurch
                            End If
                            TotalChurch += CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Cinema"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOCinema = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOCinema
                            Else
                                COCinema = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COCinema
                            End If
                            TotalCinema += CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Entertainment"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOEntertainment = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOEntertainment
                            Else
                                COEntertainment = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COEntertainment
                            End If
                            TotalEntertainment += CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Food Court"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOFoodCourt = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOFoodCourt
                            Else
                                COFoodCourt = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COFoodCourt
                            End If
                            TotalFoodCourt += CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Gasoline Station"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOGasolineStation = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOGasolineStation
                            Else
                                COGasolineStation = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COGasolineStation
                            End If
                            TotalGasolineStation += CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Hospital"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOHospital = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOHospital
                            Else
                                COHospital = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COHospital
                            End If
                            TotalHospital += CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Mall"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMall = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOMall
                            Else
                                COMall = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COMall
                            End If
                            TotalMall += CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Mall-Cinema"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMallCinema = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOMallCinema
                            Else
                                COMallCinema = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COMallCinema
                            End If
                            TotalMallCinema += CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Mall-Entertainment"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMallEntertainment = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOMallEntertainment
                            Else
                                COMallEntertainment = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COMallEntertainment
                            End If
                            TotalMallEntertainment += CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Mall-Foodcourt"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMallFoodcourt = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOMallFoodcourt
                            Else
                                COMallFoodcourt = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COMallFoodcourt
                            End If
                            TotalMallFoodcourt += CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Mall-Proper"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMallProper = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOMallProper
                            Else
                                COMallProper = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COMallProper
                            End If
                            TotalMallProper += CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Mall-Supermarket"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMallSupermarket = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOMallSupermarket
                            Else
                                COMallSupermarket = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COMallSupermarket
                            End If
                            TotalMallSupermarket += CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Offices"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOOffices = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOOffices
                            Else
                                COOffices = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COOffices
                            End If
                            TotalOffices += CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                        Case "PublicMarket"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOPublicMarket = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOPublicMarket
                            Else
                                COPublicMarket = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COPublicMarket
                            End If
                            TotalPublicMarket += CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Residential"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOResidential = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOResidential
                            Else
                                COResidential = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COResidential
                            End If
                            TotalResidential += CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Resort"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOResort = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOResort
                            Else
                                COResort = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COResort
                            End If
                            TotalResort += CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                        Case "School"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOSchool = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOSchool
                            Else
                                COSchool = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COSchool
                            End If
                            TotalSchool += CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Supermarket"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOSupermarket = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOSupermarket
                            Else
                                COSupermarket = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COSupermarket
                            End If
                            TotalSupermarket += CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Terminal"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOTerminal = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOTerminal
                            Else
                                COTerminal = CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COTerminal
                            End If
                            TotalTerminal += CDec2(dtSalesperLocationType.Rows(j).Item("TransactionCount"))
                    End Select
                Next

                dtMain.Rows(53).Item(DateHeader) = Format(COAirport, "#,###,###.##")
                dtMain.Rows(54).Item(DateHeader) = Format(COAmusementPark, "#,###,###.##")
                dtMain.Rows(55).Item(DateHeader) = Format(COCommercial, "#,###,###.##")
                dtMain.Rows(56).Item(DateHeader) = Format(COChurch, "#,###,###.##")
                dtMain.Rows(57).Item(DateHeader) = Format(COCinema, "#,###,###.##")
                dtMain.Rows(58).Item(DateHeader) = Format(COEntertainment, "#,###,###.##")
                dtMain.Rows(59).Item(DateHeader) = Format(COFoodCourt, "#,###,###.##")
                dtMain.Rows(60).Item(DateHeader) = Format(COGasolineStation, "#,###,###.##")
                dtMain.Rows(61).Item(DateHeader) = Format(COHospital, "#,###,###.##")
                dtMain.Rows(62).Item(DateHeader) = Format(COMall, "#,###,###.##")
                dtMain.Rows(63).Item(DateHeader) = Format(COMallCinema, "#,###,###.##")
                dtMain.Rows(64).Item(DateHeader) = Format(COMallEntertainment, "#,###,###.##")
                dtMain.Rows(65).Item(DateHeader) = Format(COMallFoodcourt, "#,###,###.##")
                dtMain.Rows(66).Item(DateHeader) = Format(COMallProper, "#,###,###.##")
                dtMain.Rows(67).Item(DateHeader) = Format(COMallSupermarket, "#,###,###.##")
                dtMain.Rows(68).Item(DateHeader) = Format(COOffices, "#,###,###.##")
                dtMain.Rows(69).Item(DateHeader) = Format(COPublicMarket, "#,###,###.##")
                dtMain.Rows(70).Item(DateHeader) = Format(COResidential, "#,###,###.##")
                dtMain.Rows(71).Item(DateHeader) = Format(COResort, "#,###,###.##")
                dtMain.Rows(72).Item(DateHeader) = Format(COSchool, "#,###,###.##")
                dtMain.Rows(73).Item(DateHeader) = Format(COSupermarket, "#,###,###.##")
                dtMain.Rows(74).Item(DateHeader) = Format(COTerminal, "#,###,###.##")
                dtMain.Rows(75).Item(DateHeader) = Format(TotalCOSalesperLocationType, "#,###,###.##")
                dtMain.Rows(76).Item(DateHeader) = ""
                dtMain.Rows(77).Item(DateHeader) = ""

                dtMain.Rows(78).Item(DateHeader) = Format(FOAirport, "#,###,###.##")
                dtMain.Rows(79).Item(DateHeader) = Format(FOAmusementPark, "#,###,###.##")
                dtMain.Rows(80).Item(DateHeader) = Format(FOCommercial, "#,###,###.##")
                dtMain.Rows(81).Item(DateHeader) = Format(FOChurch, "#,###,###.##")
                dtMain.Rows(82).Item(DateHeader) = Format(FOCinema, "#,###,###.##")
                dtMain.Rows(83).Item(DateHeader) = Format(FOEntertainment, "#,###,###.##")
                dtMain.Rows(84).Item(DateHeader) = Format(FOFoodCourt, "#,###,###.##")
                dtMain.Rows(85).Item(DateHeader) = Format(FOGasolineStation, "#,###,###.##")
                dtMain.Rows(86).Item(DateHeader) = Format(FOHospital, "#,###,###.##")
                dtMain.Rows(87).Item(DateHeader) = Format(FOMall, "#,###,###.##")
                dtMain.Rows(88).Item(DateHeader) = Format(FOMallCinema, "#,###,###.##")
                dtMain.Rows(89).Item(DateHeader) = Format(FOMallEntertainment, "#,###,###.##")
                dtMain.Rows(90).Item(DateHeader) = Format(FOMallFoodcourt, "#,###,###.##")
                dtMain.Rows(91).Item(DateHeader) = Format(FOMallProper, "#,###,###.##")
                dtMain.Rows(92).Item(DateHeader) = Format(FOMallSupermarket, "#,###,###.##")
                dtMain.Rows(93).Item(DateHeader) = Format(FOOffices, "#,###,###.##")
                dtMain.Rows(94).Item(DateHeader) = Format(FOPublicMarket, "#,###,###.##")
                dtMain.Rows(95).Item(DateHeader) = Format(FOResidential, "#,###,###.##")
                dtMain.Rows(96).Item(DateHeader) = Format(FOResort, "#,###,###.##")
                dtMain.Rows(97).Item(DateHeader) = Format(FOSchool, "#,###,###.##")
                dtMain.Rows(98).Item(DateHeader) = Format(FOSupermarket, "#,###,###.##")
                dtMain.Rows(99).Item(DateHeader) = Format(FOTerminal, "#,###,###.##")
                dtMain.Rows(100).Item(DateHeader) = Format(TotalFOSalesperLocationType, "#,###,###.##")
                dtMain.Rows(101).Item(DateHeader) = ""
                dtMain.Rows(102).Item(DateHeader) = ""

                dtMain.Rows(103).Item(DateHeader) = Format(TotalAirport, "#,###,###.##")
                dtMain.Rows(104).Item(DateHeader) = Format(TotalAmusementPark, "#,###,###.##")
                dtMain.Rows(105).Item(DateHeader) = Format(TotalCommercial, "#,###,###.##")
                dtMain.Rows(106).Item(DateHeader) = Format(TotalChurch, "#,###,###.##")
                dtMain.Rows(107).Item(DateHeader) = Format(TotalCinema, "#,###,###.##")
                dtMain.Rows(108).Item(DateHeader) = Format(TotalEntertainment, "#,###,###.##")
                dtMain.Rows(109).Item(DateHeader) = Format(TotalFoodCourt, "#,###,###.##")
                dtMain.Rows(110).Item(DateHeader) = Format(TotalGasolineStation, "#,###,###.##")
                dtMain.Rows(111).Item(DateHeader) = Format(TotalHospital, "#,###,###.##")
                dtMain.Rows(112).Item(DateHeader) = Format(TotalMall, "#,###,###.##")
                dtMain.Rows(113).Item(DateHeader) = Format(TotalMallCinema, "#,###,###.##")
                dtMain.Rows(114).Item(DateHeader) = Format(TotalMallEntertainment, "#,###,###.##")
                dtMain.Rows(115).Item(DateHeader) = Format(TotalMallFoodcourt, "#,###,###.##")
                dtMain.Rows(116).Item(DateHeader) = Format(TotalMallProper, "#,###,###.##")
                dtMain.Rows(117).Item(DateHeader) = Format(TotalMallSupermarket, "#,###,###.##")
                dtMain.Rows(118).Item(DateHeader) = Format(TotalOffices, "#,###,###.##")
                dtMain.Rows(119).Item(DateHeader) = Format(TotalPublicMarket, "#,###,###.##")
                dtMain.Rows(120).Item(DateHeader) = Format(TotalResidential, "#,###,###.##")
                dtMain.Rows(121).Item(DateHeader) = Format(TotalResort, "#,###,###.##")
                dtMain.Rows(122).Item(DateHeader) = Format(TotalSchool, "#,###,###.##")
                dtMain.Rows(123).Item(DateHeader) = Format(TotalSupermarket, "#,###,###.##")
                dtMain.Rows(124).Item(DateHeader) = Format(TotalTerminal, "#,###,###.##")
                dtMain.Rows(125).Item(DateHeader) = Format(TotalCOSalesperLocationType + TotalFOSalesperLocationType, "#,###,###.##")

                DateFrom = DateFrom.AddDays(7)
                DateTo = DateTo.AddDays(7)

                cinco.DateFrom = CStr2(DateFrom)
                cinco.DateTo = CStr2(DateTo)

                SelectedWeekNo += 1
            Next

            pOut = New JsonDictionary
            'pOut.Add("SalesperOwnership", dtSalesperOwnership)
            'pOut.Add("DeliverySalesperOwnershipDelivery", dtDeliverySalesperOwnershipDelivery)
            'pOut.Add("TakeOutSalesperOwnershipTakeOut", dtTakeOutSalesperOwnershipTakeOut)
            'pOut.Add("SalesperRegion", dtSalesperRegion)
            'pOut.Add("SalesperLocationType", dtSalesperLocationType)
            pOut.Add("MainTable", dtMain)

            Return pOut.Serialize
        End Function

        Function GetFourWeeklyAverageCheckReports() As String
            Dim cinco As New CincoCorporation
            Dim dtSalesperOwnership As DataTable
            Dim dtDeliverySalesperOwnershipDelivery As DataTable
            Dim dtTakeOutSalesperOwnershipTakeOut As DataTable
            Dim dtSalesperRegion As DataTable
            Dim dtSalesperLocationType As DataTable

            Dim dtTransactionCountperOwnership As DataTable
            Dim dtTransactionCountperOwnershipDelivery As DataTable
            Dim dtTransactionCountperOwnershipTakeOut As DataTable
            Dim dtTransactionCountperRegion As DataTable
            Dim dtTransactionCountperLocationType As DataTable

            Dim pOut As JsonDictionary
            Dim dtMain As New DataTable
            Dim DateFrom As Date
            Dim DateTo As Date
            Dim SelectedWeek As String
            Dim SelectedWeekNo As Integer
            Dim dr As DataRow
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))            
#End If
            SelectedWeek = CStr2(Common.WebRequest.Data("SelectedWeek"))
            SelectedWeekNo = CInt2(Common.WebRequest.Data("WeekNo"))
            DateFrom = CDate(Common.WebRequest.Data("dateFrom"))
            DateTo = CDate(Common.WebRequest.Data("dateTo"))

            dtMain = CreateTableHeader("ADS")

            Dim DateHeaderFrom As String
            Dim DateHeaderTo As String
            Dim DateHeader As String

            For i As Integer = 0 To 3

                dtSalesperOwnership = cinco.GetSalesperOwnership
                dtDeliverySalesperOwnershipDelivery = cinco.GetSalesperOwnershipDelivery
                dtTakeOutSalesperOwnershipTakeOut = cinco.GetSalesperOwnershipTakeOut
                dtSalesperRegion = cinco.GetSalesperRegion
                dtSalesperLocationType = cinco.GetSalesperLocationType

                dtTransactionCountperOwnership = cinco.GetTransactionCountperOwnership
                dtTransactionCountperOwnershipDelivery = cinco.GetTransactionCountperOwnershipDelivery
                dtTransactionCountperOwnershipTakeOut = cinco.GetTransactionCountperOwnershipTakeOut
                dtTransactionCountperRegion = cinco.GetTransactionCountperRegion
                dtTransactionCountperLocationType = cinco.GetTransactionCountperLocationType

                Dim SalesperOwnershipCO As Double = 0
                Dim SalesperOwnershipFO As Double = 0
                Dim TotalSalesperOwnership As Double = 0

                DateHeaderFrom = DateFrom.ToString("MMM d")
                DateHeaderTo = DateTo.ToString("MMM d")
                DateHeader = DateHeaderFrom & "-" & DateHeaderTo
                dtMain.Columns.Add(DateHeader)

                dr = dtMain.NewRow

                For j As Integer = 0 To dtSalesperOwnership.Rows.Count - 1
                    If CBool2(dtSalesperOwnership.Rows(j).Item("FromFranchisee")) Then
                        SalesperOwnershipFO = CDec2(dtSalesperOwnership.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperOwnership.Rows(j).Item("TransactionCount"))
                    Else
                        SalesperOwnershipCO = CDec2(dtSalesperOwnership.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperOwnership.Rows(j).Item("TransactionCount"))
                    End If

                    TotalSalesperOwnership += CDec2(dtSalesperOwnership.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperOwnership.Rows(j).Item("TransactionCount"))
                Next

                dtMain.Rows(0).Item(DateHeader) = "Week " & SelectedWeekNo
                dtMain.Rows(1).Item(DateHeader) = Format(SalesperOwnershipCO, "#,###,###.##")
                dtMain.Rows(2).Item(DateHeader) = Format(SalesperOwnershipFO, "#,###,###.##")
                dtMain.Rows(3).Item(DateHeader) = Format(TotalSalesperOwnership, "#,###,###.##")
                dtMain.Rows(4).Item(DateHeader) = ""
                dtMain.Rows(5).Item(DateHeader) = ""

                Dim DeliverySalesperOwnershipCO As Double = 0
                Dim DeliverySalesperOwnershipFO As Double = 0
                Dim TotalDeliverySalesperOwnership As Double = 0

                For j As Integer = 0 To dtDeliverySalesperOwnershipDelivery.Rows.Count - 1
                    If CBool2(dtDeliverySalesperOwnershipDelivery.Rows(j).Item("FromFranchisee")) Then
                        DeliverySalesperOwnershipFO = CDec2(dtDeliverySalesperOwnershipDelivery.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperOwnershipDelivery.Rows(j).Item("TransactionCount"))
                    Else
                        DeliverySalesperOwnershipCO = CDec2(dtDeliverySalesperOwnershipDelivery.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperOwnershipDelivery.Rows(j).Item("TransactionCount"))
                    End If

                    TotalDeliverySalesperOwnership += CDec2(dtDeliverySalesperOwnershipDelivery.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperOwnershipDelivery.Rows(j).Item("TransactionCount"))
                Next

                dtMain.Rows(6).Item(DateHeader) = Format(DeliverySalesperOwnershipCO, "#,###,###.##")
                dtMain.Rows(7).Item(DateHeader) = Format(DeliverySalesperOwnershipFO, "#,###,###.##")
                dtMain.Rows(8).Item(DateHeader) = Format(TotalDeliverySalesperOwnership, "#,###,###.##")
                dtMain.Rows(9).Item(DateHeader) = ""
                dtMain.Rows(10).Item(DateHeader) = ""

                Dim TakeOutSalesperOwnershipCO As Double = 0
                Dim TakeOutSalesperOwnershipFO As Double = 0
                Dim TotalTakeOutSalesperOwnership As Double = 0

                For j As Integer = 0 To dtTakeOutSalesperOwnershipTakeOut.Rows.Count - 1
                    If CBool2(dtTakeOutSalesperOwnershipTakeOut.Rows(j).Item("FromFranchisee")) Then
                        TakeOutSalesperOwnershipFO = CDec2(dtTakeOutSalesperOwnershipTakeOut.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperOwnershipTakeOut.Rows(j).Item("TransactionCount"))
                    Else
                        TakeOutSalesperOwnershipCO = CDec2(dtTakeOutSalesperOwnershipTakeOut.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperOwnershipTakeOut.Rows(j).Item("TransactionCount"))
                    End If

                    TotalTakeOutSalesperOwnership += CDec2(dtTakeOutSalesperOwnershipTakeOut.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperOwnershipTakeOut.Rows(j).Item("TransactionCount"))
                Next

                dtMain.Rows(11).Item(DateHeader) = Format(TakeOutSalesperOwnershipCO, "#,###,###.##")
                dtMain.Rows(12).Item(DateHeader) = Format(TakeOutSalesperOwnershipFO, "#,###,###.##")
                dtMain.Rows(13).Item(DateHeader) = Format(TotalTakeOutSalesperOwnership, "#,###,###.##")
                dtMain.Rows(14).Item(DateHeader) = ""
                dtMain.Rows(15).Item(DateHeader) = ""

                Dim COGMAN As Double = 0
                Dim COGMAS As Double = 0
                Dim CONorthLuzon As Double = 0
                Dim COSouthLuzon As Double = 0
                Dim COTotalLuzon As Double = 0
                Dim COVisayas As Double = 0
                Dim COMindanao As Double = 0
                Dim COTotalVisMin As Double = 0

                Dim FOGMAN As Double = 0
                Dim FOGMAS As Double = 0
                Dim FONorthLuzon As Double = 0
                Dim FOSouthLuzon As Double = 0
                Dim FOTotalLuzon As Double = 0
                Dim FOVisayas As Double = 0
                Dim Cebu As Double = 0
                Dim FOMindanao As Double = 0
                Dim FOTotalVisMin As Double = 0

                For j As Integer = 0 To dtSalesperRegion.Rows.Count - 1
                    Select Case CStr2(dtSalesperRegion.Rows(j).Item("GroupName"))
                        Case "CO - GMAN"
                            COGMAN = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperRegion.Rows(j).Item("TransactionCount"))
                        Case "CO - GMAS"
                            COGMAS = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperRegion.Rows(j).Item("TransactionCount"))
                        Case "CO - North Luzon"
                            CONorthLuzon = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperRegion.Rows(j).Item("TransactionCount"))
                        Case "CO - South Luzon"
                            COSouthLuzon = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperRegion.Rows(j).Item("TransactionCount"))
                        Case "CO - Visayas"
                            COVisayas = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperRegion.Rows(j).Item("TransactionCount"))
                        Case "CO - Mindanao"
                            COMindanao = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperRegion.Rows(j).Item("TransactionCount"))
                        Case "FO - GMAN"
                            FOGMAN = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperRegion.Rows(j).Item("TransactionCount"))
                        Case "FO - GMAS"
                            FOGMAS = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperRegion.Rows(j).Item("TransactionCount"))
                        Case "FO - North Luzon"
                            FONorthLuzon = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperRegion.Rows(j).Item("TransactionCount"))
                        Case "FO - South Luzon"
                            FOSouthLuzon = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperRegion.Rows(j).Item("TransactionCount"))
                        Case "FO - Visayas"
                            FOVisayas = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperRegion.Rows(j).Item("TransactionCount"))
                        Case "Cebu"
                            Cebu = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperRegion.Rows(j).Item("TransactionCount"))
                        Case "Cebu"
                            COGMAN = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperRegion.Rows(j).Item("TransactionCount"))
                        Case "FO - Mindanao"
                            FOMindanao = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperRegion.Rows(j).Item("TransactionCount"))
                    End Select
                Next

                COTotalLuzon = COGMAN + COGMAS + CONorthLuzon + COSouthLuzon
                COTotalVisMin = COVisayas + COMindanao

                FOTotalLuzon = FOGMAN + FOGMAS + FONorthLuzon + FOSouthLuzon
                FOTotalVisMin = FOVisayas + Cebu + FOMindanao

                dtMain.Rows(16).Item(DateHeader) = Format(COGMAN, "#,###,###.##")
                dtMain.Rows(17).Item(DateHeader) = Format(COGMAS, "#,###,###.##")
                dtMain.Rows(18).Item(DateHeader) = Format(CONorthLuzon, "#,###,###.##")
                dtMain.Rows(19).Item(DateHeader) = Format(COSouthLuzon, "#,###,###.##")
                dtMain.Rows(20).Item(DateHeader) = Format(COTotalLuzon, "#,###,###.##")
                dtMain.Rows(21).Item(DateHeader) = ""
                dtMain.Rows(22).Item(DateHeader) = Format(COVisayas, "#,###,###.##")
                dtMain.Rows(23).Item(DateHeader) = ""
                dtMain.Rows(24).Item(DateHeader) = Format(COMindanao, "#,###,###.##")
                dtMain.Rows(25).Item(DateHeader) = Format(COTotalVisMin, "#,###,###.##")
                dtMain.Rows(26).Item(DateHeader) = ""
                dtMain.Rows(27).Item(DateHeader) = ""
                dtMain.Rows(28).Item(DateHeader) = Format(FOGMAN, "#,###,###.##")
                dtMain.Rows(29).Item(DateHeader) = Format(FOGMAS, "#,###,###.##")
                dtMain.Rows(30).Item(DateHeader) = Format(FONorthLuzon, "#,###,###.##")
                dtMain.Rows(31).Item(DateHeader) = Format(FOSouthLuzon, "#,###,###.##")
                dtMain.Rows(32).Item(DateHeader) = Format(FOTotalLuzon, "#,###,###.##")
                dtMain.Rows(33).Item(DateHeader) = ""
                dtMain.Rows(34).Item(DateHeader) = Format(FOVisayas, "#,###,###.##")
                dtMain.Rows(35).Item(DateHeader) = Format(Cebu, "#,###,###.##")
                dtMain.Rows(36).Item(DateHeader) = Format(FOMindanao, "#,###,###.##")
                dtMain.Rows(37).Item(DateHeader) = Format(FOTotalVisMin, "#,###,###.##")
                dtMain.Rows(38).Item(DateHeader) = ""
                dtMain.Rows(39).Item(DateHeader) = ""
                dtMain.Rows(40).Item(DateHeader) = Format(COGMAN + FOGMAN, "#,###,###.##")
                dtMain.Rows(41).Item(DateHeader) = Format(COGMAS + FOGMAS, "#,###,###.##")
                dtMain.Rows(42).Item(DateHeader) = Format(CONorthLuzon + FONorthLuzon, "#,###,###.##")
                dtMain.Rows(43).Item(DateHeader) = Format(COSouthLuzon + FOSouthLuzon, "#,###,###.##")
                dtMain.Rows(44).Item(DateHeader) = Format(COTotalLuzon + FOTotalLuzon, "#,###,###.##")
                dtMain.Rows(45).Item(DateHeader) = ""
                dtMain.Rows(46).Item(DateHeader) = Format(COVisayas + FOVisayas, "#,###,###.##")
                dtMain.Rows(47).Item(DateHeader) = Format(Cebu, "#,###,###.##")
                dtMain.Rows(48).Item(DateHeader) = Format(COMindanao + FOMindanao, "#,###,###.##")
                dtMain.Rows(49).Item(DateHeader) = Format(COTotalVisMin + FOTotalVisMin, "#,###,###.##")
                dtMain.Rows(50).Item(DateHeader) = Format(COTotalLuzon + FOTotalLuzon + COTotalVisMin + FOTotalVisMin, "#,###,###.##")

                dtMain.Rows(51).Item(DateHeader) = ""
                dtMain.Rows(52).Item(DateHeader) = ""

                Dim COAirport As Double = 0
                Dim COAmusementPark As Double = 0
                Dim COChurch As Double = 0
                Dim COCinema As Double = 0
                Dim COCommercial As Double = 0
                Dim COEntertainment As Double = 0
                Dim COFoodCourt As Double = 0
                Dim COGasolineStation As Double = 0
                Dim COHospital As Double = 0
                Dim COMall As Double = 0
                Dim COMallCinema As Double = 0
                Dim COMallEntertainment As Double = 0
                Dim COMallFoodcourt As Double = 0
                Dim COMallProper As Double = 0
                Dim COMallSupermarket As Double = 0
                Dim COOffices As Double = 0
                Dim COPublicMarket As Double = 0
                Dim COResidential As Double = 0
                Dim COResort As Double = 0
                Dim COSchool As Double = 0
                Dim COSupermarket As Double = 0
                Dim COTerminal As Double = 0
                Dim TotalCOSalesperLocationType As Double = 0

                Dim FOAirport As Double = 0
                Dim FOAmusementPark As Double = 0
                Dim FOChurch As Double = 0
                Dim FOCinema As Double = 0
                Dim FOCommercial As Double = 0
                Dim FOEntertainment As Double = 0
                Dim FOFoodCourt As Double = 0
                Dim FOGasolineStation As Double = 0
                Dim FOHospital As Double = 0
                Dim FOMall As Double = 0
                Dim FOMallCinema As Double = 0
                Dim FOMallEntertainment As Double = 0
                Dim FOMallFoodcourt As Double = 0
                Dim FOMallProper As Double = 0
                Dim FOMallSupermarket As Double = 0
                Dim FOOffices As Double = 0
                Dim FOPublicMarket As Double = 0
                Dim FOResidential As Double = 0
                Dim FOResort As Double = 0
                Dim FOSchool As Double = 0
                Dim FOSupermarket As Double = 0
                Dim FOTerminal As Double = 0
                Dim TotalFOSalesperLocationType As Double = 0

                Dim TotalAirport As Double = 0
                Dim TotalAmusementPark As Double = 0
                Dim TotalChurch As Double = 0
                Dim TotalCinema As Double = 0
                Dim TotalCommercial As Double = 0
                Dim TotalEntertainment As Double = 0
                Dim TotalFoodCourt As Double = 0
                Dim TotalGasolineStation As Double = 0
                Dim TotalHospital As Double = 0
                Dim TotalMall As Double = 0
                Dim TotalMallCinema As Double = 0
                Dim TotalMallEntertainment As Double = 0
                Dim TotalMallFoodcourt As Double = 0
                Dim TotalMallProper As Double = 0
                Dim TotalMallSupermarket As Double = 0
                Dim TotalOffices As Double = 0
                Dim TotalPublicMarket As Double = 0
                Dim TotalResidential As Double = 0
                Dim TotalResort As Double = 0
                Dim TotalSchool As Double = 0
                Dim TotalSupermarket As Double = 0
                Dim TotalTerminal As Double = 0
                Dim TotalSalesperLocationType As Double = 0

                For j As Integer = 0 To dtSalesperLocationType.Rows.Count - 1
                    Select Case CStr2(dtSalesperLocationType.Rows(j).Item("StoreLocation"))
                        Case "Airport"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOAirport = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOAirport
                            Else
                                COAirport = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COAirport
                            End If
                            TotalAirport += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Amusement Park"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOAmusementPark = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOAmusementPark
                            Else
                                COAmusementPark = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COAmusementPark
                            End If
                            TotalAmusementPark += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Commercial"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOCommercial = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOCommercial
                            Else
                                COCommercial = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COCommercial
                            End If
                            TotalCommercial += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Church"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOChurch = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOChurch
                            Else
                                COChurch = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COChurch
                            End If
                            TotalChurch += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Cinema"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOCinema = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOCinema
                            Else
                                COCinema = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COCinema
                            End If
                            TotalCinema += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Entertainment"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOEntertainment = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOEntertainment
                            Else
                                COEntertainment = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COEntertainment
                            End If
                            TotalEntertainment += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Food Court"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOFoodCourt = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOFoodCourt
                            Else
                                COFoodCourt = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COFoodCourt
                            End If
                            TotalFoodCourt += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Gasoline Station"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOGasolineStation = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOGasolineStation
                            Else
                                COGasolineStation = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COGasolineStation
                            End If
                            TotalGasolineStation += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Hospital"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOHospital = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOHospital
                            Else
                                COHospital = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COHospital
                            End If
                            TotalHospital += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Mall"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMall = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOMall
                            Else
                                COMall = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COMall
                            End If
                            TotalMall += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Mall-Cinema"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMallCinema = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOMallCinema
                            Else
                                COMallCinema = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COMallCinema
                            End If
                            TotalMallCinema += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Mall-Entertainment"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMallEntertainment = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOMallEntertainment
                            Else
                                COMallEntertainment = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COMallEntertainment
                            End If
                            TotalMallEntertainment += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Mall-Foodcourt"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMallFoodcourt = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOMallFoodcourt
                            Else
                                COMallFoodcourt = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COMallFoodcourt
                            End If
                            TotalMallFoodcourt += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Mall-Proper"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMallProper = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOMallProper
                            Else
                                COMallProper = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COMallProper
                            End If
                            TotalMallProper += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Mall-Supermarket"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMallSupermarket = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOMallSupermarket
                            Else
                                COMallSupermarket = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COMallSupermarket
                            End If
                            TotalMallSupermarket += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Offices"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOOffices = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOOffices
                            Else
                                COOffices = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COOffices
                            End If
                            TotalOffices += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                        Case "PublicMarket"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOPublicMarket = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOPublicMarket
                            Else
                                COPublicMarket = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COPublicMarket
                            End If
                            TotalPublicMarket += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Residential"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOResidential = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOResidential
                            Else
                                COResidential = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COResidential
                            End If
                            TotalResidential += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Resort"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOResort = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOResort
                            Else
                                COResort = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COResort
                            End If
                            TotalResort += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                        Case "School"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOSchool = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOSchool
                            Else
                                COSchool = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COSchool
                            End If
                            TotalSchool += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Supermarket"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOSupermarket = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOSupermarket
                            Else
                                COSupermarket = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COSupermarket
                            End If
                            TotalSupermarket += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                        Case "Terminal"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOTerminal = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalFOSalesperLocationType += FOTerminal
                            Else
                                COTerminal = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                                TotalCOSalesperLocationType += COTerminal
                            End If
                            TotalTerminal += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales")) / CDec2(dtTransactionCountperLocationType.Rows(j).Item("TransactionCount"))
                    End Select
                Next

                dtMain.Rows(53).Item(DateHeader) = Format(COAirport, "#,###,###.##")
                dtMain.Rows(54).Item(DateHeader) = Format(COAmusementPark, "#,###,###.##")
                dtMain.Rows(55).Item(DateHeader) = Format(COCommercial, "#,###,###.##")
                dtMain.Rows(56).Item(DateHeader) = Format(COChurch, "#,###,###.##")
                dtMain.Rows(57).Item(DateHeader) = Format(COCinema, "#,###,###.##")
                dtMain.Rows(58).Item(DateHeader) = Format(COEntertainment, "#,###,###.##")
                dtMain.Rows(59).Item(DateHeader) = Format(COFoodCourt, "#,###,###.##")
                dtMain.Rows(60).Item(DateHeader) = Format(COGasolineStation, "#,###,###.##")
                dtMain.Rows(61).Item(DateHeader) = Format(COHospital, "#,###,###.##")
                dtMain.Rows(62).Item(DateHeader) = Format(COMall, "#,###,###.##")
                dtMain.Rows(63).Item(DateHeader) = Format(COMallCinema, "#,###,###.##")
                dtMain.Rows(64).Item(DateHeader) = Format(COMallEntertainment, "#,###,###.##")
                dtMain.Rows(65).Item(DateHeader) = Format(COMallFoodcourt, "#,###,###.##")
                dtMain.Rows(66).Item(DateHeader) = Format(COMallProper, "#,###,###.##")
                dtMain.Rows(67).Item(DateHeader) = Format(COMallSupermarket, "#,###,###.##")
                dtMain.Rows(68).Item(DateHeader) = Format(COOffices, "#,###,###.##")
                dtMain.Rows(69).Item(DateHeader) = Format(COPublicMarket, "#,###,###.##")
                dtMain.Rows(70).Item(DateHeader) = Format(COResidential, "#,###,###.##")
                dtMain.Rows(71).Item(DateHeader) = Format(COResort, "#,###,###.##")
                dtMain.Rows(72).Item(DateHeader) = Format(COSchool, "#,###,###.##")
                dtMain.Rows(73).Item(DateHeader) = Format(COSupermarket, "#,###,###.##")
                dtMain.Rows(74).Item(DateHeader) = Format(COTerminal, "#,###,###.##")
                dtMain.Rows(75).Item(DateHeader) = Format(TotalCOSalesperLocationType, "#,###,###.##")
                dtMain.Rows(76).Item(DateHeader) = ""
                dtMain.Rows(77).Item(DateHeader) = ""

                dtMain.Rows(78).Item(DateHeader) = Format(FOAirport, "#,###,###.##")
                dtMain.Rows(79).Item(DateHeader) = Format(FOAmusementPark, "#,###,###.##")
                dtMain.Rows(80).Item(DateHeader) = Format(FOCommercial, "#,###,###.##")
                dtMain.Rows(81).Item(DateHeader) = Format(FOChurch, "#,###,###.##")
                dtMain.Rows(82).Item(DateHeader) = Format(FOCinema, "#,###,###.##")
                dtMain.Rows(83).Item(DateHeader) = Format(FOEntertainment, "#,###,###.##")
                dtMain.Rows(84).Item(DateHeader) = Format(FOFoodCourt, "#,###,###.##")
                dtMain.Rows(85).Item(DateHeader) = Format(FOGasolineStation, "#,###,###.##")
                dtMain.Rows(86).Item(DateHeader) = Format(FOHospital, "#,###,###.##")
                dtMain.Rows(87).Item(DateHeader) = Format(FOMall, "#,###,###.##")
                dtMain.Rows(88).Item(DateHeader) = Format(FOMallCinema, "#,###,###.##")
                dtMain.Rows(89).Item(DateHeader) = Format(FOMallEntertainment, "#,###,###.##")
                dtMain.Rows(90).Item(DateHeader) = Format(FOMallFoodcourt, "#,###,###.##")
                dtMain.Rows(91).Item(DateHeader) = Format(FOMallProper, "#,###,###.##")
                dtMain.Rows(92).Item(DateHeader) = Format(FOMallSupermarket, "#,###,###.##")
                dtMain.Rows(93).Item(DateHeader) = Format(FOOffices, "#,###,###.##")
                dtMain.Rows(94).Item(DateHeader) = Format(FOPublicMarket, "#,###,###.##")
                dtMain.Rows(95).Item(DateHeader) = Format(FOResidential, "#,###,###.##")
                dtMain.Rows(96).Item(DateHeader) = Format(FOResort, "#,###,###.##")
                dtMain.Rows(97).Item(DateHeader) = Format(FOSchool, "#,###,###.##")
                dtMain.Rows(98).Item(DateHeader) = Format(FOSupermarket, "#,###,###.##")
                dtMain.Rows(99).Item(DateHeader) = Format(FOTerminal, "#,###,###.##")
                dtMain.Rows(100).Item(DateHeader) = Format(TotalFOSalesperLocationType, "#,###,###.##")
                dtMain.Rows(101).Item(DateHeader) = ""
                dtMain.Rows(102).Item(DateHeader) = ""

                dtMain.Rows(103).Item(DateHeader) = Format(TotalAirport, "#,###,###.##")
                dtMain.Rows(104).Item(DateHeader) = Format(TotalAmusementPark, "#,###,###.##")
                dtMain.Rows(105).Item(DateHeader) = Format(TotalCommercial, "#,###,###.##")
                dtMain.Rows(106).Item(DateHeader) = Format(TotalChurch, "#,###,###.##")
                dtMain.Rows(107).Item(DateHeader) = Format(TotalCinema, "#,###,###.##")
                dtMain.Rows(108).Item(DateHeader) = Format(TotalEntertainment, "#,###,###.##")
                dtMain.Rows(109).Item(DateHeader) = Format(TotalFoodCourt, "#,###,###.##")
                dtMain.Rows(110).Item(DateHeader) = Format(TotalGasolineStation, "#,###,###.##")
                dtMain.Rows(111).Item(DateHeader) = Format(TotalHospital, "#,###,###.##")
                dtMain.Rows(112).Item(DateHeader) = Format(TotalMall, "#,###,###.##")
                dtMain.Rows(113).Item(DateHeader) = Format(TotalMallCinema, "#,###,###.##")
                dtMain.Rows(114).Item(DateHeader) = Format(TotalMallEntertainment, "#,###,###.##")
                dtMain.Rows(115).Item(DateHeader) = Format(TotalMallFoodcourt, "#,###,###.##")
                dtMain.Rows(116).Item(DateHeader) = Format(TotalMallProper, "#,###,###.##")
                dtMain.Rows(117).Item(DateHeader) = Format(TotalMallSupermarket, "#,###,###.##")
                dtMain.Rows(118).Item(DateHeader) = Format(TotalOffices, "#,###,###.##")
                dtMain.Rows(119).Item(DateHeader) = Format(TotalPublicMarket, "#,###,###.##")
                dtMain.Rows(120).Item(DateHeader) = Format(TotalResidential, "#,###,###.##")
                dtMain.Rows(121).Item(DateHeader) = Format(TotalResort, "#,###,###.##")
                dtMain.Rows(122).Item(DateHeader) = Format(TotalSchool, "#,###,###.##")
                dtMain.Rows(123).Item(DateHeader) = Format(TotalSupermarket, "#,###,###.##")
                dtMain.Rows(124).Item(DateHeader) = Format(TotalTerminal, "#,###,###.##")
                dtMain.Rows(125).Item(DateHeader) = Format(TotalCOSalesperLocationType + TotalFOSalesperLocationType, "#,###,###.##")

                DateFrom = DateFrom.AddDays(7)
                DateTo = DateTo.AddDays(7)

                cinco.DateFrom = CStr2(DateFrom)
                cinco.DateTo = CStr2(DateTo)

                SelectedWeekNo += 1
            Next

            pOut = New JsonDictionary
            pOut.Add("MainTable", dtMain)

            Return pOut.Serialize
        End Function

        Function GetMonthlySalesReports() As String
            Dim cinco As New CincoCorporation
            Dim dtSalesperOwnership As DataTable
            Dim dtDeliverySalesperOwnershipDelivery As DataTable
            Dim dtTakeOutSalesperOwnershipTakeOut As DataTable
            Dim dtSalesperRegion As DataTable
            Dim dtSalesperLocationType As DataTable
            Dim pOut As JsonDictionary
            Dim dtMain As New DataTable
            Dim DateFrom As Date
            Dim DateTo As Date
            Dim SelectedWeek As String
            Dim SelectedWeekNo As Integer
            Dim dr As DataRow
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.InvoiceYear = CInt2(Common.WebRequest.Data("Year"))
#Else
            cinco.InvoiceYear = CInt2(Common.WebRequest.Data("Year"))
#End If
            SelectedWeek = CStr2(Common.WebRequest.Data("SelectedWeek"))
            SelectedWeekNo = CInt2(Common.WebRequest.Data("WeekNo"))
            'DateFrom = CDate(Common.WebRequest.Data("dateFrom"))
            'DateTo = CDate(Common.WebRequest.Data("dateTo"))

            dtMain = CreateTableHeader("Sales")

            Dim DateHeaderFrom As String
            Dim DateHeaderTo As String
            Dim DateHeader As String
            Dim Months() As String = {"JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"}

            For i As Integer = 1 To 12

                cinco.InvoiceMonth = i
                dtSalesperOwnership = cinco.GetMonthlySalesperOwnership
                dtDeliverySalesperOwnershipDelivery = cinco.GetMonthlySalesperOwnershipDelivery
                dtTakeOutSalesperOwnershipTakeOut = cinco.GetMonthlySalesperOwnershipTakeOut
                dtSalesperRegion = cinco.GetMonthlySalesperRegion
                dtSalesperLocationType = cinco.GetMonthlySalesperLocationType

                Dim SalesperOwnershipCO As Double = 0
                Dim SalesperOwnershipFO As Double = 0
                Dim TotalSalesperOwnership As Double = 0

                'DateHeaderFrom = DateFrom.ToString("MMM d")
                'DateHeaderTo = DateTo.ToString("MMM d")
                'DateHeader = DateHeaderFrom & "-" & DateHeaderTo

                DateHeader = Months(i - 1)
                dtMain.Columns.Add(DateHeader)

                dr = dtMain.NewRow

                For j As Integer = 0 To dtSalesperOwnership.Rows.Count - 1
                    If CBool2(dtSalesperOwnership.Rows(j).Item("FromFranchisee")) Then
                        SalesperOwnershipFO = CDec2(dtSalesperOwnership.Rows(j).Item("GrossSales"))
                    Else
                        SalesperOwnershipCO = CDec2(dtSalesperOwnership.Rows(j).Item("GrossSales"))
                    End If

                    TotalSalesperOwnership += CDec2(dtSalesperOwnership.Rows(j).Item("GrossSales"))
                Next

                'dtMain.Rows(0).Item(DateHeader) = Months(i)
                dtMain.Rows(1).Item(DateHeader) = Format(SalesperOwnershipCO, "#,###,###.##")
                dtMain.Rows(2).Item(DateHeader) = Format(SalesperOwnershipFO, "#,###,###.##")
                dtMain.Rows(3).Item(DateHeader) = Format(TotalSalesperOwnership, "#,###,###.##")
                dtMain.Rows(4).Item(DateHeader) = ""
                dtMain.Rows(5).Item(DateHeader) = ""

                Dim DeliverySalesperOwnershipCO As Double = 0
                Dim DeliverySalesperOwnershipFO As Double = 0
                Dim TotalDeliverySalesperOwnership As Double = 0

                For j As Integer = 0 To dtDeliverySalesperOwnershipDelivery.Rows.Count - 1
                    If CBool2(dtDeliverySalesperOwnershipDelivery.Rows(j).Item("FromFranchisee")) Then
                        DeliverySalesperOwnershipFO = CDec2(dtDeliverySalesperOwnershipDelivery.Rows(j).Item("GrossSales"))
                    Else
                        DeliverySalesperOwnershipCO = CDec2(dtDeliverySalesperOwnershipDelivery.Rows(j).Item("GrossSales"))
                    End If

                    TotalDeliverySalesperOwnership += CDec2(dtDeliverySalesperOwnershipDelivery.Rows(j).Item("GrossSales"))
                Next

                dtMain.Rows(6).Item(DateHeader) = Format(DeliverySalesperOwnershipCO, "#,###,###.##")
                dtMain.Rows(7).Item(DateHeader) = Format(DeliverySalesperOwnershipFO, "#,###,###.##")
                dtMain.Rows(8).Item(DateHeader) = Format(TotalDeliverySalesperOwnership, "#,###,###.##")
                dtMain.Rows(9).Item(DateHeader) = ""
                dtMain.Rows(10).Item(DateHeader) = ""

                Dim TakeOutSalesperOwnershipCO As Double = 0
                Dim TakeOutSalesperOwnershipFO As Double = 0
                Dim TotalTakeOutSalesperOwnership As Double = 0

                For j As Integer = 0 To dtTakeOutSalesperOwnershipTakeOut.Rows.Count - 1
                    If CBool2(dtTakeOutSalesperOwnershipTakeOut.Rows(j).Item("FromFranchisee")) Then
                        TakeOutSalesperOwnershipFO = CDec2(dtTakeOutSalesperOwnershipTakeOut.Rows(j).Item("GrossSales"))
                    Else
                        TakeOutSalesperOwnershipCO = CDec2(dtTakeOutSalesperOwnershipTakeOut.Rows(j).Item("GrossSales"))
                    End If

                    TotalTakeOutSalesperOwnership += CDec2(dtTakeOutSalesperOwnershipTakeOut.Rows(j).Item("GrossSales"))
                Next

                dtMain.Rows(11).Item(DateHeader) = Format(TakeOutSalesperOwnershipCO, "#,###,###.##")
                dtMain.Rows(12).Item(DateHeader) = Format(TakeOutSalesperOwnershipFO, "#,###,###.##")
                dtMain.Rows(13).Item(DateHeader) = Format(TotalTakeOutSalesperOwnership, "#,###,###.##")
                dtMain.Rows(14).Item(DateHeader) = ""
                dtMain.Rows(15).Item(DateHeader) = ""

                Dim COGMAN As Double = 0
                Dim COGMAS As Double = 0
                Dim CONorthLuzon As Double = 0
                Dim COSouthLuzon As Double = 0
                Dim COTotalLuzon As Double = 0
                Dim COVisayas As Double = 0
                Dim COMindanao As Double = 0
                Dim COTotalVisMin As Double = 0

                Dim FOGMAN As Double = 0
                Dim FOGMAS As Double = 0
                Dim FONorthLuzon As Double = 0
                Dim FOSouthLuzon As Double = 0
                Dim FOTotalLuzon As Double = 0
                Dim FOVisayas As Double = 0
                Dim Cebu As Double = 0
                Dim FOMindanao As Double = 0
                Dim FOTotalVisMin As Double = 0

                For j As Integer = 0 To dtSalesperRegion.Rows.Count - 1
                    Select Case CStr2(dtSalesperRegion.Rows(j).Item("GroupName"))
                        Case "CO - GMAN"
                            COGMAN = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                        Case "CO - GMAS"
                            COGMAS = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                        Case "CO - North Luzon"
                            CONorthLuzon = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                        Case "CO - South Luzon"
                            COSouthLuzon = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                        Case "CO - Visayas"
                            COVisayas = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                        Case "CO - Mindanao"
                            COMindanao = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                        Case "FO - GMAN"
                            FOGMAN = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                        Case "FO - GMAS"
                            FOGMAS = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                        Case "FO - North Luzon"
                            FONorthLuzon = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                        Case "FO - South Luzon"
                            FOSouthLuzon = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                        Case "FO - Visayas"
                            FOVisayas = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                        Case "Cebu"
                            Cebu = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                        Case "Cebu"
                            COGMAN = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                        Case "FO - Mindanao"
                            FOMindanao = CDec2(dtSalesperRegion.Rows(j).Item("GrossSales"))
                    End Select
                Next

                COTotalLuzon = COGMAN + COGMAS + CONorthLuzon + COSouthLuzon
                COTotalVisMin = COVisayas + COMindanao

                FOTotalLuzon = FOGMAN + FOGMAS + FONorthLuzon + FOSouthLuzon
                FOTotalVisMin = FOVisayas + Cebu + FOMindanao

                dtMain.Rows(16).Item(DateHeader) = Format(COGMAN, "#,###,###.##")
                dtMain.Rows(17).Item(DateHeader) = Format(COGMAS, "#,###,###.##")
                dtMain.Rows(18).Item(DateHeader) = Format(CONorthLuzon, "#,###,###.##")
                dtMain.Rows(19).Item(DateHeader) = Format(COSouthLuzon, "#,###,###.##")
                dtMain.Rows(20).Item(DateHeader) = Format(COTotalLuzon, "#,###,###.##")
                dtMain.Rows(21).Item(DateHeader) = ""
                dtMain.Rows(22).Item(DateHeader) = Format(COVisayas, "#,###,###.##")
                dtMain.Rows(23).Item(DateHeader) = ""
                dtMain.Rows(24).Item(DateHeader) = Format(COMindanao, "#,###,###.##")
                dtMain.Rows(25).Item(DateHeader) = Format(COTotalVisMin, "#,###,###.##")
                dtMain.Rows(26).Item(DateHeader) = ""
                dtMain.Rows(27).Item(DateHeader) = ""
                dtMain.Rows(28).Item(DateHeader) = Format(FOGMAN, "#,###,###.##")
                dtMain.Rows(29).Item(DateHeader) = Format(FOGMAS, "#,###,###.##")
                dtMain.Rows(30).Item(DateHeader) = Format(FONorthLuzon, "#,###,###.##")
                dtMain.Rows(31).Item(DateHeader) = Format(FOSouthLuzon, "#,###,###.##")
                dtMain.Rows(32).Item(DateHeader) = Format(FOTotalLuzon, "#,###,###.##")
                dtMain.Rows(33).Item(DateHeader) = ""
                dtMain.Rows(34).Item(DateHeader) = Format(FOVisayas, "#,###,###.##")
                dtMain.Rows(35).Item(DateHeader) = Format(Cebu, "#,###,###.##")
                dtMain.Rows(36).Item(DateHeader) = Format(FOMindanao, "#,###,###.##")
                dtMain.Rows(37).Item(DateHeader) = Format(FOTotalVisMin, "#,###,###.##")
                dtMain.Rows(38).Item(DateHeader) = ""
                dtMain.Rows(39).Item(DateHeader) = ""
                dtMain.Rows(40).Item(DateHeader) = Format(COGMAN + FOGMAN, "#,###,###.##")
                dtMain.Rows(41).Item(DateHeader) = Format(COGMAS + FOGMAS, "#,###,###.##")
                dtMain.Rows(42).Item(DateHeader) = Format(CONorthLuzon + FONorthLuzon, "#,###,###.##")
                dtMain.Rows(43).Item(DateHeader) = Format(COSouthLuzon + FOSouthLuzon, "#,###,###.##")
                dtMain.Rows(44).Item(DateHeader) = Format(COTotalLuzon + FOTotalLuzon, "#,###,###.##")
                dtMain.Rows(45).Item(DateHeader) = ""
                dtMain.Rows(46).Item(DateHeader) = Format(COVisayas + FOVisayas, "#,###,###.##")
                dtMain.Rows(47).Item(DateHeader) = Format(Cebu, "#,###,###.##")
                dtMain.Rows(48).Item(DateHeader) = Format(COMindanao + FOMindanao, "#,###,###.##")
                dtMain.Rows(49).Item(DateHeader) = Format(COTotalVisMin + FOTotalVisMin, "#,###,###.##")
                dtMain.Rows(50).Item(DateHeader) = Format(COTotalLuzon + FOTotalLuzon + COTotalVisMin + FOTotalVisMin, "#,###,###.##")

                dtMain.Rows(51).Item(DateHeader) = ""
                dtMain.Rows(52).Item(DateHeader) = ""

                Dim COAirport As Double = 0
                Dim COAmusementPark As Double = 0
                Dim COChurch As Double = 0
                Dim COCinema As Double = 0
                Dim COCommercial As Double = 0
                Dim COEntertainment As Double = 0
                Dim COFoodCourt As Double = 0
                Dim COGasolineStation As Double = 0
                Dim COHospital As Double = 0
                Dim COMall As Double = 0
                Dim COMallCinema As Double = 0
                Dim COMallEntertainment As Double = 0
                Dim COMallFoodcourt As Double = 0
                Dim COMallProper As Double = 0
                Dim COMallSupermarket As Double = 0
                Dim COOffices As Double = 0
                Dim COPublicMarket As Double = 0
                Dim COResidential As Double = 0
                Dim COResort As Double = 0
                Dim COSchool As Double = 0
                Dim COSupermarket As Double = 0
                Dim COTerminal As Double = 0
                Dim TotalCOSalesperLocationType As Double = 0

                Dim FOAirport As Double = 0
                Dim FOAmusementPark As Double = 0
                Dim FOChurch As Double = 0
                Dim FOCinema As Double = 0
                Dim FOCommercial As Double = 0
                Dim FOEntertainment As Double = 0
                Dim FOFoodCourt As Double = 0
                Dim FOGasolineStation As Double = 0
                Dim FOHospital As Double = 0
                Dim FOMall As Double = 0
                Dim FOMallCinema As Double = 0
                Dim FOMallEntertainment As Double = 0
                Dim FOMallFoodcourt As Double = 0
                Dim FOMallProper As Double = 0
                Dim FOMallSupermarket As Double = 0
                Dim FOOffices As Double = 0
                Dim FOPublicMarket As Double = 0
                Dim FOResidential As Double = 0
                Dim FOResort As Double = 0
                Dim FOSchool As Double = 0
                Dim FOSupermarket As Double = 0
                Dim FOTerminal As Double = 0
                Dim TotalFOSalesperLocationType As Double = 0

                Dim TotalAirport As Double = 0
                Dim TotalAmusementPark As Double = 0
                Dim TotalChurch As Double = 0
                Dim TotalCinema As Double = 0
                Dim TotalCommercial As Double = 0
                Dim TotalEntertainment As Double = 0
                Dim TotalFoodCourt As Double = 0
                Dim TotalGasolineStation As Double = 0
                Dim TotalHospital As Double = 0
                Dim TotalMall As Double = 0
                Dim TotalMallCinema As Double = 0
                Dim TotalMallEntertainment As Double = 0
                Dim TotalMallFoodcourt As Double = 0
                Dim TotalMallProper As Double = 0
                Dim TotalMallSupermarket As Double = 0
                Dim TotalOffices As Double = 0
                Dim TotalPublicMarket As Double = 0
                Dim TotalResidential As Double = 0
                Dim TotalResort As Double = 0
                Dim TotalSchool As Double = 0
                Dim TotalSupermarket As Double = 0
                Dim TotalTerminal As Double = 0
                Dim TotalSalesperLocationType As Double = 0

                For j As Integer = 0 To dtSalesperLocationType.Rows.Count - 1
                    Select Case CStr2(dtSalesperLocationType.Rows(j).Item("StoreLocation"))
                        Case "Airport"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOAirport = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOAirport
                            Else
                                COAirport = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COAirport
                            End If
                            TotalAirport += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Amusement Park"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOAmusementPark = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOAmusementPark
                            Else
                                COAmusementPark = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COAmusementPark
                            End If
                            TotalAmusementPark += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Commercial"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOCommercial = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOCommercial
                            Else
                                COCommercial = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COCommercial
                            End If
                            TotalCommercial += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Church"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOChurch = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOChurch
                            Else
                                COChurch = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COChurch
                            End If
                            TotalChurch += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Cinema"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOCinema = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOCinema
                            Else
                                COCinema = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COCinema
                            End If
                            TotalCinema += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Entertainment"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOEntertainment = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOEntertainment
                            Else
                                COEntertainment = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COEntertainment
                            End If
                            TotalEntertainment += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Food Court"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOFoodCourt = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOFoodCourt
                            Else
                                COFoodCourt = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COFoodCourt
                            End If
                            TotalFoodCourt += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Gasoline Station"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOGasolineStation = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOGasolineStation
                            Else
                                COGasolineStation = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COGasolineStation
                            End If
                            TotalGasolineStation += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Hospital"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOHospital = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOHospital
                            Else
                                COHospital = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COHospital
                            End If
                            TotalHospital += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Mall"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMall = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOMall
                            Else
                                COMall = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COMall
                            End If
                            TotalMall += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Mall-Cinema"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMallCinema = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOMallCinema
                            Else
                                COMallCinema = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COMallCinema
                            End If
                            TotalMallCinema += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Mall-Entertainment"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMallEntertainment = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOMallEntertainment
                            Else
                                COMallEntertainment = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COMallEntertainment
                            End If
                            TotalMallEntertainment += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Mall-Foodcourt"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMallFoodcourt = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOMallFoodcourt
                            Else
                                COMallFoodcourt = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COMallFoodcourt
                            End If
                            TotalMallFoodcourt += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Mall-Proper"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMallProper = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOMallProper
                            Else
                                COMallProper = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COMallProper
                            End If
                            TotalMallProper += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Mall-Supermarket"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOMallSupermarket = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOMallSupermarket
                            Else
                                COMallSupermarket = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COMallSupermarket
                            End If
                            TotalMallSupermarket += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Offices"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOOffices = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOOffices
                            Else
                                COOffices = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COOffices
                            End If
                            TotalOffices += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "PublicMarket"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOPublicMarket = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOPublicMarket
                            Else
                                COPublicMarket = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COPublicMarket
                            End If
                            TotalPublicMarket += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Residential"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOResidential = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOResidential
                            Else
                                COResidential = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COResidential
                            End If
                            TotalResidential += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Resort"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOResort = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOResort
                            Else
                                COResort = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COResort
                            End If
                            TotalResort += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "School"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOSchool = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOSchool
                            Else
                                COSchool = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COSchool
                            End If
                            TotalSchool += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Supermarket"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOSupermarket = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOSupermarket
                            Else
                                COSupermarket = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COSupermarket
                            End If
                            TotalSupermarket += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                        Case "Terminal"
                            If CBool2(dtSalesperLocationType.Rows(j).Item("FromFranchisee")) Then
                                FOTerminal = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalFOSalesperLocationType += FOTerminal
                            Else
                                COTerminal = CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                                TotalCOSalesperLocationType += COTerminal
                            End If
                            TotalTerminal += CDec2(dtSalesperLocationType.Rows(j).Item("GrossSales"))
                    End Select
                Next

                dtMain.Rows(53).Item(DateHeader) = Format(COAirport, "#,###,###.##")
                dtMain.Rows(54).Item(DateHeader) = Format(COAmusementPark, "#,###,###.##")
                dtMain.Rows(55).Item(DateHeader) = Format(COCommercial, "#,###,###.##")
                dtMain.Rows(56).Item(DateHeader) = Format(COChurch, "#,###,###.##")
                dtMain.Rows(57).Item(DateHeader) = Format(COCinema, "#,###,###.##")
                dtMain.Rows(58).Item(DateHeader) = Format(COEntertainment, "#,###,###.##")
                dtMain.Rows(59).Item(DateHeader) = Format(COFoodCourt, "#,###,###.##")
                dtMain.Rows(60).Item(DateHeader) = Format(COGasolineStation, "#,###,###.##")
                dtMain.Rows(61).Item(DateHeader) = Format(COHospital, "#,###,###.##")
                dtMain.Rows(62).Item(DateHeader) = Format(COMall, "#,###,###.##")
                dtMain.Rows(63).Item(DateHeader) = Format(COMallCinema, "#,###,###.##")
                dtMain.Rows(64).Item(DateHeader) = Format(COMallEntertainment, "#,###,###.##")
                dtMain.Rows(65).Item(DateHeader) = Format(COMallFoodcourt, "#,###,###.##")
                dtMain.Rows(66).Item(DateHeader) = Format(COMallProper, "#,###,###.##")
                dtMain.Rows(67).Item(DateHeader) = Format(COMallSupermarket, "#,###,###.##")
                dtMain.Rows(68).Item(DateHeader) = Format(COOffices, "#,###,###.##")
                dtMain.Rows(69).Item(DateHeader) = Format(COPublicMarket, "#,###,###.##")
                dtMain.Rows(70).Item(DateHeader) = Format(COResidential, "#,###,###.##")
                dtMain.Rows(71).Item(DateHeader) = Format(COResort, "#,###,###.##")
                dtMain.Rows(72).Item(DateHeader) = Format(COSchool, "#,###,###.##")
                dtMain.Rows(73).Item(DateHeader) = Format(COSupermarket, "#,###,###.##")
                dtMain.Rows(74).Item(DateHeader) = Format(COTerminal, "#,###,###.##")
                dtMain.Rows(75).Item(DateHeader) = Format(TotalCOSalesperLocationType, "#,###,###.##")
                dtMain.Rows(76).Item(DateHeader) = ""
                dtMain.Rows(77).Item(DateHeader) = ""

                dtMain.Rows(78).Item(DateHeader) = Format(FOAirport, "#,###,###.##")
                dtMain.Rows(79).Item(DateHeader) = Format(FOAmusementPark, "#,###,###.##")
                dtMain.Rows(80).Item(DateHeader) = Format(FOCommercial, "#,###,###.##")
                dtMain.Rows(81).Item(DateHeader) = Format(FOChurch, "#,###,###.##")
                dtMain.Rows(82).Item(DateHeader) = Format(FOCinema, "#,###,###.##")
                dtMain.Rows(83).Item(DateHeader) = Format(FOEntertainment, "#,###,###.##")
                dtMain.Rows(84).Item(DateHeader) = Format(FOFoodCourt, "#,###,###.##")
                dtMain.Rows(85).Item(DateHeader) = Format(FOGasolineStation, "#,###,###.##")
                dtMain.Rows(86).Item(DateHeader) = Format(FOHospital, "#,###,###.##")
                dtMain.Rows(87).Item(DateHeader) = Format(FOMall, "#,###,###.##")
                dtMain.Rows(88).Item(DateHeader) = Format(FOMallCinema, "#,###,###.##")
                dtMain.Rows(89).Item(DateHeader) = Format(FOMallEntertainment, "#,###,###.##")
                dtMain.Rows(90).Item(DateHeader) = Format(FOMallFoodcourt, "#,###,###.##")
                dtMain.Rows(91).Item(DateHeader) = Format(FOMallProper, "#,###,###.##")
                dtMain.Rows(92).Item(DateHeader) = Format(FOMallSupermarket, "#,###,###.##")
                dtMain.Rows(93).Item(DateHeader) = Format(FOOffices, "#,###,###.##")
                dtMain.Rows(94).Item(DateHeader) = Format(FOPublicMarket, "#,###,###.##")
                dtMain.Rows(95).Item(DateHeader) = Format(FOResidential, "#,###,###.##")
                dtMain.Rows(96).Item(DateHeader) = Format(FOResort, "#,###,###.##")
                dtMain.Rows(97).Item(DateHeader) = Format(FOSchool, "#,###,###.##")
                dtMain.Rows(98).Item(DateHeader) = Format(FOSupermarket, "#,###,###.##")
                dtMain.Rows(99).Item(DateHeader) = Format(FOTerminal, "#,###,###.##")
                dtMain.Rows(100).Item(DateHeader) = Format(TotalFOSalesperLocationType, "#,###,###.##")
                dtMain.Rows(101).Item(DateHeader) = ""
                dtMain.Rows(102).Item(DateHeader) = ""

                dtMain.Rows(103).Item(DateHeader) = Format(TotalAirport, "#,###,###.##")
                dtMain.Rows(104).Item(DateHeader) = Format(TotalAmusementPark, "#,###,###.##")
                dtMain.Rows(105).Item(DateHeader) = Format(TotalCommercial, "#,###,###.##")
                dtMain.Rows(106).Item(DateHeader) = Format(TotalChurch, "#,###,###.##")
                dtMain.Rows(107).Item(DateHeader) = Format(TotalCinema, "#,###,###.##")
                dtMain.Rows(108).Item(DateHeader) = Format(TotalEntertainment, "#,###,###.##")
                dtMain.Rows(109).Item(DateHeader) = Format(TotalFoodCourt, "#,###,###.##")
                dtMain.Rows(110).Item(DateHeader) = Format(TotalGasolineStation, "#,###,###.##")
                dtMain.Rows(111).Item(DateHeader) = Format(TotalHospital, "#,###,###.##")
                dtMain.Rows(112).Item(DateHeader) = Format(TotalMall, "#,###,###.##")
                dtMain.Rows(113).Item(DateHeader) = Format(TotalMallCinema, "#,###,###.##")
                dtMain.Rows(114).Item(DateHeader) = Format(TotalMallEntertainment, "#,###,###.##")
                dtMain.Rows(115).Item(DateHeader) = Format(TotalMallFoodcourt, "#,###,###.##")
                dtMain.Rows(116).Item(DateHeader) = Format(TotalMallProper, "#,###,###.##")
                dtMain.Rows(117).Item(DateHeader) = Format(TotalMallSupermarket, "#,###,###.##")
                dtMain.Rows(118).Item(DateHeader) = Format(TotalOffices, "#,###,###.##")
                dtMain.Rows(119).Item(DateHeader) = Format(TotalPublicMarket, "#,###,###.##")
                dtMain.Rows(120).Item(DateHeader) = Format(TotalResidential, "#,###,###.##")
                dtMain.Rows(121).Item(DateHeader) = Format(TotalResort, "#,###,###.##")
                dtMain.Rows(122).Item(DateHeader) = Format(TotalSchool, "#,###,###.##")
                dtMain.Rows(123).Item(DateHeader) = Format(TotalSupermarket, "#,###,###.##")
                dtMain.Rows(124).Item(DateHeader) = Format(TotalTerminal, "#,###,###.##")
                dtMain.Rows(125).Item(DateHeader) = Format(TotalCOSalesperLocationType + TotalFOSalesperLocationType, "#,###,###.##")

                DateFrom = DateFrom.AddDays(7)
                DateTo = DateTo.AddDays(7)

                cinco.DateFrom = CStr2(DateFrom)
                cinco.DateTo = CStr2(DateTo)

                SelectedWeekNo += 1
            Next

            pOut = New JsonDictionary
            pOut.Add("SalesperOwnership", dtSalesperOwnership)
            pOut.Add("DeliverySalesperOwnershipDelivery", dtDeliverySalesperOwnershipDelivery)
            pOut.Add("TakeOutSalesperOwnershipTakeOut", dtTakeOutSalesperOwnershipTakeOut)
            pOut.Add("SalesperRegion", dtSalesperRegion)
            pOut.Add("SalesperLocationType", dtSalesperLocationType)
            pOut.Add("MainTable", dtMain)

            Return pOut.Serialize
        End Function

        'End of Management Reports 

        Function GetDeviceUtilizationDetails() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
            cinco.StoreStatus = CStr2(Common.WebRequest.Data("StoreStatus"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
            cinco.StoreStatus = CStr2(Common.WebRequest.Data("StoreStatus"))
#End If
            Dim pOut As New JsonDictionary
            pOut.Add("Results", cinco.GetDeviceUtilizationDetails())

            Return pOut.Serialize()
        End Function

        Function CreateTableHeader(vTransactionType As String) As DataTable
            Dim dtMain As New DataTable
            Dim dr As DataRow

            dtMain.Columns.Add("-")

            dr = dtMain.NewRow
            dr(0) = vTransactionType & " per Ownership"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Company Owned"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Franchised"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "TOTAL"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = ""
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Delivery " & vTransactionType & " per Ownership"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Company Owned"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Franchised"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "TOTAL"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = ""
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Take - Out  " & vTransactionType & "  per Ownership"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Company Owned"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Franchised"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "TOTAL"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = ""
            dtMain.Rows.Add(dr)

            '=================per Region================
            dr = dtMain.NewRow
            dr(0) = "CO-" & vTransactionType & "  per Region"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "GMAN"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "GMAS"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "North Luzon"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "South Luzon"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Luzon"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = ""
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Visayas"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Cebu"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Mindanao"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Vismin"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = ""
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "FO-" & vTransactionType & "  per Region"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "GMAN"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "GMAS"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "North Luzon"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "South Luzon"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Luzon"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = ""
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Visayas"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Cebu"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Mindanao"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Vismin"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = ""
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "SystemWide-" & vTransactionType & "  per Region"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "GMAN"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "GMAS"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "North Luzon"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "South Luzon"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Luzon"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = ""
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Visayas"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Cebu"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Mindanao"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Vismin"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Total"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = ""
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "CO -  " & vTransactionType & "  per Location Type"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Airport"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Amusement Park"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Church"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Cinema"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Commercial"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Entertainment"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Food Court"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Gasoline Station"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Hospital"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Mall"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Mall-Cinema"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Mall-Entertainment"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Mall-Foodcourt"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Mall-Proper"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Mall-Supermarket"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Offices"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Public Market"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Residential"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Resort"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "School"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Supermarket"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Terminal"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Total"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = ""
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "FO -  " & vTransactionType & "  per Location Type"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Airport"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Amusement Park"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Church"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Cinema"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Commercial"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Entertainment"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Food Court"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Gasoline Station"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Hospital"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Mall"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Mall-Cinema"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Mall-Entertainment"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Mall-Foodcourt"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Mall-Proper"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Mall-Supermarket"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Offices"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Public Market"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Residential"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Resort"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "School"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Supermarket"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Terminal"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Total"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = ""
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Systemwide -  " & vTransactionType & "  per Location Type"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Airport"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Amusement Park"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Church"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Cinema"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Commercial"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Entertainment"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Food Court"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Gasoline Station"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Hospital"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Mall"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Mall-Cinema"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Mall-Entertainment"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Mall-Foodcourt"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Mall-Proper"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Mall-Supermarket"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Offices"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Public Market"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Residential"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Resort"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "School"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Supermarket"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Terminal"
            dtMain.Rows.Add(dr)

            dr = dtMain.NewRow
            dr(0) = "Total"
            dtMain.Rows.Add(dr)


            Return dtMain
        End Function

        Function SendEmailDeviceUtilizationDetails() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

#If DEBUG Then
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
            cinco.StoreStatus = CStr2(Common.WebRequest.Data("StoreStatus"))
#Else
            cinco.DateFrom = CStr2(Common.WebRequest.Data("dateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("dateTo"))
            cinco.FOWSTID = CStr2(Common.WebRequest.Data("FOWSTID"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))
            cinco.StoreStatus = CStr2(Common.WebRequest.Data("StoreStatus"))
#End If

            Return JsonDictionary.Serialize("Result", cinco.SendEmailDeviceUtilizationDetails())
        End Function


        '================================ Device Utilization Dashboard =======================================

        Function GetOrganizationWideUtilization() As String
            'If (Now - systemwideDatetime).TotalMinutes < 5 Then
            '    Return systemwide
            'End If

            Dim cinco As New CincoCorporation

            'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut As New JsonDictionary

            pOut.Add("Result", cinco.GetOrganizationWideUtilization())
            pOut.Add("ClosedStores", Common.WebRequest.ProData.Execute("Select [OwnershipType], COUNT(ID) Count from TransStoreDeviceUtilization " & vbCrLf &
                "WHERE [Status] = 'Closed' AND convert(date,[date]) = convert(date," & ReadyString(Common.WebRequest.Data("Date")) & ")" & vbCrLf &
                "Group by OwnershipType " & vbCrLf))
            Return pOut.Serialize()

        End Function

        Function GetUtilizationByOwnershipType() As String
            'If (Now - ownershiptypeDatetime).TotalMinutes < 5 Then
            '    Return ownershiptype
            'End If

            Dim cinco As New CincoCorporation

            'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            ownershiptype = JsonDictionary.Serialize("Result", cinco.GetUtilizationByOwnershipType())
            ownershiptypeDatetime = Now
            Return ownershiptype

        End Function

        Function GetStoreWithTheFewestNoOfTransactions() As String
            If CBool2(WebRequest.Instance.QueryString("IsDownloadData")) Then
                Dim pSourceURL As String = Common.ServerPath & "FileTemplates\" & CStr(WebRequest.Instance.OrgID) & "\"
                Dim strFileName = "Stores with fewest transactions as of " & Format(CDate(WebRequest.Instance.QueryString("Date")), "MM-dd-yyyy") & ".xls"

                If (IO.File.Exists(pSourceURL & "\" & strFileName)) Then
                    IO.File.Delete(pSourceURL & "\" & strFileName)
                End If

                TableToExcelFile(fewesttrans, "", "", pSourceURL & "\" & strFileName)

                Dim file As New System.IO.FileInfo(pSourceURL & "\" & strFileName)

                Response.Clear()
                Response.AddHeader("Content-Disposition", "attachment; filename=" & strFileName)
                Response.AddHeader("Content-Length", file.Length.ToString())
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/")
                Response.ContentType = "application/octet-stream"
                Response.WriteFile(file.FullName)
                Response.End()

                Return JsonDictionary.Serialize("success", file.FullName)
            End If

            Dim cinco As New CincoCorporation

            'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            fewesttrans = cinco.GetStoreWithTheFewestNoOfTransactions()

            Return JsonDictionary.Serialize("Result", fewesttrans)

        End Function


        Function GetCountStoreWithTheFewestNoOfTransactions() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim cinco As New CincoCorporation

            Return JsonDictionary.Serialize("Result", cinco.GetCountStoreWithTheFewestNoOfTransactions())
        End Function

        Function GetUtilizationByOwnershipTypeAndGroup() As String
            Dim cinco As New CincoCorporation

            'If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Result", cinco.GetUtilizationByOwnershipTypeAndGroup())

        End Function

        Function GetDeviceWithoutSalesData() As String
            Dim cinco As New CincoCorporation

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            cinco.DateFrom = CStr2(Common.WebRequest.Data("DateFrom"))
            cinco.DateTo = CStr2(Common.WebRequest.Data("DateTo"))
            cinco.GroupID = CStr2(Common.WebRequest.Data("GroupID"))

            Return JsonDictionary.Serialize("Result", cinco.GetDeviceWithoutSalesData())

        End Function

        Function GetFORegions()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim cinco As New CincoCorporation
            Return JsonDictionary.Serialize("Result", cinco.GetFORegions())
        End Function

        Function GetDetailedDirectManagerReport()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim cinco As New CincoCorporation
            Dim pDT As DataTable = cinco.GetDetailedDirectManagerReport()

            If pDT Is Nothing Then
                Return JsonDictionary.Serialize("Result", "")
            Else
                Return JsonDictionary.Serialize("Result", pDT)
            End If
        End Function

        Function GetUtilSummaryPerRegion()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim cinco As New CincoCorporation
            Common.WebRequest.ProData.CommandTimeout = 600
            Return Json(New With {Key .Results = cinco.GetUtilSummaryPerRegion()})
        End Function

        Function GetSystemWideUtilSummaryPerRegion()
            If (Now - franchisereportDate).TotalMinutes < 3 And franchisereportViewDate = Common.WebRequest.Data("DateFrom") Then
                Return franchisereport
            End If
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim cinco As New CincoCorporation
            Common.WebRequest.ProData.CommandTimeout = 600
            franchisereport = Json(New With {Key .Results = cinco.GetUtilSummaryPerRegion()})
            franchisereportDate = Now
            franchisereportViewDate = Common.WebRequest.Data("DateFrom")
            'AppendToLog("UTILIZATION INTERVAL: " & franchisereportDate & ", DATE REPORT: " & franchisereportViewDate)
            Return franchisereport
        End Function

        Function GetStoreStatus()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim pSql As String

            pSql = "SELECT Status FROM MobizLibraryOrganizationFOWST WHERE ID = " & ReadyString(Common.WebRequest.Data("FOWSTID"))
            Return JsonDictionary.Serialize("Result", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function SaveStoreStatus()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Try
                Dim p As New InsertUpdate("MobizLibraryOrganizationFOWST")

                p("Status") = ReadyString(Common.WebRequest.Data("Status"))
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = "getdate()"

                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(Common.WebRequest.Data("FOWSTID"))))

                Return JsonDictionary.Serialize("Result", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function GetFlushingSummary()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim pSql As String
            Dim pOut As New JsonDictionary
            Dim dateFrom As String = Format(CDate(Common.WebRequest.Data("Date")), "MM-dd-yyyy")

            pSql = " SELECT " & vbCrLf &
                    "   --(SELECT COUNT(*) FROM TransImports WHERE Result IS NULL) as [CO-NoOfFlushing] " & vbCrLf &
                    "   COUNT(DISTINCT FowstID) as [CO-NoOfFlushing] FROM TransUnflushedCounts WHERE UnflushedCount > 0 AND CONVERT(date, FlushingDate) = " & ReadyString(dateFrom) & vbCrLf

            pOut.Add("CO", Common.WebRequest.ProData.Execute(pSql))

            pSql = " --USE MobizPotato_Corner_Franchise " & vbCrLf &
                    "   --SELECT (SELECT COUNT(*) FROM TransImports WHERE Result IS NULL) as [FO-NoOfFlushing] " & vbCrLf &
                    "   SELECT COUNT(DISTINCT FowstID) as [FO-NoOfFlushing] FROM TransUnflushedCounts WHERE UnflushedCount > 0 AND CONVERT(date, FlushingDate) = " & ReadyString(dateFrom) & vbCrLf

            pOut.Add("FO", Common.WebRequest.ProData.Execute(pSql))

            Return pOut.Serialize()
        End Function

        Function GetStoreUtilStatus()
            If CBool2(WebRequest.Instance.QueryString("IsDownloadData")) Then
                Dim pSourceURL As String = Common.ServerPath & "FileTemplates\" & CStr(WebRequest.Instance.OrgID) & "\"
                Dim strFileName = "Stores Utilization Status.xls"

                If (IO.File.Exists(pSourceURL & "\" & strFileName)) Then
                    IO.File.Delete(pSourceURL & "\" & strFileName)
                End If

                TableToExcelFile(storeutilstatus, "", "", pSourceURL & "\" & strFileName)

                Dim file As New System.IO.FileInfo(pSourceURL & "\" & strFileName)

                Response.Clear()
                Response.AddHeader("Content-Disposition", "attachment; filename=" & strFileName)
                Response.AddHeader("Content-Length", file.Length.ToString())
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/")
                Response.ContentType = "application/octet-stream"
                Response.WriteFile(file.FullName)
                Response.End()

                Return JsonDictionary.Serialize("success", file.FullName)
            End If

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim cinco As New CincoCorporation

            storeutilstatus = cinco.GetStoreUtilStatus()
            Return JsonDictionary.Serialize("Results", storeutilstatus)
        End Function

        Function DownloadFranchiseReport()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim cinco As New CincoCorporation
            Common.WebRequest.ProData.CommandTimeout = 600
            Return JsonDictionary.Serialize("Result", cinco.DownloadFranchiseReport())
        End Function

        Function DownloadFile() As ActionResult
            Dim pFileName = Common.WebRequest.QueryString("FileName")
            Dim pOrgID = Common.WebRequest.QueryString("OrgID")
            Dim pSourceURL As String = Common.ServerPath & "FileTemplates\" & pOrgID & "\" & pFileName
            Dim pContentType As String = "application/csv"

            Try
                Return File(pSourceURL, pContentType, pFileName)
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        Function GetMonthlyUtil()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim cinco As New CincoCorporation
            Common.WebRequest.ProData.CommandTimeout = 600
            monthlyutil = cinco.GetMonthlyUtil()
            Return Json(New With {Key .Results = monthlyutil})
        End Function

        Function DownloadMonthlyUtil()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim cinco As New CincoCorporation

            Return JsonDictionary.Serialize("Result", cinco.DownloadMonthlyUtil(monthlyutil))
        End Function

        Function GetOrganizationSummarySales()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim cinco As New CincoCorporation

            Return cinco.GetOrganizationSummarySales()
        End Function

        Function GetMPOSDashboardDetails()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim cinco As New CincoCorporation

            Return cinco.GetMPOSDashboardDetails()
        End Function

        Function GetMPOSDashboardDetailedReport()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim cinco As New CincoCorporation

            Return cinco.GetMPOSDashboardDetailedReport()
        End Function
    End Class
End Namespace