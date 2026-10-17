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
End Class