Public Class TableViewController
    Inherits System.Web.Mvc.Controller

    '
    ' GET: /TableView

    Function Index() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()
    End Function

    Function Index2() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()
    End Function

    Function NewTable() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()
    End Function

    Function BsNewTable() As ActionResult
        Return View()
    End Function

    Function Settings() As ActionResult
        Return View()
    End Function

    Function RemoveTemplate() As String
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pSql As String
        Dim temID As String = Common.WebRequest.Data("ID")

        Try
            pSql = "DELETE FROM LibraryTableTemplate WHERE ID = " & ReadyString(temID)

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "DELETE FROM LibraryTableFowst WHERE TemplateID = " & ReadyString(temID)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "DELETE FROM LibraryTableTableInfo WHERE DiningAreaID IN (SELECT ID FROM LibraryTableDiningArea WHERE TemplateID = " & ReadyString(temID) & ")"
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "DELETE FROM LibraryTableDiningArea WHERE TemplateID = " & ReadyString(temID)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

            Return JsonDictionary.Serialize("Results", "removed")
        Catch ex As Exception
            Return JsonDictionary.Serialize("Results", ex.Message)
        End Try
    End Function

    Function SaveTemplate() As String
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pSql As String
        Dim pDT As DataTable
        Dim strReturn As String
        Dim TempIsNew As String = Common.WebRequest.Data("State")
        Dim templatename As String = Common.WebRequest.Data("TemplateName")
        Dim objLocation As Object = Common.WebRequest.AllData("Location")
        Dim objDine As Object = Common.WebRequest.AllData("Dine")
        Dim p As New InsertUpdate("LibraryTableTemplate")
        Dim tempID As String = Guid.NewGuid().ToString

        Try
            pSql = "SELECT * FROM LibraryTableTemplate WHERE Name = " & ReadyString(templatename)
            pDT = Common.WebRequest.ProData.Execute(pSql)

            If pDT.Rows.Count > 0 And TempIsNew = True Then
                Return JsonDictionary.Serialize("Results", "Template named <b>" & templatename & "</b> already exist. Please change the template name.")
            End If

            p = New InsertUpdate("LibraryTableTemplate")
            p("Name") = ReadyString(templatename)
            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            p("LastUpdated") = "GETDATE()"

            Common.WebRequest.ProData.BeginTransaction()

            If TempIsNew = True Then
                p("ID") = ReadyString(tempID)
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Else
                tempID = Common.WebRequest.Data("ID")
                pSql = p.GetUpdate(" ID = " & ReadyString(tempID))
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            For Each location As Object In objLocation
                p = New InsertUpdate("LibraryTableFowst")
                p("TemplateID") = ReadyString(tempID)

                If location("State") = "new" Then
                    p("FowstID") = ReadyString(location("ID"))
                    p("ID") = ReadyString(Guid.NewGuid.ToString)
                    pSql = p.GetInsert()
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                ElseIf location("State") = "edit" Then
                    p("FowstID") = ReadyString(location("ID"))
                    pSql = p.GetUpdate(" ID = " & ReadyString(location("TableFowstID")))
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                ElseIf location("State") = "update" Then
                    pSql = p.GetUpdate(" FowstID = " & ReadyString(location("ID")))
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                Else
                    pSql = "DELETE FROM LibraryTableFowst WHERE ID = " & ReadyString(location("TableFowstID"))
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If
            Next location


            For Each dine As Object In objDine
                p = New InsertUpdate("LibraryTableDiningArea")
                Dim dineID As String = Guid.NewGuid.ToString
                p("TemplateID") = ReadyString(tempID)
                p("DiningAreaName") = ReadyString(dine("AreaName"))
                p("OrderNo") = ReadyString(dine("No"))
                p("NoOfTable") = ReadyString(dine("NoOfTable"))
                p("Prefix") = ReadyString(dine("Prefix"))
                'p("InternalPrefix") = ReadyString(templatename.Replace(" ", ""))
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = "GETDATE()"

                If dine("State") = "new" Then
                    p("ID") = ReadyString(dineID)
                    pSql = p.GetInsert()
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                ElseIf dine("State") = "edit" Then
                    dineID = dine("ID")
                    pSql = p.GetUpdate(" ID = " & ReadyString(dine("ID")))
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                Else
                    pSql = "DELETE FROM LibraryTableDiningArea WHERE ID = " & ReadyString(dine("ID"))
                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    pSql = "DELETE FROM LibraryTableTableInfo WHERE DiningAreaID = " & ReadyString(dine("ID"))
                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                End If

                If dine("State") = "new" Or dine("State") = "edit" Then
                    For Each tbl As Object In dine("Table")
                        p = New InsertUpdate("LibraryTableTableInfo")
                        p("DiningAreaID") = ReadyString(dineID)
                        p("TableName") = ReadyString(tbl("Name"))
                        p("OrderNo") = ReadyString(tbl("No"))
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        p("LastUpdated") = "GETDATE()"

                        If tbl("State") = "new" Then
                            p("InternalTableName") = ReadyString(CheckInternalTableName(templatename.Replace(" ", "") & "-" & dine("AreaName").Replace(" ", "") & "-" & tbl("No"), ""))
                            p("ID") = ReadyString(Guid.NewGuid.ToString)
                            pSql = p.GetInsert()
                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                        ElseIf tbl("State") = "edit" Then
                            p("InternalTableName") = ReadyString(CheckInternalTableName(templatename.Replace(" ", "") & "-" & dine("AreaName").Replace(" ", "") & "-" & tbl("No"), tbl("ID").ToString()))
                            pSql = p.GetUpdate(" ID = " & ReadyString(tbl("ID")))
                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                        Else
                            pSql = "DELETE FROM LibraryTableTableInfo WHERE ID = " & ReadyString(tbl("ID"))
                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                        End If
                    Next tbl
                End If

            Next dine

            pSql = "SELECT * FROM TablesLastUpdated WHERE TableName = 'LibraryTableTemplate'"
            pDT = Common.WebRequest.ProData.Execute(pSql)
            If pDT.Rows.Count > 0 Then
                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryTableTemplate'"
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Else
                pSql = "INSERT INTO TablesLastUpdated VALUES (NEWID(), 'LibraryTableTemplate', NULL, NULL)"
                Common.WebRequest.ProData.ExecuteSQL(pSql)
                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryTableTemplate'"
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            Common.WebRequest.ProData.CommitTransaction()

            Return JsonDictionary.Serialize("Results", "saved")
        Catch ex As Exception
            AppendToDebug("vbVFKX7KMU6D!: " & ex.Message, True, ex, True)
            Return JsonDictionary.Serialize("Results", "<b>Error: </b>" & ex.Message & " ref: vbVFKX7KMU6D!")
        End Try

    End Function

    Function CheckInternalTableName(ByVal name As String, ByVal id As String, Optional ByVal ctr As Integer = 0) As String
        Dim pSql As String = "SELECT ID FROM LibraryTableTableInfo WHERE InternalTableName = " & ReadyString(name) & vbCrLf &
                            IIf(id = "", "", " AND ID <> " & ReadyString(id.ToString()))

        If Common.WebRequest.ProData.Execute(pSql).Rows.Count > 0 Then
            ctr += 1
            Return CheckInternalTableName(name & "_" & ctr, id, ctr)
        Else
            Return name
        End If
    End Function

    Function GetTemplateData() As Object
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pSql As String
        Dim pDTTemplate As DataTable
        Dim pDTLocation As DataTable
        Dim pDTDine As DataTable
        Dim pDTTable As DataTable
        Dim tempID As String = Common.WebRequest.Data("ID")
        Dim arrTemplate(0) As Object

        pSql = "SELECT ID, Name FROM LibraryTableTemplate WHERE ID = " & ReadyString(tempID)
        pDTTemplate = Common.WebRequest.ProData.Execute(pSql)

        If pDTTemplate.Rows.Count > 0 Then
            pSql = "SELECT tempFowst.ID as TableFowstID, fowst.ID as ID, fowst.Name as FowstName FROM LibraryTableFowst as tempFowst INNER JOIN MobizLibraryOrganizationFowst as fowst ON fowst.ID = tempFowst.FowstID WHERE TemplateID = " & ReadyString(tempID)
            pDTLocation = Common.WebRequest.ProData.Execute(pSql)

            Dim arrLocation(pDTLocation.Rows.Count - 1) As Object

            If pDTLocation.Rows.Count > 0 Then
                For l As Integer = 0 To pDTLocation.Rows.Count - 1
                    arrLocation(l) = New With {Key .TableFowstID = pDTLocation.Rows(l)("TableFowstID"),
                                                    .ID = pDTLocation.Rows(l)("ID"),
                                                    .FowstName = pDTLocation.Rows(l)("FowstName"),
                                                    .IsNew = False,
                                                    .State = "edit"}
                Next
            End If

            pSql = "SELECT * FROM LibraryTableDiningArea WHERE TemplateID = " & ReadyString(tempID) & " ORDER BY OrderNo ASC"
            pDTDine = Common.WebRequest.ProData.Execute(pSql)

            Dim arrDine(pDTDine.Rows.Count - 1) As Object

            If pDTDine.Rows.Count > 0 Then
                For i As Integer = 0 To pDTDine.Rows.Count - 1
                    Dim dine As Object = pDTDine.Rows(i)
                    pSql = "SELECT * FROM LibraryTableTableInfo WHERE DiningAreaID = " & ReadyString(dine("ID")) & " ORDER BY OrderNo ASC"
                    pDTTable = Common.WebRequest.ProData.Execute(pSql)

                    Dim arrTable(pDTTable.Rows.Count - 1) As Object

                    If pDTTable.Rows.Count > 0 Then
                        For t As Integer = 0 To pDTTable.Rows.Count - 1
                            arrTable(t) = New With {Key .ID = pDTTable.Rows(t)("ID"),
                                                    .Name = pDTTable.Rows(t)("TableName"),
                                                    .No = pDTTable.Rows(t)("OrderNo"),
                                                    .State = "edit"}
                        Next
                    End If

                    arrDine(i) = New With {Key .ID = dine("ID"),
                                                .No = dine("OrderNo"),
                                                .AreaName = dine("DiningAreaName"),
                                                .NoOfTable = dine("NoOfTable"),
                                                .Prefix = dine("Prefix"),
                                                .Table = arrTable,
                                                .State = "edit"}
                Next
            End If

            arrTemplate(0) = New With {Key .ID = pDTTemplate.Rows(0)("ID"),
                                            .TemplateName = pDTTemplate.Rows(0)("Name"),
                                            .Location = arrLocation,
                                            .Dine = arrDine,
                                            .State = "edit"}
        End If

        Return Json(arrTemplate)

    End Function

    Function GetAllFowst() As String
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT DISTINCT fowst.ID AS [ID] " & vbCrLf &
                ", fowst.Name AS [FowstName] " & vbCrLf &
                ", ISNULL((SELECT TOP 1 1 FROM LibraryTableFowst WHERE FowstID = fowst.ID), 0) as IsAssigned " & vbCrLf &
                "FROM dbo.MobizLibraryOrganizationFOWST fowst " & vbCrLf &
                "WHERE fowst.OrganizationID = " & ReadyString(WebRequest.Instance.Data("OrgID")) & vbCrLf &
                " AND fowst.IsActive = 1 AND FOWSTTypeID <> 'dff0f087-4c2b-4b67-aa1d-502f02773e2b' -- dff0f087-4c2b-4b67-aa1d-502f02773e2b kani na FOWST Type ID, only used in Mobiz Project " & vbCrLf &
                "ORDER BY FowstName"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return JsonDictionary.Serialize("Results", pDT)
    End Function

    Function GetUnassignedFowst() As String
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT DISTINCT fowst.ID AS [ID] " & vbCrLf &
                ", fowst.Name AS [FowstName] " & vbCrLf &
                ", 1 as IsNew, 'new' as State " & vbCrLf &
                "FROM dbo.MobizLibraryOrganizationFOWST fowst " & vbCrLf &
                "WHERE fowst.OrganizationID = " & ReadyString(WebRequest.Instance.Data("OrgID")) & vbCrLf &
                " AND fowst.ID NOT IN (SELECT FowstID FROM LibraryTableFowst) AND fowst.IsActive = 1 " & vbCrLf &
                " AND FOWSTTypeID <> 'dff0f087-4c2b-4b67-aa1d-502f02773e2b' -- dff0f087-4c2b-4b67-aa1d-502f02773e2b kani na FOWST Type ID, only used in Mobiz Project " & vbCrLf &
                "ORDER BY FowstName"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return JsonDictionary.Serialize("Results", pDT)
    End Function

    <HttpPost()>
    Function CheckUniqueTableViewTemplateName() As String
        Dim pSql As String
        Dim pDT As DataTable
        Dim strReturn As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pSql = "SELECT SectionName" & vbCrLf &
            " FROM LibraryTableViewTemplate WHERE SectionName = " & ReadyString(Common.WebRequest.Data("SectionName")) & " AND FowstID = " & ReadyString(Common.WebRequest.Data("FowstID"))
        pDT = Common.WebRequest.ProData.Execute(pSql)


        'return
        strReturn = JsonDictionary.Serialize("TemplateData", pDT)

        Return strReturn

    End Function

    <HttpPost()>
    Public Function SaveTableViewSettings() As String
        Dim pSql As String
        Dim pDT As DataTable
        Dim pIU As InsertUpdate
        Dim pID As String
        Dim templateID As String
        Dim pOut As JsonDictionary
        Dim objectDataList As Object()
        Dim arrObject As Array
        Dim lastTableCount As Integer

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        Try

            pOut = New JsonDictionary
            pIU = New InsertUpdate("LibraryTableViewTemplate")
            pIU("SectionName") = ReadyString(Common.WebRequest.Data("SectionName"))
            pIU("FowstID") = ReadyString(Common.WebRequest.Data("FowstID"))
            pIU("FloorArea") = Convert.ToInt16(Common.WebRequest.Data("FloorArea"))
            pIU("IsActive") = Common.WebRequest.Data("IsActive")
            pIU("LastUpdated") = "GETDATE()"
            pIU("UserID") = ReadyString(WebSession.Instance.PeopleUserID)

            If (Common.WebRequest.Data("ID").Trim.Length > 0) Then
                pSql = pIU.GetUpdate(" ID = " & ReadyString(Common.WebRequest.Data("ID")))
                templateID = ""
            Else
                pID = Guid.NewGuid.ToString
                pIU("ID") = ReadyString(pID)
                pSql = pIU.GetInsert
                templateID = pID
            End If

            Common.WebRequest.ProData.BeginTransaction()

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            'get last table count
            pSql = "SELECT TOP 1 TableName" & vbCrLf &
           " FROM LibraryTableViewLayout WHERE TableViewTemplateID = " & ReadyString(templateID) & " ORDER BY LastUpdated Desc"
            pDT = Common.WebRequest.ProData.Execute(pSql)
            If (pDT.Rows.Count > 0) Then
                arrObject = pDT.Rows(0).Item("TableName").ToString().Split("_")
                lastTableCount = arrObject(1)
            Else
                lastTableCount = 1
            End If

            
            pIU = New InsertUpdate("LibraryTableViewLayout")
            If (Common.WebRequest.Data("ID").Trim.Length > 0) Then

                'pSql = pIU.GetUpdate(" TableViewTemplateID = " & ReadyString(pDT.Rows(0).Item("ID")))
            Else

                '''Save object data
                objectDataList = Common.WebRequest.AllData("ObjectDataList")
                For Each objectDataRecord As Object In objectDataList

                    'pIU("ObjectHtmlID") = ReadyString(objectDataRecord("ObjectHtmlID"))
                    pIU("PositionTop") = Convert.ToDecimal(objectDataRecord("PositionTop"))
                    pIU("PositionLeft") = Convert.ToDecimal(objectDataRecord("PositionLeft"))
                    pIU("ImageWidth") = Convert.ToDecimal(objectDataRecord("ImageWidth"))
                    pIU("ImageHeight") = Convert.ToDecimal(objectDataRecord("ImageHeight"))

                    arrObject = objectDataRecord("ObjectHtmlID").ToString().Split("_")
                    pIU("ObjectType") = ReadyString(arrObject(0))

                    pIU("TableName") = ReadyString("Table_ " & lastTableCount)
                    pIU("LastUpdated") = "GETDATE()"
                    'if not 0, save it. if zero just leave it null
                    If arrObject(1) = "0" Then
                        pIU("DefaultSeatNo") = "NULL"
                    Else
                        pIU("DefaultSeatNo") = Convert.ToInt16(arrObject(1))
                    End If


                    If (templateID.Length > 0) Then
                        pIU("TableViewTemplateID") = ReadyString(templateID)
                    End If

                    pID = Guid.NewGuid.ToString
                    pIU("ID") = ReadyString(pID)
                    pSql = pIU.GetInsert

                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    lastTableCount = lastTableCount + 1
                Next

            End If

            Common.WebRequest.ProData.CommitTransaction()


            pOut.Add("Result", "ok")
            Return pOut.Serialize()

        Catch ex As System.Exception
            Common.WebRequest.ProData.RollbackTransaction()

            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    <HttpPost()>
    Function GetTableLayout() As String
        Dim strReturn As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pDT As DataTable
        Dim pSql As String

        If (Common.WebRequest.Data("SearchKeyword") Is Nothing) Then
            pSql = "SELECT tbl.ID" &
            ",tbl.SectionName, tbl.LastUpdated, ppl.FirstName, ppl.MiddleName, ppl.Surname" & vbCrLf &
            " FROM LibraryTableViewTemplate tbl INNER JOIN MobizLibraryPeople ppl" & vbCrLf &
            " ON tbl.UserID = ppl.ID WHERE tbl.IsActive = 1"
        Else
            pSql = "SELECT tbl.ID" &
            ",tbl.SectionName, tbl.LastUpdated, ppl.FirstName, ppl.MiddleName, ppl.Surname" & vbCrLf &
            " FROM LibraryTableViewTemplate tbl INNER JOIN MobizLibraryPeople ppl" & vbCrLf &
            " ON tbl.UserID = ppl.ID WHERE tbl.IsActive = 1 AND tbl.SectionName LIKE " & ReadyStringWild(Common.WebRequest.Data("SearchKeyword"))
        End If

        pSql &= " ORDER BY tbl.SectionName"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        strReturn = JsonDictionary.Serialize("TableLayoutData", pDT)

        Return strReturn

    End Function

    <HttpPost()>
    Function GetPlanogramLayout() As String
        Dim strReturn As String

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pDT As DataTable
        Dim pSql As String


        pSql = "SELECT PositionTop, PositionLeft,ImageWidth,ImageHeight, ObjectType, TableName, DefaultSeatNo, TableViewTemplateID " &
        " FROM LibraryTableViewLayout "

        pDT = Common.WebRequest.ProData.Execute(pSql)

        strReturn = JsonDictionary.Serialize("TableLayoutData", pDT)

        Return strReturn

    End Function

    <HttpPost>
    Public Function GetDiningTables() As String

        Dim pSql As String
        Dim pDT As DataTable
        Dim pDiningID As String = Common.WebRequest.Data("DiningID")

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pSql = "  SELECT *" & vbCrLf &
            "  ,(SELECT TOP 1 1" & vbCrLf &
            "      FROM TransInvoice" & vbCrLf &
            "      WHERE CustomerTableNo = 'TBL_' + LibraryTableTableInfo .InternalTableName" & vbCrLf &
            "      AND ISNULL(TransInvoice.InvoiceNumber, 0) = 0" & vbCrLf &
            "      AND EXISTS (" & vbCrLf &
            "            SELECT *" & vbCrLf &
            "            FROM TransInvoiceItems" & vbCrLf &
            "            WHERE InvoiceID = TransInvoice.ID" & vbCrLf &
            "            AND (Quantity > 0 OR OrderedQuantity > 0)" & vbCrLf &
            "       )" & vbCrLf &
            "  ) AS IsOccupied" & vbCrLf &
            "  , (SELECT IsParent FROM LibraryTableJoined WHERE TableID = LibraryTableTableInfo.ID) as IsParent" & vbCrLf &
            "  FROM LibraryTableTableInfo " & vbCrLf &
            "  WHERE DiningAreaID = " & ReadyString(pDiningID) & vbCrLf &
            "  ORDER BY OrderNo ASC"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return JsonDictionary.Serialize(pDT)

    End Function
End Class