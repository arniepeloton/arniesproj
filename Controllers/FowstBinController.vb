Namespace WebProj
    Public Class FowstBinController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /FowstBin

        Function Index() As ActionResult
            'If Common.WebRequest.QueryString("IsMobile") = "1" Then Return View()

            'If WebRequest.Instance.RedirectToLogin(ErpAccess.eAccess.PointOfSales_PointOfSales, ErpAccess.eAccessMode.IsEnable, True) = False Then Exit Function
            Return View()
        End Function

        Function GetOrganizationID()
            Return JsonDictionary.Serialize("OrganizationID", WebSession.Instance.SubscriberOrganizationID)
        End Function

        <HttpPost()>
        Function SaveBinByMultipleStock()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim p As New InsertUpdate("LibraryFowstBinProduct")
            Dim pSql As String
            Dim pDT As DataTable
            Dim ProductID As String

            For Each pObj As Object In Common.WebRequest.AllData("ProductIDs")

                ProductID = CStr2(pObj)

                pSql = "Select * from LibraryFowstBinProduct where FowstID = " & ReadyString(WebRequest.Instance.Data("FowstID")) &
                                                                " AND ProductID = " & ReadyString(ProductID) &
                                                                " AND BinID = " & ReadyString(WebRequest.Instance.Data("BinID"))
                pDT = Common.WebRequest.ProData.Execute(pSql)

                If pDT.Rows.Count > 0 Then

                    pSql = "Update LibraryFowstBinProduct set IsActive=1 where FowstID = " & ReadyString(WebRequest.Instance.Data("FowstID")) &
                                                                " AND ProductID = " & ReadyString(ProductID) &
                                                                " AND BinID = " & ReadyString(WebRequest.Instance.Data("BinID"))

                Else

                    p("FowstID") = ReadyString(IIf(WebRequest.Instance.Data("FowstID") = "", WebSession.Instance.FowstID, WebRequest.Instance.Data("FowstID")))
                    p("BinID") = ReadyString(WebRequest.Instance.Data("BinID"))
                    p("ProductID") = ReadyString(ProductID)

                    pSql = p.GetInsert()

                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)

            Next

            Return JsonDictionary.Serialize("Save", "True")
        End Function

        <HttpPost()>
        Function SaveBinStock()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim p As New InsertUpdate("LibraryFowstBinProduct")
            Dim pSql As String
            Dim pDT As DataTable

            pSql = "Select * from LibraryFowstBinProduct where FowstID = " & ReadyString(WebRequest.Instance.Data("FowstID")) &
                                                            " AND ProductID = " & ReadyString(WebRequest.Instance.Data("ProductID")) &
                                                            " AND BinID = " & ReadyString(WebRequest.Instance.Data("BinID"))
            pDT = Common.WebRequest.ProData.Execute(pSql)

            If pDT.Rows.Count > 0 Then

                pSql = "Update LibraryFowstBinProduct set IsActive=1 where FowstID = " & ReadyString(WebRequest.Instance.Data("FowstID")) &
                                                            " AND ProductID = " & ReadyString(WebRequest.Instance.Data("ProductID")) &
                                                            " AND BinID = " & ReadyString(WebRequest.Instance.Data("BinID"))

            Else

                p("FowstID") = ReadyString(IIf(WebRequest.Instance.Data("FowstID") = "", WebSession.Instance.FowstID, WebRequest.Instance.Data("FowstID")))
                p("BinID") = ReadyString(WebRequest.Instance.Data("BinID"))
                p("ProductID") = ReadyString(WebRequest.Instance.Data("ProductID"))

                pSql = p.GetInsert()

            End If
            
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Return JsonDictionary.Serialize("Save", "True")
        End Function

        <HttpPost()>
        Function SaveBin()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim p As New InsertUpdate("LibraryFowstBin")
            Dim pSql As String

            If CInt(WebRequest.Instance.Data("IsCustomerBin")) Then
                p = New InsertUpdate("LibraryOrganizationCustomerFowstBin")
            End If

            If Not CStr2(WebRequest.Instance.Data("ID")) = "" Then
                p("ID") = ReadyString(WebRequest.Instance.Data("ID"))
            End If

            p("FowstID") = ReadyString(IIf(WebRequest.Instance.Data("FowstID") = "", WebSession.Instance.FowstID, WebRequest.Instance.Data("FowstID")))
            p("Name") = ReadyString(WebRequest.Instance.Data("Name"))
            p("Description") = ReadyString(WebRequest.Instance.Data("Description"))

            If CBool2(WebRequest.Instance.Data("IsEdit")) Then
                pSql = p.GetUpdate("ID = " & ReadyString(WebRequest.Instance.Data("ID")))
            Else
                pSql = p.GetInsert()
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Return JsonDictionary.Serialize("Save", "True")
        End Function

        <HttpPost()>
        Function GetAllArea() As String
            Dim pSql As String
            Dim pDT As DataTable

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "Select * from LibraryArea where FowstID = " & ReadyString(WebRequest.Instance.Data("FowstID"))
            pDT = Common.WebRequest.ProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", pDT)
        End Function

        <HttpPost()>
        Function SaveBinBatch() As String

            Dim p As New InsertUpdate("LibraryFowstBin")
            Dim StrData As String()
            Dim pSql As String
            Dim totalBin As Integer = 0
            Dim pOut As JsonDictionary
            Dim pDT As DataTable
            Dim dtArea As DataTable
            Dim AreaID As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            For Each pObj As Object In Common.WebRequest.AllData("BinNames")
                totalBin = totalBin + 1
            Next

            Dim arrayBin(totalBin) As Object
            Dim arrayID(totalBin) As Object
            Dim arrayBinLongNames(totalBin) As Object

            Dim counted As Integer = 0
            For Each pObj As Object In Common.WebRequest.AllData("BinID")
                arrayID(counted) = pObj
                counted = counted + 1
            Next

            counted = 0
            For Each pObj As Object In Common.WebRequest.AllData("BinLongNames")
                arrayBinLongNames(counted) = pObj
                counted = counted + 1
            Next

            pSql = "Select ID from LibraryFowstArea where AreaName = " & ReadyString(CStr2(Common.WebRequest.Data("Area")))
            dtArea = Common.WebRequest.ProData.Execute(pSql)

            If dtArea.Rows.Count > 0 Then
                AreaID = CStr2(dtArea.Rows(0).Item("ID"))
            Else
                AreaID = Guid.NewGuid.ToString

                pSql = "Insert into LibraryFowstArea(ID, FowstID, AreaName) values(" &
                    ReadyString(AreaID) & "," &
                    ReadyString(WebRequest.Instance.Data("FowstID")) & "," &
                    ReadyString(CStr2(Common.WebRequest.Data("Area"))) & ")"
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            counted = 0
            For Each pObj As Object In Common.WebRequest.AllData("BinNames")
                arrayBin(counted) = pObj
                StrData = Split(pObj.ToString, ",")

                pSql = "SELECT ID FROM LibraryFowstBin WHERE Name = " & ReadyString(StrData(0)) &
                                                            " AND FOWSTID=" & ReadyString(WebRequest.Instance.Data("FowstID")) &
                                                            " AND AreaID=" & ReadyString(AreaID)
                pDT = Common.WebRequest.ProData.Execute(pSql)


                p("FowstID") = ReadyString(IIf(WebRequest.Instance.Data("FowstID") = "", WebSession.Instance.FowstID, WebRequest.Instance.Data("FowstID")))
                StrData = Split(arrayBin(counted).ToString, ",")
                p("Name") = ReadyString(StrData(0))
                p("AreaID") = ReadyString(AreaID)
                p("BinLongName") = ReadyString(arrayBinLongNames(counted))
                p("Description") = "''"
                p("IsActive") = 1

                If Not pDT.Rows.Count > 0 Then
                    StrData = Split(arrayID(counted).ToString, ",")
                    p("ID") = ReadyString(StrData(0))
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate("ID=" & ReadyString(CStr2(pDT.Rows(0).Item("ID")))) '"Update LibraryFowstBin set IsActive=1 where ID=" & ReadyString(CStr2(pDT.Rows(0).Item("ID")))
                    arrayID(counted) = CStr2(pDT.Rows(0).Item("ID"))
                End If

                Common.WebRequest.ProData.ExecuteSQL(pSql)
                counted = counted + 1
            Next

            pOut = New JsonDictionary
            pOut.Add("BinName", arrayBin)
            pOut.Add("BinID", arrayID)

            Return pOut.Serialize()

        End Function

        <HttpPost()>
        Function SearchBin()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim pSql As String
            Dim StrData As String()
            Dim pDT As DataTable
            Dim pOut As JsonDictionary
            Dim totalBin As Integer = 0
            Dim i As Integer = 0

            For Each pObj As Object In Common.WebRequest.AllData("Bins")
                totalBin = totalBin + 1
            Next
            Dim arrayBin(totalBin - 1) As Object

            For Each pObj As Object In Common.WebRequest.AllData("Bins")
                StrData = Split(pObj.ToString, ",")
                pSql = "SELECT ID, Name FROM LibraryFowstBin WHERE Name = " & ReadyString(StrData(0))
                pDT = Common.WebRequest.ProData.Execute(pSql)

                If Not pDT.Rows.Count > 0 Then
                    arrayBin(i) = pObj
                    i = i + 1
                End If
            Next

            Dim sortedBin(i) As Object
            For j As Integer = 0 To i - 1
                sortedBin(j) = arrayBin(j)
            Next

            pOut = New JsonDictionary
            pOut.Add("Results", sortedBin)

            Return pOut.Serialize()

        End Function

        <HttpPost()>
        Function SaveCustomerBin()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim p As New InsertUpdate("LibraryOrganizationCustomerFowstBin")
            Dim pSql As String

            p("FowstID") = ReadyString(IIf(WebRequest.Instance.Data("FowstID") = "", WebSession.Instance.FowstID, WebRequest.Instance.Data("FowstID")))
            p("Name") = ReadyString(WebRequest.Instance.Data("Name"))
            p("Description") = ReadyString(WebRequest.Instance.Data("Description"))

            If CBool(WebRequest.Instance.Data("IsEdit")) Then
                pSql = p.GetUpdate("ID = " & ReadyString(WebRequest.Instance.Data("ID")))
            Else
                pSql = p.GetInsert()
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Return JsonDictionary.Serialize("Save", "True")
        End Function

        <HttpPost()>
        Function DeactivateFOWSTBinProduct()
            Dim pSql As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "Update LibraryFowstBinProduct set IsDeactivated = 1 " & vbCrLf &
                    " WHERE FOWSTID = " & ReadyString(Common.WebRequest.Data("FowstID")) & vbCrLf &
                    " AND BinID = " & ReadyString(Common.WebRequest.Data("BinID")) & vbCrLf &
                    " AND ProductID = " & ReadyString(Common.WebRequest.Data("ProductID"))


            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Return JsonDictionary.Serialize("Save", "True")
        End Function

        <HttpPost()>
        Function GetAllProductsByBin()
            Dim pFinder As Finder
            Dim pDT As DataTable
            Dim SqlWhere As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pFinder = New Finder

            With pFinder
                .ProData = Common.WebRequest.ProData
                .MaxRecords = 50
                .Distinct = True
                With .Fields
                    With .Add("ID", SqlDbType.Date)
                        .Expression = "dbo.MobizLibraryPIMProduct.ID"
                    End With
                    With .Add("ProductNo", SqlDbType.Date)
                        .Expression = "dbo.MobizLibraryPIMProduct.ProductNo"
                    End With
                    With .Add("ProductName", SqlDbType.Date)
                        .Expression = "dbo.MobizLibraryPIMProduct.ProductName"
                    End With
                End With

                .SqlFrom = "FROM MobizLibraryPIMProduct inner join LibraryFowstBinProduct on " & vbCrLf &
                            "LibraryFowstBinProduct.ProductID = MobizLibraryPIMProduct.ID " & vbCrLf &
                            "INNER join LibraryFowstBin on " & vbCrLf &
                            "LibraryFowstBin.ID = LibraryFowstBinProduct.BinID"

                .SqlWhere = "WHERE LibraryFowstBinProduct.BinID = " & ReadyString(WebRequest.Instance.Data("BinID")) & vbCrLf &
                            " AND LibraryFowstBinProduct.FowstID = " & ReadyString(WebRequest.Instance.Data("FowstID")) & vbCrLf &
                            " AND dbo.LibraryFowstBin.Name <> 'Unnamed Bin'"

                .SqlOrder = "Order by dbo.MobizLibraryPIMProduct.ProductNo"
                pDT = .Requery
            End With

            Return JsonDictionary.Serialize("Results", pDT)
        End Function

        Function DeactivateBin() As String
            Dim pSql As String
            Dim pDT As DataTable
            Dim pFowstID As String
            Dim pBinID As String
            Dim pProductID As String
            Dim pDTBins As DataTable
            Dim pDTBinProducts As DataTable


            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pFowstID = CStr2(WebRequest.Instance.Data("FowstID"))

            If CBool2(Common.WebRequest.Data("DeactivateAll")) Then

                pSql = "Select * from LibraryFowstBin where FowstID = " & ReadyString(pFowstID) & " AND Name<>'Unnamed Bin'"
                pDTBins = Common.WebRequest.ProData.Execute(pSql)

                For i As Integer = 0 To pDTBins.Rows.Count - 1

                    pBinID = CStr2(pDTBins.Rows(i).Item("ID"))

                    pSql = "Update LibraryFowstBin set IsActive = 0 where ID = " & ReadyString(pBinID)
                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    pSql = "Select ID, ProductID from LibraryFowstBinProduct" & vbCrLf &
                            "WHERE LibraryFowstBinProduct.BinID = " & ReadyString(pBinID) & vbCrLf &
                            " AND LibraryFowstBinProduct.FowstID = " & ReadyString(pFowstID)

                    pDTBinProducts = Common.WebRequest.ProData.Execute(pSql)

                    For j As Integer = 0 To pDTBinProducts.Rows.Count - 1
                        'set bin balance to 0
                        pProductID = CStr2(pDTBinProducts.Rows(j).Item("ProductID"))
                        AddToInventoryLedger(Guid.NewGuid.ToString, pProductID, 0, "N/A", pFowstID, pBinID)

                        pSql = "Update LibraryFowstBinProduct set IsActive=0 where ID = " & ReadyString(CStr2(pDTBinProducts.Rows(j).Item("ID")))
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    Next
                Next
            Else

                pBinID = CStr2(Common.WebRequest.Data("BinID"))

                pSql = "Update LibraryFowstBin set IsActive = 0 where ID = " & ReadyString(pBinID)
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                pSql = "Select ID, ProductID from LibraryFowstBinProduct" & vbCrLf &
                        "WHERE LibraryFowstBinProduct.BinID = " & ReadyString(pBinID) & vbCrLf &
                        " AND LibraryFowstBinProduct.FowstID = " & ReadyString(pFowstID)

                pDTBinProducts = Common.WebRequest.ProData.Execute(pSql)

                For i As Integer = 0 To pDTBinProducts.Rows.Count - 1
                    'set bin balance to 0
                    pProductID = CStr2(pDTBinProducts.Rows(i).Item("ProductID"))
                    AddToInventoryLedger(Guid.NewGuid.ToString, pProductID, 0, "N/A", pFowstID, pBinID)

                    pSql = "Update LibraryFowstBinProduct set IsActive=0 where ID = " & ReadyString(CStr2(pDTBinProducts.Rows(i).Item("ID")))
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                Next
            End If

            Return JsonDictionary.Serialize("Results", True)

        End Function

        Function DeactivateBinsInArea() As String
            Dim pSql As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "Update LibraryFowstBin set IsActive = 0 where ID IN( " & vbCrLf &
                    "Select LibraryFowstBin.ID from LibraryFowstBin inner join LibraryFowstArea on LibraryFowstArea.ID = LibraryFowstBin.AreaID " & vbCrLf &
                    "WHERE AreaName = " & ReadyString(Common.WebRequest.Data("AreaName")) & ")"

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Return JsonDictionary.Serialize("Results", True)
        End Function

        Function BinInfo() As ActionResult
            Return View()
        End Function

        Function AssignBinToStock() As ActionResult
            Return View()
        End Function

        <HttpPost()>
        Function AddToInventoryLedger(ByVal TransactionID As String,
                                      ByVal ProductID As String,
                                      ByVal Total As Double,
                                      ByVal ExpDate As String,
                                      ByVal FowstID As String,
                                      ByVal BinID As String)
            Dim EDate As Date?

            If Not ExpDate = "N/A" Then
                EDate = CDate(ExpDate)
            End If

            WebRequest.Instance.Business.TransInventoryLedger.Add( _
                            ProductID, _
                            FowstID, _
                            TransactionID, _
                            TransInventoryLedger.eTransType.InventoryCount, _
                            WebSession.Instance.PeopleUserID, , _
                            Total, _
                            BinID,
                            EDate
                        )
        End Function
    End Class
End Namespace