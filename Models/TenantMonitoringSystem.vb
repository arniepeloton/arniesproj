Public Class TenantMonitoringSystem

    Function GetTrendByFOWST() As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim trendObj As New ArrayList
        Dim pOut As JsonDictionary
        Dim gBy As String = Common.WebRequest.Data("GroupBy")

        pOut = New JsonDictionary
        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("FowstID", SqlDbType.Date)
                    .Expression = "MobizLibraryOrganizationFOWST.ID"
                End With

                With .Add("FowstName", SqlDbType.Date)
                    .Expression = "MobizLibraryOrganizationFOWST.Name"
                End With

                With .Add("GrossSales", SqlDbType.Date)
                    .Expression = "SUM(TransInvoiceItems.GrossSalesNetOfVATExemptions)"
                End With

                With .Add("Discounts", SqlDbType.Date)
                    .Expression = "SUM(TransInvoiceItems.DiscountAmount)"
                End With
            End With

            .SqlFrom = "FROM TransInvoiceItems inner join TransInvoice " & vbCrLf &
                        "   ON TransInvoiceItems.InvoiceID = transinvoice.ID " & vbCrLf &
                        "INNER JOIN MobizLibraryOrganizationFOWST " & vbCrLf &
                        "   ON MobizLibraryOrganizationFOWST.ID = TransInvoice.FOWSTID " & vbCrLf &
                        "INNER JOIN MobizLibraryOrganizationFOWSTMallInfoSM " & vbCrLf &
                        "   ON MobizLibraryOrganizationFOWSTMallInfoSM.FOWSTID = TransInvoice.FOWSTID "

            .SqlWhere = " WHERE Transinvoice.InvoiceNumber > 0 AND CONVERT(date, Transinvoice.InvoiceDate) BETWEEN " & ReadyString(CDate(Common.WebRequest.Data("From"))) & " AND " & ReadyString(CDate(Common.WebRequest.Data("To")))

            If gBy = "Class Code" Then
                .SqlWhere += " AND MobizLibraryOrganizationFOWSTMallInfoSM.ClassCode = " & ReadyString(Common.WebRequest.Data("FilVal"))

            ElseIf gBy = "Trade Code" Then
                .SqlWhere += " AND MobizLibraryOrganizationFOWSTMallInfoSM.TradeCode = " & ReadyString(Common.WebRequest.Data("FilVal"))

            ElseIf gBy = "Sales Type" Then
                .SqlWhere += " AND MobizLibraryOrganizationFOWSTMallInfoSM.SaleType = " & ReadyString(Common.WebRequest.Data("FilVal"))

            End If

            .SqlGroup = " GROUP BY MobizLibraryOrganizationFOWST.ID, MobizLibraryOrganizationFOWST.Name"

            pDT = .Requery
        End With

        If pDT.Rows.Count > 0 Then
            For i As Integer = 0 To pDT.Rows.Count - 1
                Dim pFowstResult As DataTable = GetTrendByFOWSTQuery(gBy, Common.WebRequest.Data("FilVal"), pDT.Rows(i).Item("FowstID").ToString(), Common.WebRequest.Data("From"), Common.WebRequest.Data("To"))
                trendObj.Add(JsonDictionary.Serialize(pFowstResult))
            Next i

            pOut.Add("Results", trendObj)
            Return pOut.Serialize

        Else
            pOut.Add("Results", "0")
            Return pOut.Serialize
        End If

    End Function

    Function GetTrendByFOWSTQuery(ByVal filType As String, ByVal filTypeVal As String, ByVal id As String, ByVal dateFrom As String, ByVal dateTo As String) As DataTable
        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT DISTINCT MobizLibraryOrganizationFOWST.ID AS FowstID, MobizLibraryOrganizationFOWST.Name AS FowstName " +
                "   , CONVERT(date, Transinvoice.InvoiceDate) AS InvoiceDate " +
                "   , SUM(TransInvoiceItems.GrossSalesNetOfVATExemptions) AS GrossSales " +
                "   , SUM(TransInvoiceItems.DiscountAmount) AS Discounts " +
                "FROM TransInvoiceItems inner join TransInvoice " +
                "   ON TransInvoiceItems.InvoiceID = transinvoice.ID " +
                "INNER JOIN MobizLibraryOrganizationFOWST " +
                "   ON MobizLibraryOrganizationFOWST.ID = TransInvoice.FOWSTID " +
                "INNER JOIN MobizLibraryOrganizationFOWSTMallInfoSM " +
                "   ON MobizLibraryOrganizationFOWSTMallInfoSM.FOWSTID = TransInvoice.FOWSTID " +
                "WHERE Transinvoice.InvoiceNumber > 0 AND CONVERT(date, Transinvoice.InvoiceDate) BETWEEN " + ReadyString(CDate(dateFrom)) + "" +
                "   AND " + ReadyString(CDate(dateTo))

        If filType = "Class Code" Then
            pSql += "AND MobizLibraryOrganizationFOWSTMallInfoSM.ClassCode = " + ReadyString(filTypeVal)

        ElseIf filType = "Trade Code" Then
            pSql += "AND MobizLibraryOrganizationFOWSTMallInfoSM.TradeCode = " + ReadyString(filTypeVal)

        ElseIf filType = "Sales Type" Then
            pSql += "AND MobizLibraryOrganizationFOWSTMallInfoSM.SaleType = " + ReadyString(filTypeVal)

        End If

        pSql += " AND MobizLibraryOrganizationFOWST.ID = " + ReadyString(id) + " GROUP BY MobizLibraryOrganizationFOWST.ID, MobizLibraryOrganizationFOWST.Name, CONVERT(date, Transinvoice.InvoiceDate) "

        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return pDT

    End Function

    Function GetDistibutionByFOWST() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        Dim gBy As String = Common.WebRequest.Data("GroupBy")

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                With .Add("FowstID", SqlDbType.Date)
                    .Expression = "MobizLibraryOrganizationFOWST.ID"
                End With

                With .Add("FowstName", SqlDbType.Date)
                    .Expression = "MobizLibraryOrganizationFOWST.Name"
                End With

                With .Add("GrossSales", SqlDbType.Date)
                    .Expression = "SUM(TransInvoiceItems.GrossSalesNetOfVATExemptions)"
                End With

                With .Add("Discounts", SqlDbType.Date)
                    .Expression = "SUM(TransInvoiceItems.DiscountAmount)"
                End With
            End With

            .SqlFrom = "FROM TransInvoiceItems inner join TransInvoice " & vbCrLf &
                        "   ON TransInvoiceItems.InvoiceID = transinvoice.ID " & vbCrLf &
                        "INNER JOIN MobizLibraryOrganizationFOWST " & vbCrLf &
                        "   ON MobizLibraryOrganizationFOWST.ID = TransInvoice.FOWSTID " & vbCrLf &
                        "INNER JOIN MobizLibraryOrganizationFOWSTMallInfoSM " & vbCrLf &
                        "   ON MobizLibraryOrganizationFOWSTMallInfoSM.FOWSTID = TransInvoice.FOWSTID "

            .SqlWhere = " WHERE Transinvoice.InvoiceNumber > 0 AND CONVERT(date, Transinvoice.InvoiceDate) BETWEEN " & ReadyString(CDate(Common.WebRequest.Data("From"))) & " AND " & ReadyString(CDate(Common.WebRequest.Data("To")))

            If gBy = "Class Code" Then
                .SqlWhere += " AND MobizLibraryOrganizationFOWSTMallInfoSM.ClassCode = " & ReadyString(Common.WebRequest.Data("FilVal"))

            ElseIf gBy = "Trade Code" Then
                .SqlWhere += " AND MobizLibraryOrganizationFOWSTMallInfoSM.TradeCode = " & ReadyString(Common.WebRequest.Data("FilVal"))

            ElseIf gBy = "Sales Type" Then
                .SqlWhere += " AND MobizLibraryOrganizationFOWSTMallInfoSM.SaleType = " & ReadyString(Common.WebRequest.Data("FilVal"))

            End If

            .SqlGroup = " GROUP BY MobizLibraryOrganizationFOWST.ID, MobizLibraryOrganizationFOWST.Name"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function GetTotalForAllFowst() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim gBy As String = Common.WebRequest.Data("GroupBy")

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields
                If gBy = "Date" Then
                    With .Add("InvoiceDate", SqlDbType.Date)
                        .Expression = "CONVERT(date, dbo.TransInvoice.InvoiceDate)"
                    End With

                ElseIf gBy = "Class Code" Then
                    With .Add("Name", SqlDbType.Date)
                        .Expression = "MobizLibraryOrganizationFOWSTMallInfoSM.ClassCode"
                    End With

                ElseIf gBy = "Trade Code" Then
                    With .Add("Name", SqlDbType.Date)
                        .Expression = "MobizLibraryOrganizationFOWSTMallInfoSM.TradeCode"
                    End With

                ElseIf gBy = "Sales Type" Then
                    With .Add("Name", SqlDbType.Date)
                        .Expression = "MobizLibraryOrganizationFOWSTMallInfoSM.SaleType"
                    End With

                ElseIf gBy = "Day" Then
                    With .Add("Name", SqlDbType.Date)
                        .Expression = "DATENAME(dw, dbo.TransInvoice.InvoiceDate)"
                    End With

                ElseIf gBy = "Hour" Then
                    With .Add("Name", SqlDbType.Date)
                        .Expression = "DATEPART(hh, dbo.TransInvoice.InvoiceDate)"
                    End With

                    With .Add("Starts", SqlDbType.Date)
                        .Expression = "MIN(CAST(TransInvoice.InvoiceDate as Time))"
                    End With

                    With .Add("Ends", SqlDbType.Date)
                        .Expression = "MAX(CAST(TransInvoice.InvoiceDate as Time))"
                    End With

                ElseIf gBy = "Store Name" Then
                    With .Add("Name", SqlDbType.Date)
                        .Expression = "MobizLibraryOrganizationFOWST.Name"
                    End With

                ElseIf gBy = "Store Address" Then
                    With .Add("Name", SqlDbType.Date)
                        .Expression = "dbo.SplitJoinComma(ISNULL(MobizLibraryOrganizationFOWST.StreetNumber, '') + ', ' + ISNULL(MobizLibraryOrganizationFOWST.Street, '') + ', ' + ISNULL(MobizLibraryCities.Name, '') + ', ' + ISNULL(MobizLibraryCountries.Name, ''))"
                    End With

                ElseIf gBy = "Mall Code" Then
                    With .Add("Name", SqlDbType.Date)
                        .Expression = "MobizLibraryOrganizationFOWSTMallInfoSM.MallCode"
                    End With

                ElseIf gBy = "Tenant No" Then
                    With .Add("Name", SqlDbType.Date)
                        .Expression = "MobizLibraryOrganizationFOWSTMallInfoSM.TenantNo"
                    End With

                ElseIf gBy = "Class Code" Then
                    With .Add("Name", SqlDbType.Date)
                        .Expression = "MobizLibraryOrganizationFOWSTMallInfoSM.ClassCode"
                    End With

                ElseIf gBy = "Trade Code" Then
                    With .Add("Name", SqlDbType.Date)
                        .Expression = "MobizLibraryOrganizationFOWSTMallInfoSM.TradeCode"
                    End With

                ElseIf gBy = "Outlet No" Then
                    With .Add("Name", SqlDbType.Date)
                        .Expression = "MobizLibraryOrganizationFOWSTMallInfoSM.OutletNo"
                    End With

                ElseIf gBy = "Sales Type" Then
                    With .Add("Name", SqlDbType.Date)
                        .Expression = "MobizLibraryOrganizationFOWSTMallInfoSM.SalesType"
                    End With
                End If

                With .Add("GrossSales", SqlDbType.Date)
                    .Expression = "SUM(TransInvoiceItems.GrossSalesNetOfVATExemptions)"
                End With

                With .Add("Discounts", SqlDbType.Date)
                    .Expression = "SUM(TransInvoiceItems.DiscountAmount)"
                End With
            End With

            .SqlFrom = "FROM TransInvoiceItems inner join TransInvoice " & vbCrLf &
                        "   ON TransInvoiceItems.InvoiceID = transinvoice.ID " & vbCrLf &
                        "INNER JOIN MobizLibraryOrganizationFOWST " & vbCrLf &
                        "   ON MobizLibraryOrganizationFOWST.ID = TransInvoice.FOWSTID " & vbCrLf &
                        "INNER JOIN MobizLibraryOrganizationFOWSTMallInfoSM " & vbCrLf &
                        "   ON MobizLibraryOrganizationFOWSTMallInfoSM.FOWSTID = TransInvoice.FOWSTID "

            .SqlWhere = " WHERE Transinvoice.InvoiceNumber > 0 AND CONVERT(date, Transinvoice.InvoiceDate) BETWEEN " & ReadyString(CDate(Common.WebRequest.Data("From"))) & " AND " & ReadyString(CDate(Common.WebRequest.Data("To")))


            If gBy = "Date" Then
                .SqlGroup = " GROUP BY CONVERT(date, dbo.TransInvoice.InvoiceDate)"

            ElseIf gBy = "Class Code" Then
                .SqlGroup = " GROUP BY MobizLibraryOrganizationFOWSTMallInfoSM.ClassCode"

            ElseIf gBy = "Trade Code" Then
                .SqlWhere += " AND MobizLibraryOrganizationFOWSTMallInfoSM.TradeCode <> '' "
                .SqlGroup = " GROUP BY MobizLibraryOrganizationFOWSTMallInfoSM.TradeCode"

            ElseIf gBy = "Sales Type" Then
                .SqlWhere += " AND MobizLibraryOrganizationFOWSTMallInfoSM.SaleType <> '' "
                .SqlGroup = " GROUP BY MobizLibraryOrganizationFOWSTMallInfoSM.SaleType"

            ElseIf gBy = "Day" Then
                .SqlGroup = " GROUP BY DATENAME(dw, dbo.TransInvoice.InvoiceDate)"

            ElseIf gBy = "Hour" Then
                .SqlGroup = " GROUP BY DATEPART(hh, dbo.TransInvoice.InvoiceDate)"

            ElseIf gBy = "Store Name" Then
                .SqlGroup = " GROUP BY MobizLibraryOrganizationFOWST.Name"

            ElseIf gBy = "Store Address" Then
                .SqlFrom += "INNER JOIN MobizLibraryCities" & vbCrLf &
                            "   ON MobizLibraryCities.ID = MobizLibraryOrganizationFOWST.FowstCityID " & vbCrLf &
                            "INNER JOIN MobizLibraryCountries " & vbCrLf &
                            "   ON MobizLibraryCountries.ID = MobizLibraryOrganizationFOWST.FowstCountryID "

                .SqlGroup = " GROUP BY MobizLibraryOrganizationFOWST.StreetNumber, MobizLibraryOrganizationFOWST.Street, MobizLibraryCities.Name, MobizLibraryCountries.Name"

            ElseIf gBy = "Mall Code" Then
                .SqlGroup = " GROUP BY MobizLibraryOrganizationFOWSTMallInfoSM.MallCode"

            ElseIf gBy = "Tenant No" Then
                .SqlGroup = " GROUP BY MobizLibraryOrganizationFOWSTMallInfoSM.TenantNo"

            ElseIf gBy = "Class Code" Then
                .SqlGroup = " GROUP BY MobizLibraryOrganizationFOWSTMallInfoSM.ClassCode"

            ElseIf gBy = "Trade Code" Then
                .SqlGroup = " GROUP BY MobizLibraryOrganizationFOWSTMallInfoSM.TradeCode"

            ElseIf gBy = "Outlet No" Then
                .SqlGroup = " GROUP BY MobizLibraryOrganizationFOWSTMallInfoSM.OutletNo"

            ElseIf gBy = "Sales Type" Then
                .SqlGroup = " GROUP BY MobizLibraryOrganizationFOWSTMallInfoSM.SalesType"
            End If

            pDT = .Requery

        End With
        Return pDT
    End Function

    Function GetPOSSalesReportBySalesDistributionByTime() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim SqlWhere As String
        Dim SqlGroupBy As String
        Dim Fowst As String()
        Dim CtrFowst As Integer = 1

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .Distinct = True

            With .Fields

                If Not WebRequest.Instance.Data("GroupBy") = "Date" Then
                    With .Add("FOWSTID", SqlDbType.UniqueIdentifier)
                        .Searchable = False
                        .Expression = "dbo.MobizLibraryOrganizationFOWST.ID"
                    End With

                    With .Add("FOWST", SqlDbType.VarChar)
                        .Expression = "dbo.MobizLibraryOrganizationFOWST.Name"
                    End With
                End If


                Select Case WebRequest.Instance.Data("GroupBy")
                    Case "Hour"
                        With .Add("InvoiceDate", SqlDbType.VarChar)
                            .Expression = "DATEPART(hh, InvoiceDate)"
                        End With
                        With .Add("Total", SqlDbType.Decimal)
                            .Expression = "SUM(GrossSalesNetOfVATExemptions)"
                        End With
                        With .Add("Starts", SqlDbType.Date)
                            .Expression = "MIN(CAST(TransInvoice.InvoiceDate as Time))"
                        End With

                        With .Add("Ends", SqlDbType.Date)
                            .Expression = "MAX(CAST(TransInvoice.InvoiceDate as Time))"
                        End With
                        SqlGroupBy = ", DATEPART(hh, InvoiceDate)"
                    Case "Day"
                        With .Add("InvoiceDate", SqlDbType.VarChar)
                            .Expression = "DATEPART(weekday, InvoiceDate)"
                        End With
                        With .Add("Total", SqlDbType.Decimal)
                            .Expression = "SUM(GrossSalesNetOfVATExemptions)"
                        End With
                        SqlGroupBy = ", DATEPART(weekday, InvoiceDate)"
                    Case "Date"
                        With .Add("InvoiceDate", SqlDbType.VarChar)
                            .Expression = "convert(date, InvoiceDate)"
                        End With
                        With .Add("Total", SqlDbType.Decimal)
                            .Expression = "SUM(GrossSalesNetOfVATExemptions)"
                        End With
                        SqlGroupBy = "convert(date, InvoiceDate)"
                        'Case "Month"
                        '    With .Add("InvoiceDate", SqlDbType.VarChar)
                        '        .Expression = "Month(dbo.TransInvoice.InvoiceDate)"
                        '    End With
                        '    With .Add("Total", SqlDbType.Decimal)
                        '        .Expression = "SUM(dbo.TransInvoiceItems.GrossSalesNetOfVATExemptions)"
                        '    End With
                        '    SqlGroupBy = ", Month(dbo.TransInvoice.InvoiceDate)"

                End Select
            End With

            .SqlFrom = "FROM dbo.TransInvoiceItems INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryPIMProduct ON dbo.TransInvoiceItems.ProductID = dbo.MobizLibraryPIMProduct.ID INNER JOIN " & vbCrLf &
                      "dbo.TransInvoice ON dbo.TransInvoiceItems.InvoiceID = dbo.TransInvoice.ID INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryOrganizationFOWST ON dbo.TransInvoice.FowstID = dbo.MobizLibraryOrganizationFOWST.ID "

            SqlWhere = " WHERE CONVERT(date, dbo.TransInvoice.InvoiceDate) BETWEEN " & ReadyString(WebRequest.Instance.Data("From")) & " AND " & ReadyString(WebRequest.Instance.Data("To")) & SqlWhere

            Fowst = Split(WebRequest.Instance.Data("Fowst"), "|")

            If Not WebRequest.Instance.Data("GroupBy") = "Date" Then
                SqlWhere &= " AND FowstID = " & ReadyString(Fowst(0))
            End If

            While CtrFowst <= UBound(Fowst)
                If Not Fowst(CtrFowst) = vbNullString Then
                    SqlWhere &= " OR FowstID = " & ReadyString(Fowst(CtrFowst))
                End If
                CtrFowst += 1
            End While

            .SqlWhere = SqlWhere

            If Not WebRequest.Instance.Data("GroupBy") = "Date" Then
                .SqlGroup = "GROUP BY dbo.MobizLibraryOrganizationFOWST.ID, dbo.MobizLibraryOrganizationFOWST.Name" & SqlGroupBy
            Else
                .SqlGroup = "GROUP BY " & SqlGroupBy
            End If

            pDT = .Requery

        End With

        Return pDT
    End Function
End Class