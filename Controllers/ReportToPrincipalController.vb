Namespace WebProj
    Public Class ReportToPrincipalController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /ReportToPrincipal

        Function Index() As ActionResult
            Return View()
        End Function

        Function AdjustInventory() As ActionResult
            If WebRequest.Instance.HandleOrganization(True) = False Then Exit Function
            AdjustInventoryValues()
        End Function

        Sub AdjustInventoryValues()
            Dim pFinder As Finder
            Dim pDTLedgerBin As DataTable
            Dim pDTBinProduct As DataTable
            Dim pDTBinProduct2 As DataTable
            Dim pDT As DataTable
            pFinder = New Finder

            'get all company product
            With pFinder
                .ProData = Common.WebRequest.ProData
                .Distinct = True
                With .Fields
                    With .Add("ProductID", SqlDbType.VarChar)
                        .Expression = "dbo.LibraryOrganizationPIM.ProductID"
                    End With
                End With

                .SqlFrom = "FROM LibraryOrganizationPIM"
                pDT = .Requery
            End With

            For i As Integer = 0 To pDT.Rows.Count - 1
                pFinder = New Finder

                'get bin from ledger filter by product
                With pFinder
                    .ProData = Common.WebRequest.ProData
                    .Distinct = True

                    With .Fields
                        With .Add("BinID", SqlDbType.VarChar)
                            .Expression = "TransInventoryLedger.BinID"
                        End With
                        With .Add("FowstID", SqlDbType.VarChar)
                            .Expression = "TransInventoryLedger.FowstID"
                        End With
                    End With

                    .SqlFrom = "from TransInventoryLedger"
                    .SqlWhere = "WHERE ProductID = " & ReadyString(pDT.Rows(i).Item("ProductID"))

                    pDTLedgerBin = .Requery

                    For j As Integer = 0 To pDTLedgerBin.Rows.Count - 1
                        pFinder = New Finder

                        'check if bin exists in fowstbinproduct table -filter by product
                        With pFinder
                            .ProData = Common.WebRequest.ProData

                            With .Fields
                                With .Add("ID", SqlDbType.VarChar)
                                    .Expression = "LibraryFowstBinProduct.ID"
                                End With
                                With .Add("BinID", SqlDbType.VarChar)
                                    .Expression = "LibraryFowstBinProduct.BinID"
                                End With
                                With .Add("Name", SqlDbType.VarChar)
                                    .Expression = "LibraryFowstBin.Name"
                                End With
                            End With
                            .SqlFrom = "from LibraryFowstBinProduct inner join LibraryFowstBin on LibraryFowstBin.ID = LibraryFowstBinProduct.BinID"
                            .SqlWhere = "WHERE ProductID = " & ReadyString(pDT.Rows(i).Item("ProductID")) & " AND LibraryFowstBinProduct.BinID = " & ReadyString(pDTLedgerBin.Rows(j).Item("BinID"))

                            pDTBinProduct = .Requery

                            If pDTBinProduct.Rows.Count = 0 Then
                                Dim pSql As String

                                pSql = "insert into LibraryFowstBinProduct(FowstID, ProductID, BinID) values(" & ReadyString(pDTLedgerBin.Rows(j).Item("FowstID")) & "," & ReadyString(pDT.Rows(i).Item("ProductID")) & "," & ReadyString(pDTLedgerBin.Rows(j).Item("BinID")) & ")"

                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            'ElseIf pDTBinProduct.Rows(0).Item("Name") = "Selling Area" Then
                            '    AddOrSetNewDefaultBin(pDT.Rows(i).Item("ProductID").ToString, "78534471-9B4F-4638-8D8A-970893DA332C", pDTBinProduct.Rows(0).Item("BinID").ToString)
                            End If

                        End With
                    Next
                End With


                'get all bin in LibraryFowstBinProduct to update default bin and set 'default bin to 0'
                pFinder = New Finder
                With pFinder
                    .ProData = Common.WebRequest.ProData

                    With .Fields
                        With .Add("ID", SqlDbType.VarChar)
                            .Expression = "LibraryFowstBinProduct.ID"
                        End With
                        With .Add("BinID", SqlDbType.VarChar)
                            .Expression = "LibraryFowstBinProduct.BinID"
                        End With
                        With .Add("Name", SqlDbType.VarChar)
                            .Expression = "LibraryFowstBin.Name"
                        End With
                        With .Add("IsDefault", SqlDbType.VarChar)
                            .Expression = "LibraryFowstBinProduct.IsDefault"
                        End With
                    End With
                    .SqlFrom = "from LibraryFowstBinProduct inner join LibraryFowstBin on LibraryFowstBin.ID = LibraryFowstBinProduct.BinID"
                    .SqlWhere = "WHERE ProductID = " & ReadyString(pDT.Rows(i).Item("ProductID"))

                    pDTBinProduct2 = .Requery
                End With

                If pDTBinProduct2.Rows.Count > 1 Then
                    pDTBinProduct2.DefaultView.RowFilter = "IsDefault=1 AND Name='Unnamed Bin'"

                    'adjustment starts here
                    If pDTBinProduct2.DefaultView.Count > 0 Then
                        Dim DefaultBinID As String = pDTBinProduct2.DefaultView(0).Item("BinID").ToString
                        Dim pSql As String

                        On Error GoTo Err

                        Common.WebRequest.ProData.BeginTransaction()

                        pDTBinProduct2.DefaultView.RowFilter = "Name <> 'Unnamed Bin'"

                        If pDTBinProduct2.DefaultView.Count > 0 Then
                            'get last balance of default bin
                            pSql = "Select top 1 ID, Balance from TransInventoryLedger where ProductID = " & ReadyString(pDT.Rows(i).Item("ProductID")) & " AND BinID = " & ReadyString(DefaultBinID) & " Order by Lastupdated desc"

                            Dim dtQtyDefaultBinLastBalance As DataTable = Common.WebRequest.ProData.Execute(pSql)
                            Dim QtyDefaultBinLastBalance As Integer = CInt(dtQtyDefaultBinLastBalance.Rows(0).Item("Balance"))
                            Dim IDQtyDefaultBinLastBalance As String = dtQtyDefaultBinLastBalance.Rows(0).Item("ID").ToString

                            'zero out 'default bin'
                            AddToInventoryLedger((New Guid).ToString, DefaultBinID, 0, "N/A", pDT.Rows(i).Item("ProductID").ToString, "78534471-9B4F-4638-8D8A-970893DA332C")

                            'set default bin's IsDefault = 0
                            pSql = "Update LibraryFowstBinProduct set IsDefault = 0 where ProductID = " & ReadyString(pDT.Rows(i).Item("ProductID")) & " AND BinID = " & ReadyString(DefaultBinID) & " AND dbo.LibraryFowstBinProduct.FowstID = " & ReadyString("78534471-9B4F-4638-8D8A-970893DA332C")
                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                            'assign new default bin
                            pSql = "Update LibraryFowstBinProduct set IsDefault = 1 where ProductID = " & ReadyString(pDT.Rows(i).Item("ProductID")) & " AND BinID = " & ReadyString(pDTBinProduct2.DefaultView(0).Item("BinID")) & " AND dbo.LibraryFowstBinProduct.FowstID = " & ReadyString("78534471-9B4F-4638-8D8A-970893DA332C")
                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                            'get last balance and lastupdate of last inventory count
                            pSql = "Select top 1 Balance, LastUpdated from TransInventoryLedger " &
                                "where ProductID = " & ReadyString(pDT.Rows(i).Item("ProductID")) &
                                " AND BinID = " & ReadyString(pDTBinProduct2.DefaultView(0).Item("BinID")) &
                                " AND TransactionType = 'InventoryCount'" & " Order by Lastupdated desc"

                            Dim dtQtyNewDefaultBinLastCount As DataTable = Common.WebRequest.ProData.Execute(pSql)
                            Dim QtyNewDefaultBinLastCount As Integer
                            Dim IDQtyNewDefaultBinLastCount As String

                            If dtQtyNewDefaultBinLastCount.Rows.Count > 0 Then
                                QtyNewDefaultBinLastCount = CInt(dtQtyNewDefaultBinLastCount.Rows(0).Item("Balance"))
                            End If

                            'get last balance of 'default bin' before the last count
                            pSql = "Select top 1 ID, Balance from TransInventoryLedger " &
                                "where ProductID = " & ReadyString(pDT.Rows(i).Item("ProductID")) & " AND BinID = " & ReadyString(DefaultBinID) &
                                " AND LastUpdated < " & ReadyString(dtQtyNewDefaultBinLastCount.Rows(0).Item("LastUpdated")) &
                                " Order by Lastupdated desc"

                            Dim dtQtyLastCount As DataTable = Common.WebRequest.ProData.Execute(pSql)
                            Dim QtyLastCount As Integer


                            If dtQtyLastCount.Rows.Count > 0 Then
                                If Not dtQtyLastCount.Rows(0).Item("ID").ToString = IDQtyDefaultBinLastBalance Then
                                    QtyLastCount = CInt(dtQtyLastCount.Rows(0).Item("Balance"))
                                End If
                            End If

                            'apply formula
                            AddToInventoryLedger((New Guid).ToString, pDTBinProduct2.DefaultView(0).Item("BinID").ToString, QtyDefaultBinLastBalance - QtyLastCount + QtyNewDefaultBinLastCount, "N/A", pDT.Rows(i).Item("ProductID").ToString, "78534471-9B4F-4638-8D8A-970893DA332C")
                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                            Common.WebRequest.ProData.CommitTransaction()
                        End If

                    End If
                End If
            Next
            Exit Sub
err:
            MsgBox(Err.Description)
            Common.WebRequest.ProData.RollbackTransaction()
        End Sub

        Function AddOrSetNewDefaultBin(ByVal ProductID As String, ByVal FowstID As String, ByVal BinID As String)
            Dim pSql As String
            Dim dt As New DataTable
            Dim pOut As String

            pSql = "SELECT dbo.LibraryFowstBin.ID " &
                    "FROM dbo.LibraryFowstBin INNER JOIN " &
                    "dbo.LibraryFowstBinProduct ON dbo.LibraryFowstBin.FowstID = dbo.LibraryFowstBinProduct.FowstID AND  " &
                    "dbo.LibraryFowstBin.ID = dbo.LibraryFowstBinProduct.BinID " &
                    "WHERE dbo.LibraryFowstBinProduct.ProductID = " & ReadyString(ProductID) &
                    " AND dbo.LibraryFowstBinProduct.IsDefault = 1 " &
                    " AND  dbo.LibraryFowstBinProduct.FowstID = " & ReadyString(FowstID) &
                    " AND dbo.LibraryFowstBin.Name = 'Unnamed Bin'"

            dt = Common.WebRequest.ProData.Execute(pSql)

            If dt.Rows.Count > 0 Then
                'set default bin balance to 0
                AddToInventoryLedger(ProductID, dt.Rows(0).Item("ID").ToString, 0, "N/A", ProductID, FowstID)

                'dt.Rows(0).Item("ID").ToString = default bin ID
                pSql = "Update LibraryFowstBinProduct set IsDefault = 0 where ProductID = " & ReadyString(ProductID) & " AND BinID = " & ReadyString(dt.Rows(0).Item("ID").ToString) & " AND dbo.LibraryFowstBinProduct.FowstID = " & ReadyString(FowstID)
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                'check if the current bin is already assigned to this product in LibraryFowstBinProduct

                'AddOrSetNewDefaultBin()
                pSql = "SELECT BinID" &
                 " FROM LibraryFowstBinProduct" &
                 " WHERE FowstID = " & ReadyString(FowstID) &
                 " AND ProductID = " & ReadyString(ProductID) &
                 " AND BinID = " & ReadyString(BinID)
                pOut = CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))

                If CStr2(pOut) <> "" Then
                    pSql = "Update LibraryFowstBinProduct set IsDefault = 1 where ProductID = " & ReadyString(ProductID) & " AND BinID = " & ReadyString(BinID) & " AND dbo.LibraryFowstBinProduct.FowstID = " & ReadyString(FowstID)
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                Else
                    Dim pIU As InsertUpdate

                    pIU = New InsertUpdate("LibraryFowstBinProduct")
                    pIU("FowstID") = ReadyString(FowstID)
                    pIU("BinID") = ReadyString(BinID)
                    pIU("ProductID") = ReadyString(ProductID)
                    pIU("IsDefault") = 1
                    pSql = pIU.GetInsert
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If
            End If
        End Function

        <HttpPost()>
        Function AddToInventoryLedger(ByVal TransactionID As String, ByVal BinID As String, ByVal Total As Double, ByVal ExpDate As String, ByVal ProductID As String, ByVal FowstID As String)
            Dim EDate As Date?

            If Not ExpDate = "N/A" Then
                EDate = CDate(ExpDate)
            End If

            WebRequest.Instance.Business.TransInventoryLedger.Add( _
                            CStr2(ProductID), _
                            CStr2(FowstID), _
                            TransactionID, _
                            TransInventoryLedger.eTransType.InventoryCount, _
                            WebSession.Instance.PeopleUserID, , _
                            Total, _
                            BinID,
                            EDate
                        )
        End Function

        <HttpPost()>
        Function GetWeeklySalesReport() As String
            Dim clsReportToPrincipal As New ReportToPrincipal

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Report", clsReportToPrincipal.GetWeeklySalesReport())

        End Function

        <HttpPost()>
        Function GenerateWeeklyInventoryReport() As String
            Dim clsReportToPrincipal As New ReportToPrincipal

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Report", clsReportToPrincipal.GetWeeklyInventoryReport())
        End Function

        <HttpPost()>
        Function GenerateMonthlyInventoryReport() As String
            Dim clsReportToPrincipal As New ReportToPrincipal

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Report", clsReportToPrincipal.GetMonthInventoryReport())
        End Function

        <HttpPost()>
        Function GetOnitsukaWeeklySalesReport() As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsReportToPrincipal As New Triggers()
            Dim tempStrPath As String

            tempStrPath = clsReportToPrincipal.GetOnitsukaSalesReport(CDate(WebRequest.Instance.Data("From")), CDate(WebRequest.Instance.Data("To")))

            Return JsonDictionary.Serialize("Report", tempStrPath)
        End Function

        <HttpPost()>
        Function GetOnitsukaInventoryReport() As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsReportToPrincipal As New Triggers()
            Dim tempStrPath As String

            tempStrPath = clsReportToPrincipal.GetOnitsukaInventoryReport(CDate(WebRequest.Instance.Data("From")), CDate(WebRequest.Instance.Data("To")))

            Return JsonDictionary.Serialize("Report", tempStrPath)
        End Function

        Function DownloadReportToPrincipal() As ActionResult

            Dim pFileTitle = Common.WebRequest.QueryString("p")
            Dim pSourceURL As String = Common.ServerPath & "temp\" & pFileTitle
            Dim pContentType As String = "application/csv"

            Return File(pSourceURL, pContentType, pFileTitle)

        End Function
    End Class
End Namespace