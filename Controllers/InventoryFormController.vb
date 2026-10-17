Namespace WebProj
    Public Class InventoryFormController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /Inventory Form

        Function Index() As ActionResult
            Return View()
        End Function

        <HttpPost>
        Function getInventoryForm() As String
            Dim pSql As String
            Dim pDt As DataTable

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "Select CONVERT(VARCHAR(10),TransDailyBeginningBalance.BeginningBalanceDate,110) BeginningBalanceDate, MobizLibraryOrganizationFOWST.Name FOWST, " & vbCrLf &
                    "ProductName, TransDailyBeginningBalance.BegBalance, TransDailyBeginningBalance.Sales, " & vbCrLf &
                    "TransDailyBeginningBalance.Transfers, TransDailyBeginningBalance.Receiving, " & vbCrLf &
                    "TransDailyBeginningBalance.SalesReturns, TransDailyBeginningBalance.InventoryCountVariance, " & vbCrLf &
                    "TransDailyBeginningBalance.EndBalance, " & vbCrLf &
                    "MobizLibraryPeople.SurName + ', ' +  MobizLibraryPeople.FirstName Employee from TransDailyBeginningBalance " & vbCrLf &
                    "INNER JOIN MobizLibraryPIMProduct on MobizLibraryPIMProduct.id = TransDailyBeginningBalance.ProductID " & vbCrLf &
                    "INNER JOIN MobizLibraryPeople on MobizLibraryPeople.id = TransDailyBeginningBalance.PeopleID " & vbCrLf &
                    "INNER JOIN MobizLibraryOrganizationFOWST on MobizLibraryOrganizationFOWST.id = TransDailyBeginningBalance.FOWSTID " & vbCrLf &
                    "WHERE convert(date, BeginningBalanceDate) between " & vbCrLf &
                    ReadyString(Common.WebRequest.Data("From")) & " AND " & vbCrLf &
                    ReadyString(Common.WebRequest.Data("To")) & " AND " & vbCrLf &
                    "FOWSTID = " & ReadyString(Common.WebRequest.Data("FOWSTID"))

            If Not CStr2(Common.WebRequest.Data("ProductName")) = vbNullString Then
                pSql += " AND ProductName like " & ReadyStringWild(Common.WebRequest.Data("ProductName"))
            End If

            pDt = Common.WebRequest.ProData.Execute(pSql)

            If CBool2(Common.WebRequest.Data("IsDownloadExcelFile")) Then
                Dim filaname As String

                filaname = "InventoryForm_" & Replace(CStr2(Common.WebRequest.Data("From")), "/", "") & "_" & Replace(CStr2(Common.WebRequest.Data("To")), "/", "") & ".xls"

                pDt.Columns("BeginningBalanceDate").ColumnName = "Date"
                pDt.Columns("BegBalance").ColumnName = "Beginning Balance"
                pDt.Columns("ProductName").ColumnName = "Product Name"
                pDt.Columns("Transfers").ColumnName = "Transfer Out"
                pDt.Columns("SalesReturns").ColumnName = "Sales Returns"
                pDt.Columns("InventoryCountVariance").ColumnName = "Inventory Count Variance"
                pDt.Columns("EndBalance").ColumnName = "Ending Balance"

                TableToExcelFile(pDt, "", "", Common.ServerPath & "temp\" & filaname)

                Return JsonDictionary.Serialize("Results", filaname)
            Else
                Return JsonDictionary.Serialize("Results", pDt)
            End If



        End Function

        Function DownloadInventoryReport() As ActionResult

            Dim pFileTitle = Common.WebRequest.QueryString("p")
            Dim pSourceURL As String = Common.ServerPath & "temp\" & pFileTitle
            Dim pContentType As String = "application/csv"

            Return File(pSourceURL, pContentType, pFileTitle)

        End Function

        Function ReactivateDeactivatedSkus()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim dates As String = Common.WebRequest.Data("Date")
                Dim pSql As String

                pSql = "SELECT COUNT(ID) as Cnt FROM LibraryOrganizationPIM WHERE DeactivationDate = " & ReadyString(dates)

                Dim cnt As Integer = Common.WebRequest.ProData.Execute(pSql).Rows(0)("Cnt")

                pSql = "UPDATE LibraryOrganizationPIM SET IsDeactivated = 0, DeactivationDate = NULL" & vbCrLf &
                        " WHERE DeactivationDate = " & ReadyString(dates)

                Common.WebRequest.ProData.Execute(pSql)

                UpdateProductDeactivation()

                Return JsonDictionary.Serialize("Results", cnt)
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function UpdateProductDeactivation() As String
            Dim pSql As String
            Dim pDTDeactivateProducts As DataTable
            Dim pIU As InsertUpdate

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                pSql = "update LibraryOrganizationPIM set LastTransactionDate = " & vbCrLf &
                        "   (Select top 1 LastUpdated from TransInventoryLedger where ProductID =  " & vbCrLf &
                        "   LibraryOrganizationPIM.ProductID order by TransactionDate desc), " & vbCrLf &
                        " LastTransactionReference = " & vbCrLf &
                        "   (Select top 1 TransactionType from TransInventoryLedger where ProductID = " & vbCrLf &
                        "   LibraryOrganizationPIM.ProductID order by TransactionDate desc) " & vbCrLf
                Common.WebRequest.ProData.Execute(pSql)

                pSql = "Select ProductID, LastTransactionDate,LastTransactionReference from LibraryOrganizationPIM " & vbCrLf &
                    "where IsDeactivated=0 and DATEDIFF(DAY,LastTransactionDate,GETDATE()) > " & IIf(WebRequest.Instance.Business.SystemDefaults.DormantSKUsPeriod = "", 365, WebRequest.Instance.Business.SystemDefaults.DormantSKUsPeriod) & vbCrLf &
                    "and ProductID not in (Select ProductID from LibraryPIMForDeactivationAndReactivation)"

                pDTDeactivateProducts = Common.WebRequest.ProData.Execute(pSql)

                For i As Integer = 0 To pDTDeactivateProducts.Rows.Count - 1

                    pIU = New InsertUpdate("LibraryPIMForDeactivationAndReactivation")

                    pIU("ProductID") = ReadyString(pDTDeactivateProducts.Rows(i).Item("ProductID"))
                    pIU("LastTransactionDate") = ReadyString(pDTDeactivateProducts.Rows(i).Item("LastTransactionDate"))
                    pIU("LastTransactionReference") = ReadyString(pDTDeactivateProducts.Rows(i).Item("LastTransactionReference"))
                    pIU("Flag") = ReadyString("For Deactivation")

                    pSql = pIU.GetInsert

                    Common.WebRequest.ProData.Execute(pSql)
                Next

                Return JsonDictionary.Serialize("Results", "done")
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetDeactivatingProducts()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String
                Dim pDT As DataTable
                Dim PageStart As Integer = CInt(Request.Form.GetValues("start")(0))
                Dim PageNoOfRows As Integer = CInt(Request.Form.GetValues("length")(0))
                Dim PageSearchText As String = Request.Form.GetValues("search[value]")(0).ToString()
                Dim IsCountPage As Boolean
                Dim PageColumnNo As String = Request.Form.GetValues("order[0][column]")(0).ToString()
                Dim PageColumnDirection As String = Request.Form.GetValues("order[0][dir]")(0).ToString()

                Dim whereSearch As String = " WHERE Flag = 'For Deactivation' "
                Dim whereOrder As String = ""
                Dim whereLimit As String = " WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows))

                If PageSearchText <> "" Then
                    Dim whereSearchContent As String = ""
                    Dim operators As String = ""

                    Dim arr As Object = Split(PageSearchText, " ")

                    For i As Integer = 0 To arr.length - 1
                        If whereSearchContent <> "" Then
                            operators = " OR "
                        End If

                        whereSearchContent = whereSearchContent & operators & " pim.ProductNo + ' ' + " & vbCrLf &
                                        " pim.ProductName " & vbCrLf &
                                        " LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                    Next
                    whereSearch &= " AND (" & whereSearchContent & ")"
                End If

                If PageColumnNo = 0 Or PageColumnNo = 1 Then
                    whereOrder = " ORDER BY mainsql.ProductName " & PageColumnDirection
                    'ElseIf PageColumnNo = 2 Then
                    '    whereOrder = " ORDER BY mainsql.DateHired " & PageColumnDirection
                    'ElseIf PageColumnNo = 5 Then
                    '    whereOrder = " ORDER BY mainsql.IsBan " & PageColumnDirection
                End If

                Dim mainpSql = "SELECT pim.ID as ProductID, pim.ProductNo, pim.ProductName, pim.Brand, " & vbCrLf &
                        " pimdeac.ID, pimdeac.LastTransactionDate, pimdeac.LastTransactionReference " & vbCrLf &
                        " FROM LibraryPIMForDeactivationAndReactivation as pimdeac " & vbCrLf &
                        " LEFT JOIN MobizLibraryPIMProduct as pim ON pim.ID = pimdeac.ProductID " & vbCrLf

                pSql = "SELECT * FROM  (Select mainsql.* , ROW_NUMBER() OVER (" & whereOrder & ") AS RowNum From ( " & vbCrLf &
                        mainpSql & vbCrLf &
                        whereSearch & vbCrLf &
                        ") as mainsql ) as v2 " & whereLimit

                pDT = Common.WebRequest.ProData.Execute(pSql)

                Dim pDTCount As DataTable = Common.WebRequest.ProData.Execute(mainpSql & whereSearch)

                Return Json(New With {Key .DormantPeriod = IIf(WebRequest.Instance.Business.SystemDefaults.DormantSKUsPeriod = "", 365, WebRequest.Instance.Business.SystemDefaults.DormantSKUsPeriod), .data = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDT)), .recordsTotal = pDTCount.Rows().Count, .recordsFiltered = pDTCount.Rows().Count}, JsonRequestBehavior.AllowGet)

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))

            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function CountDormantSkus()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String = " SELECT COUNT(ID) as Counter FROM LibraryPIMForDeactivationAndReactivation "
                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function DeactivateProduct()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim rowid As String = Common.WebRequest.Data("RowID")
                Dim productid As String = Common.WebRequest.Data("ProductID")

                Common.WebRequest.ProData.BeginTransaction()
                DeactivateNow(rowid, productid)
                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "deactivated")

            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function DeactivateAllDormantSkus()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Common.WebRequest.ProData.BeginTransaction()
                Dim IsDeactivateAll As Boolean = CBool2(Common.WebRequest.Data("IsDeactivateAll"))
                Dim DonotDeactivate As String = CStr2(Common.WebRequest.Data("DonotDeactivate"))
                Dim Deactivate As String = CStr2(Common.WebRequest.Data("Deactivate"))
                Dim AllSkusSelected As String = CStr2(Common.WebRequest.Data("AllSkusSelected"))
                Dim pSql As String = "SELECT ProductID FROM LibraryPIMForDeactivationAndReactivation "

                If IsDeactivateAll And AllSkusSelected <> "''" Then
                    pSql += " WHERE ID NOT IN (" & DonotDeactivate & ")"
                Else
                    If Deactivate <> "''" Then
                        pSql += " WHERE ID IN (" & Deactivate & ")"
                    End If
                End If

                Dim pDT = Common.WebRequest.ProData.Execute(pSql)
                Dim pDtProductIds = ExtractFieldValues(pDT, "ProductID", 50)
                For Each pDrProductIDs As DataRow In pDtProductIds.Rows
                    pSql = "DELETE FROM LibraryPIMForDeactivationAndReactivation WHERE ProductID IN (" & pDrProductIDs("ProductID") & ")"
                    pSql = Common.WebRequest.ProData.ExecuteSQL(pSql) ' output is used to debugging only

                    Dim p As New InsertUpdate("LibraryOrganizationPIM")
                    p("IsDeactivated") = 1
                    p("DeactivationDate") = "getdate()"
                    p("LastUpdated") = "getdate()"
                    pSql = p.GetUpdate(" ProductID IN (" & pDrProductIDs("ProductID") & ")")
                    pSql = Common.WebRequest.ProData.ExecuteSQL(pSql) ' output is used to debugging only

                    pSql = "DELETE FROM LibraryPimGroupPims Where ProductID IN (" & pDrProductIDs("ProductID") & ")"
                    pSql = Common.WebRequest.ProData.ExecuteSQL(pSql) ' output is used to debugging only
                Next
                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "deactivated")

            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function DeactivateNow(ByVal rowid As String, ByVal productid As String)
            Dim p As New InsertUpdate("LibraryOrganizationPIM")
            Dim pSql As String

            pSql = "DELETE FROM LibraryPIMForDeactivationAndReactivation WHERE ID = " & ReadyString(rowid)
            Common.WebRequest.ProData.Execute(pSql)

            p("IsDeactivated") = 1
            p("DeactivationDate") = "getdate()"
            p("LastUpdated") = "getdate()"

            Common.WebRequest.ProData.Execute(p.GetUpdate(" ProductID = " & ReadyString(productid)))

            Dim hb As New ProductGroupController
            hb.RemoveProductInHB(productid)
        End Function
    End Class
End Namespace