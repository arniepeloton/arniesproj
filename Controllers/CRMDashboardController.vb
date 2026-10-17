Imports Aspose
Imports Aspose.Cells

Namespace WebProj
    Public Class CRMDashboardController
        Inherits System.Web.Mvc.Controller

        Dim strUploadContactStatus As String = "Uploading ..."

        Function CheckIfCustomerSurveyExist()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql = "SELECT ID FROM LibrarySurveyRespondent WHERE ClientID = " & ReadyString(Common.WebRequest.Data("ClientID")) & vbCrLf &
                        " AND QuestionaireID = " & ReadyString(Common.WebRequest.Data("QID"))

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function Index() As ActionResult
            Return View()
        End Function

        Function Settings() As ActionResult
            Return View()
        End Function

        Function GeneralSettings() As ActionResult
            Return View()
        End Function

        Function DB() As ActionResult
            'DB or dashboard
            'mao na ni ang lead gen page :)
            Return View()
        End Function

        Function SalesGrinder() As ActionResult
            Return View()
        End Function

        Function ActivityReport() As ActionResult
            Return View()
        End Function

        Function ActivityQuota() As ActionResult
            Return View()
        End Function

        Function TicketNotes() As ActionResult
            Return View()
        End Function

        Function ClientStage() As ActionResult
            Return View()
        End Function

        Function EmploymentConfirmation() As ActionResult
            Return View()
        End Function

        Function SystemAdministration() As ActionResult
            Return View()
        End Function

        Function FunnelTemplate() As ActionResult
            Return View()
        End Function

        Function Opportunity() As ActionResult
            Return View()
        End Function

        Function Tickets() As ActionResult
            Return View()
        End Function

        Function TodoCategoriesSettings() As ActionResult
            Return View()
        End Function

        Function MarkTodo()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim p As New InsertUpdate("TransCRM")

                p("ToDoFinish") = CInt2(Common.WebRequest.Data("Mark"))

                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(Common.WebRequest.Data("TodoID"))))

                Return JsonDictionary.Serialize("Results", "done")
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function TodoDashboard()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String
                Dim CreatedBy As String = CStr2(Common.WebRequest.QueryString("CreatedBy"))
                Dim RSCreatedBy As String = RemoveSymbols(CreatedBy)
                Dim AssignedTo As String = CStr2(Common.WebRequest.QueryString("AssignedTo"))
                Dim Category As String = CStr2(Common.WebRequest.QueryString("Category"))
                Dim DueDate As String = CStr2(Common.WebRequest.QueryString("DueDate"))
                Dim Status As String = CStr2(Common.WebRequest.QueryString("Status"))
                Dim Urgency As String = CStr2(Common.WebRequest.QueryString("Urgency"))
                Dim Importance As String = CStr2(Common.WebRequest.QueryString("Importance"))
                Dim keyword As String = CStr2(Common.WebRequest.QueryString("keywords"))
                Dim PageStart As Integer = CInt(Request.Form.GetValues("start")(0))
                Dim PageNoOfRows As Integer = CInt(Request.Form.GetValues("length")(0))
                Dim whereSearchContent As String = ""
                Dim operators As String = ""
                Dim arr As Object = Split(keyword, "+")

                Dim mainquery As String = " SELECT crm.ID, crm.ClientID, crm.ToDoTitle, crm.ToDo, crm.ToDoFinish, crm.ToDoDueDate, crm.TodoCategoryID, " & vbCrLf &
                        " crm.TodoNumber, crm.ToDoOrder, crm.CreatedOn, crm.LastUpdated, crm.Urgency, crm.Importance," & vbCrLf &
                        " ppol.Surname + ', ' + ppol.FirstName as CreatedByName, " & vbCrLf &
                        " modified.FirstName + ' ' + modified.Surname as LastUpdatedByName, " & vbCrLf &
                        " cat.CategoryName, ISNULL(org.CompanyName, org.PersonFirstName + ' ' + org.PersonSurname) as OrgName, " & vbCrLf &
                        " STUFF((SELECT ', ' + ppol.FirstName + ' ' + ppol.Surname " & vbCrLf &
                        "       FROM TransCRMTodoAssignedEmployee as emp " & vbCrLf &
                        "       INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = emp.EmployeeID " & vbCrLf &
                        "       WHERE emp.TodoID = crm.ID " & vbCrLf &
                        "   FOR XML PATH('') " & vbCrLf &
                        " ), 1, 1,'') as AssignedPersons, " & vbCrLf &
                        " ROW_NUMBER() OVER (ORDER BY crm.ToDoDueDate ASC) AS RowNum" & vbCrLf &
                        " FROM TransCRM as crm " & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganization as org ON org.ID = crm.ClientID " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as ppol ON ppol.ID = crm.CreatedBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as modified ON modified.ID = crm.LastUpdatedBy " & vbCrLf &
                        " LEFT JOIN LibraryCRMTodoCategories as cat ON cat.ID = crm.ToDoCategoryID " & vbCrLf &
                        " WHERE crm.ActionName = 'To-Do' " & vbCrLf &
                        " AND crm.IsDelete = 0 " & vbCrLf &
                        " AND crm.ToDoFinish = " & CInt2(Status) & vbCrLf

                If Not String.IsNullOrWhiteSpace(Urgency) Then
                    mainquery += " AND crm.Urgency = " & ReadyString(Urgency)
                End If

                If Not String.IsNullOrWhiteSpace(Importance) Then
                    mainquery += " AND crm.Importance = " & ReadyString(Importance)
                End If

                If IsGuid(CreatedBy) Then
                    mainquery += " AND crm.CreatedBy = " & ReadyString(CreatedBy) & vbCrLf
                End If

                If IsGuid(AssignedTo) Then
                    mainquery += "   AND crm.ID IN (SELECT TodoID as ID " & vbCrLf &
                        "       FROM TransCRMTodoAssignedEmployee " & vbCrLf &
                        "       WHERE EmployeeID = " & ReadyString(AssignedTo) & vbCrLf &
                        "   ) " & vbCrLf
                End If

                If Not String.IsNullOrWhiteSpace(Category) Then
                    If Category = "uncategorized" Then
                        mainquery += " AND crm.ToDoCategoryID IS NULL " & vbCrLf
                    Else
                        mainquery += " AND crm.ToDoCategoryID = " & ReadyString(Category) & vbCrLf
                    End If
                End If

                If Not String.IsNullOrWhiteSpace(DueDate) Then
                    Dim daterange As Object = Split(DueDate, "+to+")
                    mainquery += " AND CONVERT(date, crm.ToDoDueDate) BETWEEN " & ReadyString(daterange(0)) & " AND " & ReadyString(daterange(1)) & vbCrLf
                End If

                If CStr2(keyword) <> "" Then
                    mainquery += " AND ("
                    For i As Integer = 0 To arr.length - 1
                        If whereSearchContent <> "" Then
                            operators = " AND "
                        End If

                        whereSearchContent = whereSearchContent & operators & vbCrLf &
                             " ISNULL(crm.TodoNumber, '') + ' ' + " & vbCrLf &
                             " ISNULL(crm.ToDo, '') LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                    Next

                    mainquery += whereSearchContent & ") "
                End If

                pSql = "SELECT * FROM (" & vbCrLf &
                                mainquery & vbCrLf &
                                " ) as t WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows)) & vbCrLf &
                                " ORDER BY RowNum ASC "

                Dim mainpDT As DataTable = Common.WebRequest.ProData.Execute(pSql)
                Dim pDTCountAll As DataTable = Common.WebRequest.ProData.Execute(mainquery)

                Return Json(New With {Key .data = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(mainpDT)), .recordsTotal = pDTCountAll.Rows().Count, .recordsFiltered = pDTCountAll.Rows().Count}, JsonRequestBehavior.AllowGet)
                'Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetTodo()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim status As String = CStr2(Common.WebRequest.Data("Status"))
            Dim todoid As String = CStr2(Common.WebRequest.Data("ToDoID"))
            Dim keywords As String = CStr2(Common.WebRequest.Data("keywords"))
            Dim pSql As String = " SELECT crm.*, " & vbCrLf &
                                " ISNULL(client.CompanyName, client.PersonFirstName + ' ' + client.PersonSurname) as ClientName, " & vbCrLf &
                                " createdby.FirstName + ' ' + createdby.Surname as CreatedByName, " & vbCrLf &
                                " updatedby.FirstName + ' ' + updatedby.Surname as LastUpdatedByName, " & vbCrLf &
                                " cat.CategoryName " & vbCrLf &
                                " FROM TransCRM as crm " & vbCrLf &
                                " LEFT JOIN MobizLibraryOrganization as client ON client.ID = crm.ClientID " & vbCrLf &
                                " LEFT JOIN MobizLibraryPeople as createdby ON createdby.ID = crm.CreatedBy " & vbCrLf &
                                " LEFT JOIN MobizLibraryPeople as updatedby ON updatedby.ID = crm.LastUpdatedBy " & vbCrLf &
                                " LEFT JOIN LibraryCRMTodoCategories as cat ON cat.ID = crm.ToDoCategoryID " & vbCrLf &
                                " WHERE ActionName = 'To-Do' AND IsDelete = 0 " & vbCrLf &
                                " AND ToDoFinish = " & IIf(status = "todo", 0, 1) & vbCrLf

            If Not String.IsNullOrWhiteSpace(todoid) Then
                pSql += " AND crm.ID = " & ReadyString(todoid)
            End If

            If Not String.IsNullOrWhiteSpace(keywords) Then
                pSql += " AND (ISNULL(crm.Todo, '') + ISNULL(crm.TodoNumber, '') LIKE " & ReadyStringWild(keywords) & ")"
            End If

            pSql += " ORDER BY crm.LastUpdated DESC "

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function GetTodoCategories()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String
                Dim keyword As String = CStr2(Common.WebRequest.QueryString("keywords"))
                Dim modules As String = CStr2(Common.WebRequest.QueryString("module"))
                Dim PageStart As Integer = CInt(Request.Form.GetValues("start")(0))
                Dim PageNoOfRows As Integer = CInt(Request.Form.GetValues("length")(0))
                Dim whereSearchContent As String = ""
                Dim operators As String = ""
                Dim arr As Object = Split(keyword, "+")

                Dim mainquery As String = " SELECT cat.*, " & vbCrLf &
                        " modified.FirstName + ' ' + modified.Surname as LastUpdatedBy, " & vbCrLf &
                        " ROW_NUMBER() OVER (ORDER BY cat.CategoryName ASC) AS RowNum" & vbCrLf &
                        " FROM LibraryCRMTodoCategories as cat " & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as modified ON modified.ID = cat.UpdatedBy " & vbCrLf

                If CStr2(keyword) <> "" Then
                    mainquery += " AND ("
                    For i As Integer = 0 To arr.length - 1
                        If whereSearchContent <> "" Then
                            operators = " AND "
                        End If

                        whereSearchContent = whereSearchContent & operators & vbCrLf &
                             " ISNULL(cat.CategoryName, '') LIKE " & ReadyStringWild(arr(i)) & vbCrLf
                    Next

                    mainquery += whereSearchContent & ") "
                End If

                pSql = "SELECT * FROM (" & vbCrLf &
                                mainquery & vbCrLf &
                                " ) as t WHERE RowNum >= " & If(PageStart = 0, 1, (PageStart + 1)) & "  AND RowNum <= " & If(PageStart = 0, PageNoOfRows, (PageStart + PageNoOfRows)) & vbCrLf &
                                " ORDER BY RowNum ASC "

                Dim mainpDT As DataTable = Common.WebRequest.ProData.Execute(pSql)
                Dim pDTCountAll As DataTable = Common.WebRequest.ProData.Execute(mainquery)

                Return Json(New With {Key .data = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(mainpDT)), .recordsTotal = pDTCountAll.Rows().Count, .recordsFiltered = pDTCountAll.Rows().Count}, JsonRequestBehavior.AllowGet)
                'Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function SaveTodoCategory()
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim p As New InsertUpdate("LibraryCRMTodoCategories")
                Dim mods As String = CStr2(Common.WebRequest.Data("Module"))

                p("CategoryName") = ReadyString(Common.WebRequest.Data("CategoryName"))
                p("IsActive") = 1
                p("Module") = IIf(String.IsNullOrWhiteSpace(mods), "NULL", ReadyString(mods))
                p("UpdatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = "getdate()"

                If String.IsNullOrWhiteSpace(Common.WebRequest.Data("CategoryID")) Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Else
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(Common.WebRequest.Data("CategoryID"))))
                End If

                Return JsonDictionary.Serialize("Result", "saved")
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function GetClientGroupFOWST()
            Try
                Dim clientid As String = Common.WebRequest.Data("ClientID")
                Dim db As DataRow = WebRequest.Instance.Business.LibraryOrganization.ByID(clientid)

                If String.IsNullOrWhiteSpace(db("Database").ToString()) Then
                    Return JsonDictionary.Serialize("Invalid", "not a mobiz subscriber")
                End If

                Common.WebRequest.ProData.Execute("USE " & EscapeSqlName(db("Database")))

                Dim pSql As String = " SELECT ID, GroupName, ISNULL(IsFranchisee, 0) as IsFranchisee FROM LibraryFranchiseGroup "

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function BrowseClientsFOWST()
            Try
                Dim clientid As String = Common.WebRequest.Data("ClientID")
                Dim pSql As String = "SELECT ID, Name FROM LibraryOrganizationFOWST WHERE OrganizationID = " & ReadyString(clientid)

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function TagTransAsFollowUp()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pIU As InsertUpdate
                Dim pSql As String

                pIU = New InsertUpdate("LibraryOrganizationCustomer")
                pIU("IsForFollowUp") = 1
                pSql = pIU.GetUpdate("OrganizationID = " & ReadyString(Common.WebRequest.Data("CustomerID")))
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                Return JsonDictionary.Serialize("Results", "ok")
            Catch ex As Exception

            End Try
        End Function

        Function MarkTransAsFollowedUp()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pIU As InsertUpdate
                pIU = New InsertUpdate("LibraryOrganizationCustomer")
                pIU("IsForFollowUp") = 0
                Dim pSql = pIU.GetUpdate("OrganizationID = " & ReadyString(Common.WebRequest.Data("CustomerID")))
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                Return JsonDictionary.Serialize("Results", "ok")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", "error")
            End Try
        End Function

        Function GetOrderHistory() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim lastweek As String = DateRange.GetYearWeek(Now, -1)
                Dim selectedweek As String = DateRange.GetYearWeek(Now, -CInt2(Request.Params.Get("selectedweek")))
                Dim pSql As String = " SELECT MobizLibraryOrganization.CompanyName " & vbCrLf &
                                    " ,tblSalesOrder.ProductID,tblSalesOrder.OrderQty,tblSalesOrder.InvoiceQty,MobizLibraryPIMProduct.ProductNo " & vbCrLf &
                                    " ,MobizLibraryPIMProduct.Brand + ' ' + MobizLibraryPIMProduct.ProductName AS ProductDescription " & vbCrLf &
                                    " FROM MobizLibraryOrganization " & vbCrLf &
                                    " JOIN LibraryOrganizationCustomer ON LibraryOrganizationCustomer.OrganizationID = MobizLibraryOrganization.ID " & vbCrLf &
                                    " OUTER APPLY ( " & vbCrLf &
                                    "   SELECT ProductID, SUM(OrderQty) AS OrderQty, SUM(InvoiceQty) AS InvoiceQty " & vbCrLf &
                                    "   FROM ( " & vbCrLf &
                                    "       SELECT TransInvoiceItems.ProductID " & vbCrLf &
                                    "       ,SUM(TransInvoiceItems.OrderedQuantity * TransInvoiceItems.ActualQuantity) AS OrderQty " & vbCrLf &
                                    "       ,null AS InvoiceQty " & vbCrLf &
                                    "       FROM TransInvoice " & vbCrLf &
                                    "       JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID " & vbCrLf &
                                    "       WHERE TransInvoice.TransType = 'SalesOrder' " & vbCrLf &
                                    "       AND TransInvoice.CustomerID = MobizLibraryOrganization.ID " & vbCrLf &
                                    "       AND TransInvoice.ComputedSalesOrderYearWeek >= " & ReadyString(selectedweek) & " AND TransInvoice.ComputedSalesOrderYearWeek <= " & ReadyString(lastweek) & vbCrLf &
                                    "       GROUP BY TransInvoiceItems.ProductID " & vbCrLf &
                                    "       UNION " & vbCrLf &
                                    "       SELECT TransInvoiceItems.ProductID " & vbCrLf &
                                    "       ,null AS OrderQty " & vbCrLf &
                                    "       ,SUM(TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity) AS InvoiceQty " & vbCrLf &
                                    "       FROM TransInvoice " & vbCrLf &
                                    "       JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID " & vbCrLf &
                                    "       WHERE TransInvoice.TransType = 'Invoice' " & vbCrLf &
                                    "       AND TransInvoice.CustomerID = MobizLibraryOrganization.ID " & vbCrLf &
                                    "       AND TransInvoice.ComputedInvoiceYearWeek >= " & ReadyString(selectedweek) & " AND TransInvoice.ComputedInvoiceYearWeek <= " & ReadyString(lastweek) & vbCrLf &
                                    "       GROUP BY TransInvoiceItems.ProductID " & vbCrLf &
                                    "   ) T GROUP BY ProductID " & vbCrLf &
                                    " ) tblSalesOrder " & vbCrLf &
                                    " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = tblSalesOrder.ProductID " & vbCrLf &
                                    " WHERE MobizLibraryOrganization.ID = " & ReadyString(Request.Params("ID")) & vbCrLf &
                                    " ORDER BY MobizLibraryOrganization.CompanyName, MobizLibraryPIMProduct.brand + ' ' + MobizLibraryPIMProduct.productname " & vbCrLf

                Dim pOut As New JsonDictionary

                pOut.Add("OrderHistory", Common.WebRequest.ProData.Execute(pSql))
                pOut.Add("IsForFollowUp", Common.WebRequest.ProData.Execute("SELECT ISNULL(IsForFollowUp, 0) as IsForFollowUp FROM LibraryOrganizationCustomer WHERE OrganizationID = " & ReadyString(Request.Params("ID"))).Rows(0)("IsForFollowUp"))

                Return pOut.Serialize()
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetOrderHistoryBreakdown() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim lastweek As String = DateRange.GetYearWeek(Now, -1)
                Dim selectedweek As String = DateRange.GetYearWeek(Now, -CInt2(Request.Params.Get("selectedweek")))
                Dim pSql As String = " SELECT DISTINCT TOP 100 PERCENT (CASE MobizLibraryOrganization.IsOrganization WHEN 1 THEN MobizLibraryOrganization.CompanyName ELSE ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeoplePreNominalSuffix.Code)) + ' ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonSurname)) + ', ',', '), '') + ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonFirstName)) + ' ',' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonMiddleName)) + ' ',' '), '') + ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryOrganization.PersonSuffixName)),', '), '') END) AS [CustomerName] " & vbCrLf &
                                    " , TransInvoice.TransType, CASE TransInvoice.TransType " & vbCrLf &
                                    "   WHEN 'Invoice' THEN ISNULL(LibraryDocumentControl.DocumentName, 'SI') + ' ' + CONVERT(varchar, InvFowst.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, TransInvoice.InvoiceNumber) " & vbCrLf &
                                    "   WHEN 'SalesOrder' THEN 'SO ' + CONVERT(varchar, InvFowst.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' + CONVERT(varchar, TransInvoice.SalesOrderNo) " & vbCrLf &
                                    " END AS FullTransNumber " & vbCrLf &
                                    " , TransInvoice.InvoiceDate " & vbCrLf &
                                    " , TransInvoiceItems.InvoiceID " & vbCrLf &
                                    " , MobizLibraryPIMProduct.ProductNo " & vbCrLf &
                                    " , ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPIMProduct.Brand)), '') + ' ', '') + MobizLibraryPIMProduct.ProductName AS [ProductName] " & vbCrLf &
                                    " , TransInvoiceItems.Quantity" & vbCrLf &
                                    " , TransInvoiceItems.OrderedQuantity " & vbCrLf &
                                    " , TransInvoiceItems.UnitMeasure " & vbCrLf &
                                    " , MobizLibraryPIMMeasure.ID AS [UnitMeasureID]" & vbCrLf &
                                    " , TransInvoiceItems.UnitPrice " & vbCrLf &
                                    " , TransInvoiceItems.SubTotal " & vbCrLf &
                                    " , TransInvoiceItems.UnitPrice * TransInvoiceItems.OrderedQuantity AS [OrderedAmount] " & vbCrLf &
                                    " , TransInvoiceItems.UnitPrice * TransInvoiceItems.Quantity AS [ServedAmount] " & vbCrLf &
                                    " , TransInvoiceItems.ActualQuantity " & vbCrLf &
                                    " , TransInvoiceItems.ProductID" & vbCrLf &
                                    " , TransInvoiceItems.PriceLevelID " & vbCrLf &
                                    " , TransInvoiceItems.Cost " & vbCrLf &
                                    " , TransInvoiceItems.LineCost" & vbCrLf &
                                    " , TransInvoiceItems.UnitGP " & vbCrLf &
                                    " , TransInvoiceItems.UnitGPM " & vbCrLf &
                                    " , TransInvoiceItems.LineGP" & vbCrLf &
                                    " FROM TransInvoiceItems " & vbCrLf &
                                    " JOIN TransInvoice ON TransInvoice.ID = TransInvoiceItems.InvoiceID " & vbCrLf &
                                    " LEFT JOIN MobizLibraryPIMProduct ON TransInvoiceItems.ProductID = MobizLibraryPIMProduct.ID " & vbCrLf &
                                    " LEFT JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.Label = TransInvoiceItems.UnitMeasure " & vbCrLf &
                                    " LEFT JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.MeasureNameID = MobizLibraryCountriesLanguageLabel.ID " & vbCrLf &
                                    " OUTER APPLY ( " & vbCrLf &
                                    "   SELECT TOP 1 MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure " & vbCrLf &
                                    "   , MobizLibraryPIMProductMeasurement.Quantity AS UnitQty " & vbCrLf &
                                    "   FROM MobizLibraryPIMProductMeasurement " & vbCrLf &
                                    "   JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID " & vbCrLf &
                                    "   JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID " & vbCrLf &
                                    "   WHERE MobizLibraryPIMProductMeasurement.ProductID = MobizLibraryPIMProduct.ID " & vbCrLf &
                                    "   ORDER BY MobizLibraryPIMProductMeasurement.Quantity " & vbCrLf &
                                    " ) tblBaseUnit " & vbCrLf &
                                    " LEFT JOIN LibraryDocumentControl ON LibraryDocumentControl.ID = TransInvoice.DocumentNameID " & vbCrLf &
                                    " LEFT JOIN MobizLibraryOrganization ON TransInvoice.CustomerID = MobizLibraryOrganization.ID " & vbCrLf &
                                    " LEFT JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID " & vbCrLf &
                                    " LEFT JOIN MobizLibraryOrganizationFOWST InvFowst ON TransInvoice.FowstID = InvFowst.ID " & vbCrLf &
                                    " LEFT JOIN MobizLibraryPeoplePreNominalSuffix MobizLibraryPeoplePreNominalSuffix ON MobizLibraryOrganization.PersonTitleID = MobizLibraryPeoplePreNominalSuffix.ID " & vbCrLf &
                                    " WHERE TransInvoiceItems.Quantity > 0 " & vbCrLf &
                                    " AND (" & vbCrLf &
                                    "   ( TransInvoice.InvoiceNumber > 0 AND TransInvoice.TransType = 'Invoice' AND TransInvoice.ComputedInvoiceYearWeek >= " & ReadyString(selectedweek) & " AND TransInvoice.ComputedInvoiceYearWeek <= " & ReadyString(lastweek) & ") " & vbCrLf &
                                    "   OR " & vbCrLf &
                                    "   ( TransInvoice.SalesOrderNo > 0 AND TransInvoice.TransType = 'SalesOrder' AND TransInvoice.ComputedSalesOrderYearWeek >= " & ReadyString(selectedweek) & " AND TransInvoice.ComputedSalesOrderYearWeek <= " & ReadyString(lastweek) & " ) " & vbCrLf &
                                    " ) " & vbCrLf &
                                    " AND TransInvoice.CustomerID = " & ReadyString(Request.Params("ID")) & vbCrLf &
                                    " AND TransInvoiceItems.ProductID = " & ReadyString(Request.Params("ProductID")) & vbCrLf &
                                    " ORDER BY TransInvoice.InvoiceDate DESC " & vbCrLf

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function DownloadTicket() As String
            'Dim webClient As New WebClient
            Dim source As String = "FileTemplates/" & Request.Params.Get("OrgID") & "/IssueTickets/" & Request.Params.Get("TicketID") & "/" & Request.Params.Get("FileName")
            Dim path As String = Server.MapPath("~/" & source)
            Dim file As System.IO.FileInfo = New System.IO.FileInfo(path)

            If file.Exists Then 'set appropriate headers
                Response.Clear()
                Response.AddHeader("Content-Disposition", "attachment; filename=" & Request.Params.Get("FileName"))
                Response.AddHeader("Content-Length", file.Length.ToString())
                Response.ContentType = "application/octet-stream"
                Response.WriteFile(file.FullName)
                Response.End()
                Return JsonDictionary.Serialize("Results", "Exist")
            Else
                Return JsonDictionary.Serialize("Results", "Not Exist")
            End If

            'webClient.DownloadFile(source & Request.Params.Get("FileName"), Common.ServerPath & "SurveyFiles/" & Request.Params.Get("ClientID") & "/" & Request.Params.Get("DownloadFileName"))
            'webClient.Dispose()
        End Function

        Function ExportClientIssueTickets()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim ClientID As String = Request.Params.Get("ClientID")
                Dim fromdate As String = Request.Params.Get("From")
                Dim todate As String = Request.Params.Get("To")
                Dim Filter As String = Request.Params.Get("Filter")
                Dim pDtExcel As New DataTable
                Dim pSql As String = " SELECT DISTINCT crm.ID AS ID " & vbCrLf &
                                    ", Notes " & vbCrLf &
                                    ", crm.LastUpdated AS [LastUpdated] " & vbCrLf &
                                    ", crm.TicketType AS [Type] " & vbCrLf &
                                    ", crm.OtherTicketType AS [OtherType] " & vbCrLf &
                                    ", crm.TicketNumber AS [No] " & vbCrLf &
                                    ", [Status] " & vbCrLf &
                                    ", crm.AddressToService AS [Address] " & vbCrLf &
                                    ", [ProductServices] " & vbCrLf &
                                    ", [RecommendedAction] " & vbCrLf &
                                    ", [OtherRecommendedAction] " & vbCrLf &
                                    ", crm.DaysOfService AS [Days] " & vbCrLf &
                                    ", crm.TicketDueDate AS [DueDate] " & vbCrLf &
                                    ", CONVERT(datetime, TimeFrom) as TimeFrom " & vbCrLf &
                                    ", CONVERT(datetime, TimeTo) as TimeTo " & vbCrLf &
                                    ", [TicketOpenDate] " & vbCrLf &
                                    ", (SELECT ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname FROM MobizLibraryPeople as ppol WHERE ID = crm.OriginalCreatedBy) AS [OriginalCreatedBy] " & vbCrLf &
                                    ", [ContactPersonID] " & vbCrLf &
                                    ", crm.ContactPersonName AS [ContactPerson] " & vbCrLf &
                                    ", crm.IssueAssignedTo AS [AssignedID] " & vbCrLf &
                                    ", crm.IssueAssignedToName AS [Assigned] " & vbCrLf &
                                    ", (SELECT TOP 1 ID FROM TransCRM WHERE TicketNumber = crm.TicketNumber ORDER BY LastUpdated ASC) AS [ParentTicketID] " & vbCrLf &
                                    ", (SELECT ppol.FirstName + ' ' + ppol.MiddleName + ' ' + ppol.Surname FROM MobizLibraryPeople as ppol WHERE ID = crm.CreatedBy) AS [CreatedBy] " & vbCrLf &
                                    " FROM TransCRM as crm " & vbCrLf &
                                    " INNER JOIN (SELECT MAX(LastUpdated) as LastUpdated, TicketNumber FROM TransCRM WHERE ActionName = 'Ticket' GROUP BY TicketNumber) as updated  " & vbCrLf &
                                    " ON crm.TicketNumber = updated.TicketNumber and CONVERT(datetime, crm.LastUpdated) = updated.LastUpdated " & vbCrLf &
                                    " WHERE ClientID = " & ReadyString(ClientID) & vbCrLf &
                                    " AND CONVERT(date, crm.TicketOpenDate) BETWEEN " & ReadyString(fromdate) & " AND " & ReadyString(todate) & vbCrLf &
                                    " AND ActionName = 'Ticket' " & vbCrLf &
                                    IIf(Filter = "All", "", " AND Status = " & ReadyString(Filter)) & vbCrLf &
                                    " AND CONVERT(bit, ISNULL(crm.IsDelete, 0)) <> 1 ORDER BY Status DESC "
                Dim dborgid, dbname As String

                If CBool2(Common.WebRequest.QueryString("ViewTicketFromMobiz")) Then
#If DEBUG Then
                    'MobizABC dev db
                    dborgid = "cfe39df6-8809-4973-9515-810df75aed12"
                    dbname = WebRequest.Instance.Business.LibraryOrganization.ByID(dborgid)("Database")
#Else
                '1Mobiz prod db 
                dborgid = "84e972ed-f208-4867-9506-5b8f9572562f"
                dbname = WebRequest.Instance.Business.LibraryOrganization.ByID(dborgid)("Database")
#End If
                    Common.WebRequest.ProData.Execute("USE " & EscapeSqlName(dbname))
                End If

                Dim pRows As Object = Common.WebRequest.ProData.Execute(pSql).Rows
                Dim pWB As Aspose.Cells.Workbook
                Dim pWS As Aspose.Cells.Worksheet

                pWB = New Aspose.Cells.Workbook
                pWS = pWB.Worksheets(0)

                If pRows.Count = 0 Then
                    Return JsonDictionary.Serialize("Results", "No data to be downloaded")
                End If

                Dim header() As String = {"Status", "Ticket No", "Subject", "Date Reported", "Due Date", "Assigned To", "Recommended Action", "Remarks"}
                Dim ctrHeader As Integer = 0
                Dim rowctr As Integer = 0

                For Each h As String In header
                    pWS.Cells(rowctr, ctrHeader).PutValue(h)
                    pWS.Cells(rowctr, ctrHeader).Style.ForegroundColor = Color.FromArgb(233, 233, 233)
                    pWS.Cells(rowctr, ctrHeader).Style.Pattern = BackgroundType.Solid
                    pWS.Cells(rowctr, ctrHeader).Style.Font.Size = 12
                    pWS.Cells(rowctr, ctrHeader).Style.Font.IsBold = True
                    ctrHeader += 1
                Next

                rowctr += 1

                For Each report As Object In pRows
                    Dim datarow As DataRow = pDtExcel.NewRow

                    pWS.Cells(rowctr, 0).PutValue(report("Status"))
                    pWS.Cells(rowctr, 1).PutValue(report("No"))
                    pWS.Cells(rowctr, 2).PutValue(report("Type"))
                    pWS.Cells(rowctr, 3).PutValue(IIf(CStr2(report("TicketOpenDate")) = "", "", Format(CDate(report("TicketOpenDate")), "MM-dd-yyyy hh:mm tt")))
                    If CStr2(report("DueDate")) <> "" Then
                        pWS.Cells(rowctr, 4).PutValue(IIf(CStr2(report("DueDate")) = "", "", Format(CDate(report("DueDate")), "MM-dd-yyyy")))
                    End If
                    pWS.Cells(rowctr, 5).PutValue(IIf(CStr2(report("Assigned")) = "", "", report("Assigned")))
                    pWS.Cells(rowctr, 6).PutValue(IIf(CStr2(report("RecommendedAction")) = "", "", report("RecommendedAction")))
                    pWS.Cells(rowctr, 7).PutValue(IIf(CStr2(report("Notes")) = "", "", report("Notes")))

                    rowctr += 1
                Next

                pWS.AutoFitColumns()
                pWS.AutoFitRows()

                Dim mFilenameFolder = Common.ServerPath & "FileTemplates"

                If Not System.IO.Directory.Exists(mFilenameFolder) Then
                    System.IO.Directory.CreateDirectory(mFilenameFolder)
                End If

                mFilenameFolder = Common.ServerPath & "FileTemplates\" & CStr(WebRequest.Instance.OrgID)

                If Not System.IO.Directory.Exists(mFilenameFolder) Then
                    System.IO.Directory.CreateDirectory(mFilenameFolder)
                End If

                Dim strFileName = "IssueTicketsReport_" & ClientID & "_" & Format(Date.Now, "MMddyyhhmmss") & ".xlsx"

                Dim pSourceURL As String = Common.ServerPath & "FileTemplates\" & CStr(WebRequest.Instance.OrgID) & "\" & strFileName

                pWB.Save(pSourceURL)

                Dim file As New System.IO.FileInfo(pSourceURL)

                Response.Clear()
                Response.AddHeader("Content-Disposition", "attachment; filename=Issue Tickets Report.xlsx")
                Response.AddHeader("Content-Length", file.Length.ToString())
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/")
                Response.ContentType = "application/octet-stream"
                Response.WriteFile(file.FullName)
                Response.End()

                My.Computer.FileSystem.DeleteFile(pSourceURL)

            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetTicketFiles()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim ticketid As String = Common.WebRequest.Data("ticketid")
            Dim pSql As String = "SELECT " & vbCrLf &
                                "FileName, " & vbCrLf &
                                "att.LastUpdated as DateUploaded " & vbCrLf &
                                "FROM LibraryCRMAttachments as att " & vbCrLf &
                                "WHERE att.RelatedID = " & ReadyString(ticketid) & vbCrLf

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function UploadTicketFiles() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsCRMDashboard As New CRMDashboard
            Dim ticketid As String = Request.Params.Get("ticketid")
            Dim files As HttpFileCollectionBase = Request.Files
            Dim folderName As String = Common.ServerPath & "FileTemplates\" & WebSession.Instance.OrganizationID & "\IssueTickets\" & ticketid

            If Not System.IO.Directory.Exists(folderName) Then
                System.IO.Directory.CreateDirectory(folderName)
            End If

            Try
                Common.WebRequest.ProData.BeginTransaction()

                For i As Integer = 0 To files.Count - 1
                    Dim file As HttpPostedFileBase = files(i)
                    Dim name As String = file.FileName
                    Dim rename As String = file.FileName
                    Dim ctr As Integer = 1

                    'Dim pSql = "SELECT COUNT(FileName) as Counter FROM LibraryCRMAttachments WHERE FileName = " + ReadyString(name)
                    'Dim ctr = CInt2(Common.WebRequest.ProData.Execute(pSql).Rows(0)("Counter"))

                    For c As Integer = 0 To 10000
                        Dim pSql = "SELECT COUNT(FileName) as Counter FROM LibraryCRMAttachments WHERE FileName = " + ReadyString(rename) & vbCrLf &
                                    " AND RelatedID = " & ReadyString(ticketid)
                        Dim ctrSameFile = CInt2(Common.WebRequest.ProData.Execute(pSql).Rows(0)("Counter"))

                        If ctrSameFile > 0 Then
                            rename = (ctrSameFile + ctr).ToString() + " " + name
                            ctr += 1
                        Else
                            name = rename
                            Exit For
                        End If
                    Next c

                    Dim pSourceURL As String = folderName & "\" & name
                    Dim p As New InsertUpdate("LibraryCRMAttachments")

                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("FileName") = ReadyString(name)
                    p("Module") = ReadyString("IssuesTicket")
                    p("RelatedID") = ReadyString(ticketid) 'TICKET ID
                    p("Type") = ReadyString(Split(file.ContentType, "/")(1))
                    p("LastUpdated") = ReadyString(CDate(Request.Params.Get("DateNow")))
                    p("Location") = ReadyString(pSourceURL)

                    Common.WebRequest.ProData.Execute(p.GetInsert())

                    file.SaveAs(pSourceURL)
                Next

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "uploaded")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try

        End Function

        <HttpPost>
        Function ImportCustomerVendorDataToCrm() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim clsCRMDashboard As New CRMDashboard

            Return clsCRMDashboard.ImportCustomerVendorDataToCrm()
        End Function

        Function GetSystemBehavior() As String
            Dim pOut As JsonDictionary

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            pOut = New JsonDictionary

            Try
                With WebRequest.Instance.Business.SystemDefaults
                    pOut.Add("IsAutoImport", .AutoImportCustomerRecordsToCRM)
                    pOut.Add("IsAutoImportCrmToCustomer", .AutoImportCRMRecordsToCustomer)
                    pOut.Add("IsAutoImportCrmToSupplier", .AutoImportCRMRecordsToSupplier)
                End With
            Catch ex As Exception
                AppendToLog(ex.Message)
                pOut.Add("Error", ex.Message)
            End Try

            Return pOut.Serialize
        End Function

        <HttpPost()>
        Public Function SaveSystemBehavior() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Dim pSql As String

            Try
                Common.WebRequest.ProData.BeginTransaction()

                With WebRequest.Instance.Business.SystemDefaults
                    .AutoImportCustomerRecordsToCRM = Common.WebRequest.Data("IsAutoImport")
                    .AutoImportCRMRecordsToCustomer = Common.WebRequest.Data("IsAutoImportCrmToCustomer")
                    .AutoImportCRMRecordsToSupplier = Common.WebRequest.Data("IsAutoImportCrmToSupplier")
                End With

                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizSystemDefaults'"

                Common.WebRequest.ProData.ExecuteSQL(pSql)

                Common.WebRequest.ProData.CommitTransaction()
                Return JsonDictionary.Serialize("Result", "Success")

            Catch ex As Exception
                Common.WebRequest.ProData.RollbackTransaction()
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function GetFiles() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsCRMDashboard As New CRMDashboard

            Return JsonDictionary.Serialize("Results", clsCRMDashboard.GetFiles(Common.WebRequest.Data("ClientID")))
        End Function

        Function UploadFiles() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsCRMDashboard As New CRMDashboard
            Dim arrRenameFile As Object = jss.Deserialize(Of Object)(Request.Params.Get("Rename"))
            Dim clientid As String = Request.Params.Get("clientid")
            Dim files As HttpFileCollectionBase = Request.Files
            Dim folderName As String = Common.ServerPath & "SurveyFiles\" & clientid

            If Not System.IO.Directory.Exists(folderName) Then
                System.IO.Directory.CreateDirectory(folderName)
            End If

            Try
                Common.WebRequest.ProData.BeginTransaction()

                For i As Integer = 0 To files.Count - 1
                    Dim file As HttpPostedFileBase = files(i)
                    Dim name As String = file.FileName
                    Dim isRemoveFile As Boolean = False

                    For m As Integer = 0 To arrRenameFile.Length - 1
                        If arrRenameFile(m)("FileName") = name And arrRenameFile(m)("State") = "rename" Then
                            name = arrRenameFile(m)("NewName")
                            Exit For
                        ElseIf arrRenameFile(m)("State") = "remove" Then
                            isRemoveFile = True
                            Exit For
                        End If
                    Next

                    If isRemoveFile = False Then
                        Dim p As New InsertUpdate("LibraryCRMAttachments")

                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        p("ClientID") = ReadyString(clientid)
                        p("FileName") = ReadyString(name)
                        p("LastUpdated") = ReadyString(CDate(Request.Params.Get("DateNow")))

                        Common.WebRequest.ProData.Execute(p.GetInsert())

                        Dim pSourceURL As String = folderName & "\" & name
                        file.SaveAs(pSourceURL)
                    End If
                Next

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "uploaded")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try

        End Function

        Function CheckFileName() As Object
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsCRMDashboard As New CRMDashboard
            Dim files As HttpFileCollectionBase = Request.Files
            Dim checkfiles As Object
            Dim arrExFiles As New List(Of Dictionary(Of String, Object))
            Dim pSql As String
            Dim len As Integer
            Dim isFromFile As Boolean
            Dim status As String

            If files.Count > 0 Then
                checkfiles = files
                len = checkfiles.Count
                isFromFile = True
            Else
                checkfiles = Common.WebRequest.AllData("Files")
                len = checkfiles.Length
                isFromFile = False
            End If

            For i As Integer = 0 To len - 1
                'Dim file As HttpPostedFileBase = checkfiles(i)
                Dim name As String
                Dim arrFiles As New Dictionary(Of String, Object)

                If isFromFile Then
                    name = checkfiles(i).FileName
                    status = "rename"
                Else
                    name = checkfiles(i)("NewName")
                    status = checkfiles(i)("State")
                End If

                pSql = "SELECT res.ID FROM LibrarySurveyResponse as res" & vbCrLf &
                        "LEFT JOIN LibrarySurveyRespondent as resd ON res.RespondentID = resd.ID " & vbCrLf &
                        "WHERE res.Answer LIKE " & ReadyStringWild(name) & vbCrLf &
                        "AND resd.ClientID = " & ReadyString(Request.Params.Get("clientid")) & vbCrLf &
                        "AND ISNULL(convert(varchar(max), res.FieldID), '') = '' AND res.Description <> ''"
                Dim ctUploadResponse As Integer = Common.WebRequest.ProData.Execute(pSql).Rows.Count

                pSql = "SELECT ID FROM LibraryCRMAttachments WHERE FileName LIKE " & ReadyStringWild(name) & vbCrLf &
                        "AND ClientID = " & ReadyString(Request.Params.Get("clientid")) & vbCrLf
                Dim ctUpload As Integer = Common.WebRequest.ProData.Execute(pSql).Rows.Count

                If ctUpload > 0 Or ctUploadResponse > 0 Then
                    arrFiles.Add("FileName", name)
                    arrFiles.Add("NewName", "")
                    arrFiles.Add("State", "rename")

                    If status <> "remove" Then
                        arrExFiles.Add(arrFiles)
                    End If
                End If
            Next

            Return Json(New With {Key .data = arrExFiles})
        End Function

        Function SaveKbUrl() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String = "SELECT DefaultValue FROM SystemDefaults WHERE Description = 'KB URL'"
                Dim len As Integer = Common.WebRequest.ProData.Execute(pSql).Rows.Count

                Dim p As New InsertUpdate("SystemDefaults")

                p("Description") = ReadyString("KB Url")
                p("DefaultValue") = ReadyString(Common.WebRequest.Data("Url"))
                p("LastUpdated") = "getdate()"

                If len = 0 Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Else
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(Common.WebRequest.Data("ID"))))
                End If
                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function KbUrl() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = "SELECT ID, DefaultValue FROM SystemDefaults WHERE Description = 'KB URL'"
            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function GetRMAHistory() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsCRMDashboard As New CRMDashboard

            Return JsonDictionary.Serialize("Results", clsCRMDashboard.GetRMAHistory())
        End Function

        Function GetRMAOfTicket() As Object
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsCRMDashboard As New CRMDashboard
            Dim pDT As DataTable = Common.WebRequest.ProData.Execute("SELECT rma.*, (SELECT COUNT(ID) FROM LibraryCRMRMAItems WHERE RMAID = ISNULL(CONVERT(varchar(MAX), rma.RelatedID), rma.ID) AND CONVERT(bit, ISNULL(IsDelete, 0)) <> 1 AND ISNULL(CONVERT(varchar(MAX), RelatedID), '') = '') as NoOfItems FROM LibraryCRMRMA as rma " & vbCrLf &
                                                                     " INNER JOIN (SELECT MAX(LastUpdated) as LastUpdated, RMANumber FROM LibraryCRMRMA GROUP BY RMANumber) as updated " & vbCrLf &
                                                                     " ON rma.RMANumber = updated.RMANumber and CONVERT(datetime, rma.LastUpdated) = updated.LastUpdated " & vbCrLf &
                                                                     " WHERE TicketID = " & ReadyString(Common.WebRequest.Data("TicketID")) & vbCrLf &
                                                                     "  AND CONVERT(bit, ISNULL(IsDelete, 0)) <> 1 " & vbCrLf &
                                                                     " ORDER BY rma.LastUpdated DESC ")
            Dim pRows As Object = pDT.Rows
            Dim arrRMA(pRows.Count - 1) As Object

            Try
                If pRows.Count > 0 Then
                    For i As Integer = 0 To pRows.Count - 1
                        Dim origID As String = IIf(pRows(i)("RelatedID").ToString() = "", pRows(i)("ID").ToString(), pRows(i)("RelatedID").ToString())
                        Dim pRowsOrigItems As Object = Common.WebRequest.ProData.Execute("SELECT * FROM LibraryCRMRMAItems WHERE RMAID = " & ReadyString(origID) & "  AND CONVERT(bit, ISNULL(IsDelete, 0)) <> 1 AND ISNULL(CONVERT(varchar(MAX), RelatedID), '') = ''").Rows
                        Dim arrDistinctItem(pRowsOrigItems.Count - 1) As Object

                        If pRowsOrigItems.Count > 0 Then
                            For m As Integer = 0 To pRowsOrigItems.Count - 1
                                Dim pRowsDistinctItem As DataTable = Common.WebRequest.ProData.Execute("SELECT *, 'edit' as State FROM LibraryCRMRMAItems as items" & vbCrLf &
                                                                                                         " INNER JOIN (SELECT TOP 1 MAX(LastUpdated) as LastUpdated FROM LibraryCRMRMAItems " & vbCrLf &
                                                                                                         " WHERE RMAID =" & ReadyString(origID) & " AND " & vbCrLf &
                                                                                                         " (ID = " & ReadyString(pRowsOrigItems(m)("ID").ToString()) & " OR RelatedID = " & ReadyString(pRowsOrigItems(m)("ID").ToString()) & ")" & vbCrLf &
                                                                                                         "  AND CONVERT(bit, ISNULL(IsDelete, 0)) <> 1 ORDER BY LastUpdated DESC) as updated " & vbCrLf &
                                                                                                         " ON items.LastUpdated = updated.LastUpdated " & vbCrLf &
                                                                                                         " WHERE items.RMAID = " & ReadyString(origID))
                                arrDistinctItem(m) = jss.Deserialize(Of Object)(clsCRMDashboard.GetJson(pRowsDistinctItem))(0)
                            Next
                        End If

                        arrRMA(i) = New With {Key .Details = DirectCast(jss.Deserialize(Of Object)(clsCRMDashboard.GetJson(pRows(i).Table)), Object())(i), .Items = arrDistinctItem}
                    Next
                End If

                Return Json(New With {Key .data = arrRMA})
            Catch ex As Exception
                AppendToLog(ex.Message)
            End Try

        End Function

        Function RemoveRMA() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim clsCRMDashboard As New CRMDashboard
                Dim pSql As String = "UPDATE LibraryCRMRMA SET IsDelete = 1, CreatedBy = " & ReadyString(WebSession.Instance.PeopleUserID) & ", LastUpdated = " & ReadyString(CDate(Common.WebRequest.Data("Date"))) & " WHERE ID = " & ReadyString(Common.WebRequest.Data("RMAID")) & vbCrLf &
                                        " OR RelatedID = " & ReadyString(Common.WebRequest.Data("RMAID"))

                Common.WebRequest.ProData.Execute(pSql)

                pSql = "UPDATE LibraryCRMRMAItems SET IsDelete = 1, CreatedBy = " & ReadyString(WebSession.Instance.PeopleUserID) & ", LastUpdated = " & ReadyString(CDate(Common.WebRequest.Data("Date"))) & " WHERE RMAID = " & ReadyString(Common.WebRequest.Data("RMAID")) & vbCrLf

                Common.WebRequest.ProData.Execute(pSql)

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try

        End Function

        Function SaveRMA() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsCRMDashboard As New CRMDashboard

            Return JsonDictionary.Serialize("Results", clsCRMDashboard.SaveRMA(Common.WebRequest.AllData("data")))
        End Function

        Function RemoveAction() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String
                'If Common.WebRequest.Data("ActionName") = "Opportunity" Then

                'End If

                pSql = "UPDATE TransCRM SET IsDelete = 1 WHERE ID = " & ReadyString(Common.WebRequest.Data("ID"))

                Common.WebRequest.ProData.Execute(pSql)

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try

        End Function

        Function GetTemplateDetails() As Object
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsCRMDashboard As New CRMDashboard

            Dim opp As Object = clsCRMDashboard.GetTemplateDetails(Common.WebRequest.Data("ID"))

            Return Json(New With {Key .data = opp}, JsonRequestBehavior.AllowGet)
        End Function

        Function GetFunnelTemplate() As Object
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsCRMDashboard As New CRMDashboard

            Return Json(New With {Key .data = clsCRMDashboard.GetFunnelTemplate()})
        End Function

        Function SaveAttachFile() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Common.WebRequest.ProData.BeginTransaction()

                Dim p As New InsertUpdate("LibraryCRMAttachments")
                Dim filePic As HttpPostedFileBase = Request.Files(0)

                'Dim filetype As Object = Split(filePic.ContentType, "/")
                Dim pSourceURL As String = Common.ServerPath & "temp\" & WebSession.Instance.OrganizationID
                Dim location As String
                'pSourceURL = pSourceURL & "\" & OrgID

                location = pSourceURL & "\" & Request.Params("ImgID").ToString() & "_" & filePic.FileName

                If Not System.IO.Directory.Exists(pSourceURL) Then
                    IO.Directory.CreateDirectory(pSourceURL)
                End If

                filePic.SaveAs(location)

                p("ID") = ReadyString(Request.Params("ImgID").ToString())
                p("RelatedID") = ReadyString(Request.Params("RelatedID").ToString())
                p("Location") = ReadyString(location)
                p("FileName") = ReadyString(Request.Params("ImgID").ToString() & "_" & filePic.FileName)
                Common.WebRequest.ProData.Execute(p.GetInsert())

                Common.WebRequest.ProData.CommitTransaction()

                Return "saved"
                'Return Json(New With {Key .Location = pFilename,
                '                .CaretPos = caretpos,
                '                .ImgID = imgid,
                '                .Status = "add"
                '            })
            Catch ex As Exception
                Return ex.Message
            End Try
        End Function

        Function SaveFunnelTemplate() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsCRMDashboard As New CRMDashboard

            Return JsonDictionary.Serialize("Results", clsCRMDashboard.SaveFunnelTemplate())
        End Function

        Function GetOpportunity() As Object
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim pDT As DataTable
            Dim clsCRMDashboard As New CRMDashboard
            Dim jss = New JavaScriptSerializer()
            Dim ctr As Integer = 0
            clsCRMDashboard.PageStart = CInt(Request.Form.GetValues("start")(0))
            clsCRMDashboard.PageNoOfRows = CInt(Request.Form.GetValues("length")(0))
            clsCRMDashboard.PageSearchText = Request.Form.GetValues("search[value]")(0).ToString()
            clsCRMDashboard.IsCountPage = False
            clsCRMDashboard.PageColumnNo = Request.Form.GetValues("order[0][column]")(0).ToString()
            clsCRMDashboard.PageColumnDirection = Request.Form.GetValues("order[0][dir]")(0).ToString()
            pDT = clsCRMDashboard.GetOpportunity()

            If pDT.Rows.Count > 0 Then
                If pDT.Rows(0).Table.Columns("Error") IsNot Nothing Then
                    Return pDT.Rows(0)("ERROR")
                End If
            End If

            clsCRMDashboard.IsCountPage = True
            Dim pDTCountPage As DataTable = clsCRMDashboard.GetOpportunity()

            Return Json(New With {Key .data = jss.Deserialize(Of Object)(clsCRMDashboard.GetJson(pDT)), .recordsTotal = pDTCountPage.Rows().Count, .recordsFiltered = pDTCountPage.Rows().Count}, JsonRequestBehavior.AllowGet)
        End Function

        Function GetOpportunityByClient() As Object
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String
            Dim strReturn As String
            Dim pDT As DataTable
            Dim clsCRMDashboard As New CRMDashboard
            Dim jss = New JavaScriptSerializer()
            Dim ctr As Integer = 0
            clsCRMDashboard.ClientID = CStr2(Request.Params("ID"))
            clsCRMDashboard.PageStart = CInt(Request.Form.GetValues("start")(0))
            clsCRMDashboard.PageNoOfRows = CInt(Request.Form.GetValues("length")(0))
            clsCRMDashboard.PageSearchText = Request.Form.GetValues("search[value]")(0).ToString()
            clsCRMDashboard.IsCountPage = False
            clsCRMDashboard.PageColumnNo = Request.Form.GetValues("order[0][column]")(0).ToString()
            clsCRMDashboard.PageColumnDirection = Request.Form.GetValues("order[0][dir]")(0).ToString()
            pDT = clsCRMDashboard.GetOpportunityByClient()

            If pDT.Rows.Count > 0 Then
                If pDT.Rows(0).Table.Columns("Error") IsNot Nothing Then
                    Return pDT.Rows(0)("ERROR")
                End If
            End If

            Dim opp(pDT.Rows.Count - 1) As Object

            Try
                For i As Integer = 0 To pDT.Rows.Count - 1
                    Dim pID As String
                    If CStr(pDT.Rows(i)("ParentOppID").ToString()) <> "" Then
                        pID = pDT.Rows(i)("ID").ToString() 'pDT.Rows(i)("ParentOppID").ToString()
                    Else
                        pID = pDT.Rows(i)("ID").ToString()
                    End If
                    pSql = "SELECT PeopleID FROM LibraryOpportunityContactPerson WHERE OpportunityID = " & ReadyString(pID)
                    Dim pDTCP As DataTable = Common.WebRequest.ProData.Execute(pSql)
                    Dim cp(pDTCP.Rows.Count - 1) As Object
                    Dim Template As Object
                    Dim Checklist As Object
                    Dim currentStage As String = ""

                    If pDT.Rows(i)("OpportunityTemplate").ToString() <> "" Then
                        Template = clsCRMDashboard.GetTemplateDetails(pDT.Rows(i)("OpportunityTemplate").ToString())

                        pSql = "SELECT DescriptionID FROM LibraryCRMStagingChecklist WHERE FunnelTemplateID = " & ReadyString(Template.ID) & vbCrLf &
                                " AND OpportunityID = " & ReadyString(pID)
                        Checklist = New JavaScriptSerializer().Deserialize(Of Object)(clsCRMDashboard.GetJson(Common.WebRequest.ProData.Execute(pSql)))

                        pSql = "SELECT TOP 1 ISNULL(stage.Description, child.Description) as Description FROM LibraryCRMStagingChecklist as chk " & vbCrLf &
                                " LEFT JOIN LibraryCRMFunnelStageChild as child ON child.ID = chk.DescriptionID " & vbCrLf &
                                " LEFT JOIN LibraryCRMFunnelStage as stage ON chk.DescriptionID = stage.ID OR stage.ID = child.FunnelStageID " & vbCrLf &
                                "WHERE OpportunityID = " & ReadyString(pID) & vbCrLf &
                                " ORDER BY stage.OrderNo DESC "
                        Dim cStageResult As DataTable = Common.WebRequest.ProData.Execute(pSql)

                        If cStageResult.Rows.Count > 0 Then
                            currentStage = cStageResult.Rows(0)("Description")
                        End If
                    End If

                    For m As Integer = 0 To pDTCP.Rows.Count - 1
                        Dim ppolDetails As Object = jss.Deserialize(Of Object)(clsCRMDashboard.GetJson(clsCRMDashboard.GetClientOpportunityContactPerson(pDTCP.Rows(m)("PeopleID").ToString())))
                        ppolDetails(0).Add("Tel", jss.Deserialize(Of Object)(clsCRMDashboard.GetJson(clsCRMDashboard.GetProspectPeopleTel(pDTCP.Rows(m)("PeopleID").ToString()))))
                        ppolDetails(0).Add("Mobile", jss.Deserialize(Of Object)(clsCRMDashboard.GetJson(clsCRMDashboard.GetProspectPeopleMobile(pDTCP.Rows(m)("PeopleID").ToString()))))
                        ppolDetails(0).Add("Email", jss.Deserialize(Of Object)(clsCRMDashboard.GetJson(clsCRMDashboard.GetProspectPeopleEmail(pDTCP.Rows(m)("PeopleID").ToString()))))

                        cp(m) = ppolDetails(0)
                    Next

                    opp(i) = New With {Key .ID = pDT.Rows(i)("ID").ToString(),
                                            .Name = pDT.Rows(i)("OpportunityName"),
                                            .Description = pDT.Rows(i)("OpportunityDescription"),
                                            .Size = pDT.Rows(i)("OpportunitySize"),
                                            .Period = CStr2(pDT.Rows(i)("OpportunitySizePeriod")),
                                            .Status = CStr2(pDT.Rows(i)("Status")),
                                            .LastUpdated = Format(pDT.Rows(i)("LastUpdated"), "MMM dd, yyyy hh:mm:ss tt"),
                                            .ContactPerson = cp,
                                            .Template = Template,
                                            .Checklist = Checklist,
                                            .RowNum = pDT.Rows(i)("RowNum"),
                                            .ParentOppID = pDT.Rows(i)("ParentOppID").ToString(),
                                            .CurrentStage = currentStage
                                      }
                Next

                clsCRMDashboard.IsCountPage = True
                Dim pDTCountPage As DataTable = clsCRMDashboard.GetOpportunityByClient()

                Return Json(New With {Key .data = opp, .recordsTotal = pDTCountPage.Rows().Count, .recordsFiltered = pDTCountPage.Rows().Count}, JsonRequestBehavior.AllowGet)
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return ex.Message
            End Try
        End Function

        Function OpportunityList() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = "SELECT ID, OpportunityName as Name, OpportunityDescription, " & vbCrLf &
                                " OpportunitySize as Size, OpportunitySizePeriod, OpportunityTemplate, Status, crm.LastUpdated, RelatedID as ParentOppID " & vbCrLf &
                                " FROM TransCRM as crm " & vbCrLf &
                                " INNER JOIN (SELECT MAX(LastUpdated) as LastUpdated FROM TransCRM WHERE ActionName = 'Opportunity' AND ClientID = 'ba5ef053-166f-4025-8023-42f034fa6e1f' GROUP BY ID) as updated " & vbCrLf &
                                "   ON CONVERT(datetime, crm.LastUpdated) = updated.LastUpdated " & vbCrLf &
                                " WHERE ClientID = " & ReadyString(Common.WebRequest.Data("ClientID")) & vbCrLf &
                                " AND ActionName = 'Opportunity' AND CONVERT(bit, ISNULL(crm.IsDelete, 0)) <> 1 "
            Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", pDT)
        End Function

        Function SaveOpportunity() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsCRMDashboard As New CRMDashboard
            clsCRMDashboard.OpportunityStatus = Common.WebRequest.Data("Status")
            clsCRMDashboard.OpportunityName = Common.WebRequest.Data("Name")
            clsCRMDashboard.OpportunityDescription = Common.WebRequest.Data("Description")
            clsCRMDashboard.OpportunitySize = Common.WebRequest.Data("Size")
            clsCRMDashboard.OpportunitySizePeriod = Common.WebRequest.Data("Period")
            clsCRMDashboard.OpportunityTemplate = Common.WebRequest.Data("Template")
            clsCRMDashboard.OpportunityChecklist = Common.WebRequest.AllData("Checklist")
            clsCRMDashboard.ObjContactPerson = Common.WebRequest.AllData("ContactPerson")
            clsCRMDashboard.ObjContactPersonTelephone = Common.WebRequest.AllData("ContactPersonTelphone")
            clsCRMDashboard.ObjContactPersonMobile = Common.WebRequest.AllData("ContactPersonMobile")
            clsCRMDashboard.ObjContactPersonEmail = Common.WebRequest.AllData("ContactPersonEmail")
            clsCRMDashboard.DateTime = Common.WebRequest.Data("DateNow")
            clsCRMDashboard.OrgID = Common.WebRequest.Data("OrgID")

            Return JsonDictionary.Serialize("Results", clsCRMDashboard.SaveOpportunity())
        End Function

        <HttpPost()>
        Function GetClientTicketSummary() As Object
            'If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim pDT As DataTable
            Dim clsCRMDashboard As New CRMDashboard
            Dim jss = New JavaScriptSerializer()
            Dim ctr As Integer = 0
            clsCRMDashboard.PageStart = CInt(Request.Form.GetValues("start")(0))
            clsCRMDashboard.PageNoOfRows = CInt(Request.Form.GetValues("length")(0))
            clsCRMDashboard.PageSearchText = Request.Form.GetValues("search[value]")(0).ToString()
            clsCRMDashboard.IsCountPage = False
            clsCRMDashboard.PageColumnNo = Request.Form.GetValues("order[0][column]")(0).ToString()
            clsCRMDashboard.PageColumnDirection = Request.Form.GetValues("order[0][dir]")(0).ToString()
            pDT = clsCRMDashboard.GetClientTicketSummary()

            If pDT.Rows.Count > 0 Then
                If pDT.Rows(0).Table.Columns("Error") IsNot Nothing Then
                    Return pDT.Rows(0)("ERROR")
                End If
            End If

            clsCRMDashboard.IsCountPage = True
            Dim pDTCountPage As DataTable = clsCRMDashboard.GetClientTicketSummary()

            Return Json(New With {Key .data = jss.Deserialize(Of Object)(clsCRMDashboard.GetJson(pDT)), .recordsTotal = pDTCountPage.Rows().Count, .recordsFiltered = pDTCountPage.Rows().Count}, JsonRequestBehavior.AllowGet)

        End Function

        Function MyTicketReportSummary()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql, mainquery, dborgid, dbname As String
            Dim clientID As String = Common.WebRequest.Data("ClientID")
            Dim pOut As New JsonDictionary

            If CBool2(Common.WebRequest.Data("ViewTicketFromMobiz")) Then
#If DEBUG Then
                'MobizABC dev db
                dborgid = "cfe39df6-8809-4973-9515-810df75aed12"
                dbname = WebRequest.Instance.Business.LibraryOrganization.ByID(dborgid)("Database")
#Else
                '1Mobiz prod db 
                dborgid = "84e972ed-f208-4867-9506-5b8f9572562f"
                dbname = WebRequest.Instance.Business.LibraryOrganization.ByID(dborgid)("Database")
#End If
                Common.WebRequest.ProData.Execute("USE " & EscapeSqlName(dbname))
            End If

            mainquery = " SELECT COUNT(crm.ID) as Cnt " & vbCrLf &
                        " FROM TransCRM as crm " & vbCrLf &
                        " INNER JOIN (SELECT MAX(LastUpdated) as LastUpdated, TicketNumber FROM TransCRM WHERE ActionName = 'Ticket' GROUP BY TicketNumber) as updated " & vbCrLf &
                        " ON crm.TicketNumber = updated.TicketNumber and CONVERT(datetime, crm.LastUpdated) = updated.LastUpdated " & vbCrLf &
                        " WHERE ISNULL(IsDelete, 0) = 0 AND ActionName = 'Ticket' AND ClientID = " & ReadyString(clientID) & vbCrLf

            pOut.Add("TotalTickets", Common.WebRequest.ProData.ExecuteRow(mainquery)("Cnt"))

            pSql = mainquery & " AND crm.TicketOpenDate >= DATEADD(DAY,-7,GETDATE()) "

            pOut.Add("TotalNewTicketsInLast7Days", Common.WebRequest.ProData.ExecuteRow(pSql)("Cnt"))

            pSql = mainquery & " AND crm.TicketOpenDate >= DATEADD(DAY,-7,GETDATE()) " & vbCrLf &
                    " AND Status = 'Unresolved' "

            pOut.Add("TotalUnresolvedInLast7Days", Common.WebRequest.ProData.ExecuteRow(pSql)("Cnt"))

            pSql = mainquery & " AND Status = 'Unresolved' "

            pOut.Add("TotalUnresolved", Common.WebRequest.ProData.ExecuteRow(pSql)("Cnt"))

            pSql = mainquery & " AND crm.LastUpdated >= DATEADD(DAY,-7,GETDATE()) " & vbCrLf &
                    " AND Status = 'Resolved' "

            pOut.Add("TotalResolvedInLast7Days", Common.WebRequest.ProData.ExecuteRow(pSql)("Cnt"))

            pSql = mainquery & " AND Status = 'Resolved' "

            pOut.Add("TotalResolved", Common.WebRequest.ProData.ExecuteRow(pSql)("Cnt"))

            Return pOut.Serialize()
        End Function

        Function GetTicketPolicy() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute("SELECT * FROM LibraryCRMTicketNotes WHERE IsDefault = 1"))
        End Function

        Function GenerateNumber() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsCRMDashboard As New CRMDashboard
            clsCRMDashboard.GenNumberType = Common.WebRequest.Data("Type")
            Return JsonDictionary.Serialize("Results", clsCRMDashboard.GenerateNumber())
        End Function

        Function SaveCustomerInfoFromDataSource() As String
            Dim webClient As New Net.WebClient
            Dim pJsonString As String
            Dim clsCRMDashboard As New CRMDashboard

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            pJsonString = Common.WebRequest.Data("Source")

            Return JsonDictionary.Serialize("Results", clsCRMDashboard.SaveCustomerInfoFromDataSource(pJsonString))

        End Function

        Function CheckLastCacheDate() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsCRMDashboard As New CRMDashboard

            Return JsonDictionary.Serialize("Results", clsCRMDashboard.CheckLastCacheDate())
        End Function

        Function GetAllClientList() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsCRMDashboard As New CRMDashboard

            Return JsonDictionary.Serialize("Results", clsCRMDashboard.GetAllClientList())
        End Function

        Function SearchOpenToDo() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsCRMDashboard As New CRMDashboard

            Return JsonDictionary.Serialize("Results", clsCRMDashboard.GetOpenToDo())
        End Function

        Function GetOpenToDo() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsCRMDashboard As New CRMDashboard

            Return JsonDictionary.Serialize("Results", clsCRMDashboard.GetOpenToDo())
        End Function

        Function GetOpenSchedule() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsCRMDashboard As New CRMDashboard

            Return JsonDictionary.Serialize("Results", clsCRMDashboard.GetOpenSchedule())
        End Function

        Function SearchScheduledTasks() As Object
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsCRMDashboard As New CRMDashboard
            Dim objSchedule As Object = jss.Deserialize(Of Object)(clsCRMDashboard.GetJson(clsCRMDashboard.GetSchedule()))

            Return Json(New With {Key .Schedule = objSchedule}, JsonRequestBehavior.AllowGet)
        End Function

        <HttpPost()>
        Function GetDashboardRequiredData() As Object
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsCRMDashboard As New CRMDashboard

            Return clsCRMDashboard.GetDashboardRequiredData()

        End Function

        Function SendSmS() As String

            Dim sms As New CRMDashboard
            Dim mMessage As String
            Dim mSender As Object

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            mMessage = CStr2(Common.WebRequest.Data("Message"))
            mSender = Common.WebRequest.AllData("Sender")
            sms.DateTime = Common.WebRequest.Data("DateNow")

            Return JsonDictionary.Serialize("Results", sms.SaveSmS(mMessage, mSender))

        End Function

        Function GetUnsentSmS() As String

            Dim sms As New CRMDashboard

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            FlagSentMessages()
            Return JsonDictionary.Serialize("UpdateData", sms.GetAllUnsentMessages())
        End Function

        Function FlagSentMessages() As String
            Dim objMsgID As Object = Common.WebRequest.AllData("d")
            Try
                Common.WebRequest.ProData.BeginTransaction()

                For Each msg As Object In objMsgID("d")("Rows")
                    Dim pSql As String = "Update TransSmsMessages set SenttoModemTries=1 where ID = " & ReadyString(msg("ID"))
                    Common.WebRequest.ProData.Execute(pSql)

                    pSql = "SELECT TransSMSID FROM TransSmsMessages " & vbCrLf &
                            " WHERE ID = " & ReadyString(msg("ID")) & vbCrLf
                    Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)

                    If pDT.Rows.Count > 0 Then
                        pSql = "Update TransCRM set ActionName='SMS (Sent)' where SMSID = " & ReadyString(pDT.Rows(0)("TransSMSID"))
                        Common.WebRequest.ProData.Execute(pSql)
                    End If
                Next
                Common.WebRequest.ProData.CommitTransaction()

                Return "Sent"
            Catch ex As Exception
                Return ex.Message
            End Try
        End Function

        Function UploadContacts(ByVal objFile As HttpPostedFileBase, ByVal OrgID As String) As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsCRMDashboard As New CRMDashboard
            Dim pSql As String
            Dim pDT As DataTable
            Dim pFilename As String
            Dim pSourceURL As String = Common.ServerPath & "temp"
            Dim dtaExcelData = New DataTable
            Dim strHeader = {"Organization Name", "First Name", "Middle Name", "Last Name", "Country Code1", "Area or Telco Code1", "Number1", "Country Code2", "Area or Telco Code2", "Number2", "Email Address"}
            Dim p As New InsertUpdate
            Dim dateNow As String = Request.Params.Get("date")

            If (Not objFile Is Nothing) Then

                pFilename = pSourceURL & "\Contacts\" & OrgID & "_ContactTemplate.xls"
                CreateFolder(pFilename)
                objFile.SaveAs(pFilename)

                Try
                    dtaExcelData = New DataTable
                    dtaExcelData = ExcelToTable(pSourceURL & "\Contacts\" & OrgID & "_ContactTemplate.xls", strHeader, "")

                    Common.WebRequest.ProData.BeginTransaction()

                    For i As Integer = 0 To dtaExcelData.Rows.Count - 1
                        Dim countrycode1 As String = ""
                        Dim areacode1 As String = ""
                        Dim number1 As String = ""
                        Dim countrycode2 As String = ""
                        Dim areacode2 As String = ""
                        Dim number2 As String = ""
                        Dim areatelcoType1 As String = ""
                        Dim areatelcoType2 As String = ""
                        Dim objTel(1) As Object
                        Dim objMobile(1) As Object

                        strUploadContactStatus = "Uploading " & (i + 1) & " of " & dtaExcelData.Rows.Count & ". Please wait ..."

                        If Trim(CStr2(dtaExcelData.Rows(i).Item("Country Code1"))) <> "" Then
                            pSql = "SELECT TOP 1 ID as ID FROM MobizLibraryCountries WHERE CallingCode = " & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Country Code1"))))
                            pDT = Common.WebRequest.ProData.Execute(pSql)
                            countrycode1 = pDT.Rows(0)("ID").ToString()

                            pSql = "SELECT TOP 1 ID as ID FROM MobizLibraryAreaCodes WHERE CallingCode = " & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Area or Telco Code1"))))
                            pDT = Common.WebRequest.ProData.Execute(pSql)
                            areatelcoType1 = "Tel"

                            If pDT.Rows.Count = 0 Then
                                pSql = "SELECT TOP 1 ID as ID FROM MobizLibraryTelcoPrefixes WHERE Prefix = " & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Area or Telco Code1"))))
                                pDT = Common.WebRequest.ProData.Execute(pSql)

                                If pDT.Rows.Count = 0 Then
                                    Return "We can't find area code or mobile prefix no. " & Trim(CStr2(dtaExcelData.Rows(i).Item("Area or Telco Code1")))
                                End If
                                areatelcoType1 = "Mobile"
                            End If

                            areacode1 = pDT.Rows(0)("ID").ToString()
                            number1 = Trim(CStr2(dtaExcelData.Rows(i).Item("Number1")))
                        End If

                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                        If Trim(CStr2(dtaExcelData.Rows(i).Item("Country Code2"))) <> "" Then
                            pSql = "SELECT TOP 1 ID as ID FROM MobizLibraryCountries WHERE CallingCode = " & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Country Code2"))))
                            pDT = Common.WebRequest.ProData.Execute(pSql)
                            countrycode2 = pDT.Rows(0)("ID").ToString()

                            pSql = "SELECT TOP 1 ID as ID FROM MobizLibraryAreaCodes WHERE CallingCode = " & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Area or Telco Code2"))))
                            pDT = Common.WebRequest.ProData.Execute(pSql)
                            areatelcoType2 = "Tel"

                            If pDT.Rows.Count = 0 Then
                                pSql = "SELECT TOP 1 ID as ID FROM MobizLibraryTelcoPrefixes WHERE Prefix = " & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Area or Telco Code2"))))
                                pDT = Common.WebRequest.ProData.Execute(pSql)

                                If pDT.Rows.Count = 0 Then
                                    Return "We can't find area code or mobile prefix no. " & Trim(CStr2(dtaExcelData.Rows(i).Item("Area or Telco Code2")))
                                End If

                                areatelcoType2 = "Mobile"
                            End If

                            areacode2 = pDT.Rows(0)("ID").ToString()
                            number2 = Trim(CStr2(dtaExcelData.Rows(i).Item("Number2")))
                        End If

                        If Trim(CStr2(dtaExcelData.Rows(i).Item("Organization Name"))) <> "" And (Trim(CStr2(dtaExcelData.Rows(i).Item("First Name"))) <> "" Or Trim(CStr2(dtaExcelData.Rows(i).Item("Middle Name"))) <> "" Or Trim(CStr2(dtaExcelData.Rows(i).Item("Last Name"))) <> "") Then
                            Dim existedOrgID As String
                            Dim PersonID As String = Guid.NewGuid.ToString()
                            pSql = "SELECT TOP 1 ID FROM LibraryOrganization WHERE CompanyName = " & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Organization Name"))))
                            pDT = Common.WebRequest.ProData.Execute(pSql)

                            If pDT.Rows.Count > 0 Then
                                existedOrgID = pDT.Rows(0)("ID").ToString()
                            Else
                                existedOrgID = Guid.NewGuid.ToString()
                                'pSql = "INSERT INTO LibraryOrganization(ID,CompanyName,OfficeAddressBuildingNo,OfficeAddressStreet,OfficeAddressCityID,OfficeAddressCountryID,Background,Mission,Vision,FoundingDate,UserID) " & vbCrLf &
                                '        "VALUES (" & ReadyString(existedOrgID) & "," & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Organization Name")))) & ",'','',NULL,NULL,'','','',''," & ReadyString(WebSession.Instance.PeopleUserID) & ")"

                                p = New InsertUpdate("LibraryOrganization")
                                p("ID") = ReadyString(existedOrgID)
                                p("CompanyName") = ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Organization Name"))))
                                p("OfficeAddressBuildingNo") = ReadyString("")
                                p("OfficeAddressStreet") = ReadyString("")
                                p("OfficeAddressCityID") = "NULL"
                                p("OfficeAddressCountryID") = "NULL"
                                p("Background") = ReadyString("")
                                p("Mission") = ReadyString("")
                                p("Vision") = ReadyString("")
                                p("FoundingDate") = ReadyString("")
                                p("LastUpdated") = ReadyString(dateNow)
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                pSql = p.GetInsert()
                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            End If

                            'pSql = "INSERT INTO LibraryPeople(ID,FirstName,MiddleName,Surname,Position,PositionDescription,IsFromOrg,UserID) " & vbCrLf &
                            '        "VALUES (" & ReadyString(PersonID) & "," & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("First Name")))) & "," & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Middle Name")))) & "," & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Last Name")))) & ",'','','1'," & ReadyString(WebSession.Instance.PeopleUserID) & ")"

                            p = New InsertUpdate("LibraryPeople")
                            p("ID") = ReadyString(PersonID)
                            p("FirstName") = ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("First Name"))))
                            p("MiddleName") = ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Middle Name"))))
                            p("Surname") = ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Last Name"))))
                            p("Position") = ReadyString("")
                            p("PositionDescription") = ReadyString("")
                            p("IsFromOrg") = ReadyString("1")
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                            pSql = p.GetInsert()
                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                            'pSql = "INSERT INTO LibraryOrganizationContactPerson(ID,OrganizationID,PeopleID,UserID,LastUpdated,IsUseForPersonOrgAffiliates) " & vbCrLf &
                            '        "VALUES (" & ReadyString(Guid.NewGuid.ToString()) & "," & ReadyString(existedOrgID) & "," & ReadyString(PersonID) & "," & ReadyString(WebSession.Instance.PeopleUserID) & ",'','0')"

                            p = New InsertUpdate("LibraryOrganizationContactPerson")
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            p("OrganizationID") = ReadyString(existedOrgID)
                            p("PeopleID") = ReadyString(PersonID)
                            p("IsUseForPersonOrgAffiliates") = ReadyString("0")
                            p("LastUpdated") = ReadyString(dateNow)
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                            pSql = p.GetInsert()
                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                            If areatelcoType1 = "Tel" Then
                                'pSql = "INSERT INTO LibraryPeopleContactTel(ID,PeopleID,CountryID,AreaCodeID,ContactNo,ExtensionNo,ContactType,UserID,LastUpdated) " & vbCrLf &
                                '        "VALUES (" & ReadyString(Guid.NewGuid.ToString()) & "," & ReadyString(PersonID) & "," & ReadyString(countrycode1) & "," & ReadyString(areacode1) & "," & ReadyString(number1) & ",'',''," & ReadyString(WebSession.Instance.PeopleUserID) & ",getdate())"

                                p = New InsertUpdate("LibraryPeopleContactTel")
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("PeopleID") = ReadyString(PersonID)
                                p("CountryID") = ReadyString(countrycode1)
                                p("AreaCodeID") = ReadyString(areacode1)
                                p("ContactNo") = ReadyString(number1)
                                p("ExtensionNo") = ReadyString("")
                                p("ContactType") = ReadyString("")
                                p("LastUpdated") = ReadyString(dateNow)
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                pSql = p.GetInsert()
                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            ElseIf areatelcoType1 = "Mobile" Then
                                'pSql = "INSERT INTO LibraryPeopleContactMobile(ID,PeopleID,CountryID,TelCoPrefixID,ContactNo,UserID,ContactType,LastUpdated) " & vbCrLf &
                                '        "VALUES (" & ReadyString(Guid.NewGuid.ToString()) & "," & ReadyString(PersonID) & "," & ReadyString(countrycode1) & "," & ReadyString(areacode1) & "," & ReadyString(number1) & "," & ReadyString(WebSession.Instance.PeopleUserID) & ",'',getdate())"

                                p = New InsertUpdate("LibraryPeopleContactMobile")
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("PeopleID") = ReadyString(PersonID)
                                p("CountryID") = ReadyString(countrycode1)
                                p("TelCoPrefixID") = ReadyString(areacode1)
                                p("ContactNo") = ReadyString(number1)
                                p("ContactType") = ReadyString("")
                                p("LastUpdated") = ReadyString(dateNow)
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                pSql = p.GetInsert()
                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            End If

                            If areatelcoType2 = "Tel" Then
                                'pSql = "INSERT INTO LibraryPeopleContactTel(ID,PeopleID,CountryID,AreaCodeID,ContactNo,ExtensionNo,ContactType,UserID,LastUpdated) " & vbCrLf &
                                '        "VALUES (" & ReadyString(Guid.NewGuid.ToString()) & "," & ReadyString(PersonID) & "," & ReadyString(countrycode2) & "," & ReadyString(areacode2) & "," & ReadyString(number2) & ",'',''," & ReadyString(WebSession.Instance.PeopleUserID) & ",getdate())"

                                p = New InsertUpdate("LibraryPeopleContactTel")
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("PeopleID") = ReadyString(PersonID)
                                p("CountryID") = ReadyString(countrycode2)
                                p("AreaCodeID") = ReadyString(areacode2)
                                p("ContactNo") = ReadyString(number2)
                                p("ExtensionNo") = ReadyString("")
                                p("ContactType") = ReadyString("")
                                p("LastUpdated") = ReadyString(dateNow)
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                pSql = p.GetInsert()
                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            ElseIf areatelcoType2 = "Mobile" Then
                                'pSql = "INSERT INTO LibraryPeopleContactMobile(ID,PeopleID,CountryID,TelCoPrefixID,ContactNo,UserID,ContactType,LastUpdated) " & vbCrLf &
                                '        "VALUES (" & ReadyString(Guid.NewGuid.ToString()) & "," & ReadyString(PersonID) & "," & ReadyString(countrycode2) & "," & ReadyString(areacode2) & "," & ReadyString(number2) & "," & ReadyString(WebSession.Instance.PeopleUserID) & ",'',getdate())"

                                p = New InsertUpdate("LibraryPeopleContactMobile")
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("PeopleID") = ReadyString(PersonID)
                                p("CountryID") = ReadyString(countrycode2)
                                p("TelCoPrefixID") = ReadyString(areacode2)
                                p("ContactNo") = ReadyString(number2)
                                p("ContactType") = ReadyString("")
                                p("LastUpdated") = ReadyString(dateNow)
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                pSql = p.GetInsert()
                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            End If

                            If Trim(CStr2(dtaExcelData.Rows(i).Item("Email Address"))) <> "" Then
                                'pSql = "INSERT INTO LibraryPeopleContactEmail(ID,PeopleID,EmailAddress,UserID,EmailType,LastUpdated) " & vbCrLf &
                                '        "VALUES (" & ReadyString(Guid.NewGuid.ToString()) & "," & ReadyString(PersonID) & "," & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Email Address")))) & "," & ReadyString(WebSession.Instance.PeopleUserID) & ",'',getdate())"

                                p = New InsertUpdate("LibraryPeopleContactEmail")

                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("PeopleID") = ReadyString(PersonID)
                                p("EmailAddress") = ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Email Address"))))
                                p("EmailType") = ReadyString("")
                                p("LastUpdated") = ReadyString(dateNow)
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                pSql = p.GetInsert()
                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            End If

                        ElseIf Trim(CStr2(dtaExcelData.Rows(i).Item("Organization Name"))) = "" Then
                            Dim PersonID As String = Guid.NewGuid.ToString()
                            'pSql = "INSERT INTO LibraryPeople(ID,FirstName,MiddleName,Surname,Position,PositionDescription,IsFromOrg,UserID) " & vbCrLf &
                            '        "VALUES (" & ReadyString(PersonID) & "," & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("First Name")))) & "," & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Middle Name")))) & "," & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Last Name")))) & ",'','','0'," & ReadyString(WebSession.Instance.PeopleUserID) & ")"

                            p = New InsertUpdate("LibraryPeople")
                            p("ID") = ReadyString(PersonID)
                            p("FirstName") = ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("First Name"))))
                            p("MiddleName") = ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Middle Name"))))
                            p("Surname") = ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Last Name"))))
                            p("Position") = ReadyString("")
                            p("PositionDescription") = ReadyString("")
                            p("IsFromOrg") = ReadyString("0")
                            p("LastUpdated") = ReadyString(dateNow)
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                            pSql = p.GetInsert()
                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                            If areatelcoType1 = "Tel" Then
                                'pSql = "INSERT INTO LibraryPeopleContactTel(ID,PeopleID,CountryID,AreaCodeID,ContactNo,ExtensionNo,ContactType,UserID,LastUpdated) " & vbCrLf &
                                '        "VALUES (" & ReadyString(Guid.NewGuid.ToString()) & "," & ReadyString(PersonID) & "," & ReadyString(countrycode1) & "," & ReadyString(areacode1) & "," & ReadyString(number1) & ",'',''," & ReadyString(WebSession.Instance.PeopleUserID) & ",getdate())"

                                p = New InsertUpdate("LibraryPeopleContactTel")
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("PeopleID") = ReadyString(PersonID)
                                p("CountryID") = ReadyString(countrycode1)
                                p("AreaCodeID") = ReadyString(areacode1)
                                p("ContactNo") = ReadyString(number1)
                                p("ExtensionNo") = ReadyString("")
                                p("ContactType") = ReadyString("")
                                p("LastUpdated") = ReadyString(dateNow)
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                pSql = p.GetInsert()
                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            ElseIf areatelcoType1 = "Mobile" Then
                                'pSql = "INSERT INTO LibraryPeopleContactMobile(ID,PeopleID,CountryID,TelCoPrefixID,ContactNo,UserID,ContactType,LastUpdated) " & vbCrLf &
                                '        "VALUES (" & ReadyString(Guid.NewGuid.ToString()) & "," & ReadyString(PersonID) & "," & ReadyString(countrycode1) & "," & ReadyString(areacode1) & "," & ReadyString(number1) & "," & ReadyString(WebSession.Instance.PeopleUserID) & ",'',getdate())"

                                p = New InsertUpdate("LibraryPeopleContactMobile")
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("PeopleID") = ReadyString(PersonID)
                                p("CountryID") = ReadyString(countrycode1)
                                p("TelCoPrefixID") = ReadyString(areacode1)
                                p("ContactNo") = ReadyString(number1)
                                p("ContactType") = ReadyString("")
                                p("LastUpdated") = ReadyString(dateNow)
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                pSql = p.GetInsert()
                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            End If

                            If areatelcoType2 = "Tel" Then
                                'pSql = "INSERT INTO LibraryPeopleContactTel(ID,PeopleID,CountryID,AreaCodeID,ContactNo,ExtensionNo,ContactType,UserID,LastUpdated) " & vbCrLf &
                                '        "VALUES (" & ReadyString(Guid.NewGuid.ToString()) & "," & ReadyString(PersonID) & "," & ReadyString(countrycode2) & "," & ReadyString(areacode2) & "," & ReadyString(number2) & ",'',''," & ReadyString(WebSession.Instance.PeopleUserID) & ",getdate())"

                                p = New InsertUpdate("LibraryPeopleContactTel")
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("PeopleID") = ReadyString(PersonID)
                                p("CountryID") = ReadyString(countrycode2)
                                p("AreaCodeID") = ReadyString(areacode2)
                                p("ContactNo") = ReadyString(number2)
                                p("ExtensionNo") = ReadyString("")
                                p("ContactType") = ReadyString("")
                                p("LastUpdated") = ReadyString(dateNow)
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                pSql = p.GetInsert()
                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            ElseIf areatelcoType2 = "Mobile" Then
                                'pSql = "INSERT INTO LibraryPeopleContactMobile(ID,PeopleID,CountryID,TelCoPrefixID,ContactNo,UserID,ContactType,LastUpdated) " & vbCrLf &
                                '        "VALUES (" & ReadyString(Guid.NewGuid.ToString()) & "," & ReadyString(PersonID) & "," & ReadyString(countrycode2) & "," & ReadyString(areacode2) & "," & ReadyString(number2) & "," & ReadyString(WebSession.Instance.PeopleUserID) & ",'',getdate())"

                                p = New InsertUpdate("LibraryPeopleContactMobile")
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("PeopleID") = ReadyString(PersonID)
                                p("CountryID") = ReadyString(countrycode2)
                                p("TelCoPrefixID") = ReadyString(areacode2)
                                p("ContactNo") = ReadyString(number2)
                                p("ContactType") = ReadyString("")
                                p("LastUpdated") = ReadyString(dateNow)
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                pSql = p.GetInsert()
                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            End If

                            If Trim(CStr2(dtaExcelData.Rows(i).Item("Email Address"))) <> "" Then
                                'pSql = "INSERT INTO LibraryPeopleContactEmail(ID,PeopleID,EmailAddress,UserID,EmailType,LastUpdated) " & vbCrLf &
                                '        "VALUES (" & ReadyString(Guid.NewGuid.ToString()) & "," & ReadyString(PersonID) & "," & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Email Address")))) & "," & ReadyString(WebSession.Instance.PeopleUserID) & ",'',getdate())"

                                p = New InsertUpdate("LibraryPeopleContactEmail")
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("PeopleID") = ReadyString(PersonID)
                                p("EmailAddress") = ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Email Address"))))
                                p("EmailType") = ReadyString("")
                                p("LastUpdated") = ReadyString(dateNow)
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                pSql = p.GetInsert()
                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            End If

                        Else
                            Dim OrganizationID As String = Guid.NewGuid.ToString()

                            'pSql = "INSERT INTO LibraryOrganization(ID,CompanyName,OfficeAddressBuildingNo,OfficeAddressStreet,OfficeAddressCityID,OfficeAddressCountryID,Background,Mission,Vision,FoundingDate,UserID) " & vbCrLf &
                            '        "VALUES (" & ReadyString(OrganizationID) & "," & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Organization Name")))) & ",'','',NULL,NULL,'','','',''," & ReadyString(WebSession.Instance.PeopleUserID) & ")"

                            p = New InsertUpdate("LibraryOrganization")
                            p("ID") = ReadyString(OrganizationID)
                            p("CompanyName") = ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Organization Name"))))
                            p("OfficeAddressBuildingNo") = ReadyString("")
                            p("OfficeAddressStreet") = ReadyString("")
                            p("OfficeAddressCityID") = "NULL"
                            p("OfficeAddressCountryID") = "NULL"
                            p("Background") = ReadyString("")
                            p("Mission") = ReadyString("")
                            p("Vision") = ReadyString("")
                            p("LastUpdated") = ReadyString(dateNow)
                            p("FoundingDate") = ReadyString("")
                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                            pSql = p.GetInsert()
                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                            If areatelcoType1 = "Tel" Then
                                'pSql = "INSERT INTO LibraryOrganizationContactTel(ID,OrganizationID,CountryID,AreaCodeID,ContactNo,ExtensionNo,ContactType,UserID,LastUpdated) " & vbCrLf &
                                '        "VALUES (" & ReadyString(Guid.NewGuid.ToString()) & "," & ReadyString(OrganizationID) & "," & ReadyString(countrycode1) & "," & ReadyString(areacode1) & "," & ReadyString(number1) & ",'',''," & ReadyString(WebSession.Instance.PeopleUserID) & ",getdate())"

                                p = New InsertUpdate("LibraryOrganizationContactTel")
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("OrganizationID") = ReadyString(OrganizationID)
                                p("CountryID") = ReadyString(countrycode1)
                                p("AreaCodeID") = ReadyString(areacode1)
                                p("ContactNo") = ReadyString(number1)
                                p("ExtensionNo") = ReadyString("")
                                p("ContactType") = ReadyString("")
                                p("LastUpdated") = ReadyString(dateNow)
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                pSql = p.GetInsert()

                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            ElseIf areatelcoType1 = "Mobile" Then
                                'pSql = "INSERT INTO LibraryOrganizationContactMobile(ID,OrganizationID,CountryID,TelCoPrefixID,ContactNo,UserID,ContactType,LastUpdated) " & vbCrLf &
                                '        "VALUES (" & ReadyString(Guid.NewGuid.ToString()) & "," & ReadyString(OrganizationID) & "," & ReadyString(countrycode1) & "," & ReadyString(areacode1) & "," & ReadyString(number1) & "," & ReadyString(WebSession.Instance.PeopleUserID) & ",'',getdate())"

                                p = New InsertUpdate("LibraryOrganizationContactMobile")
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("OrganizationID") = ReadyString(OrganizationID)
                                p("CountryID") = ReadyString(countrycode1)
                                p("TelCoPrefixID") = ReadyString(areacode1)
                                p("ContactNo") = ReadyString(number1)
                                p("ContactType") = ReadyString("")
                                p("LastUpdated") = ReadyString(dateNow)
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                pSql = p.GetInsert()
                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            End If

                            If areatelcoType2 = "Tel" Then
                                'pSql = "INSERT INTO LibraryOrganizationContactTel(ID,OrganizationID,CountryID,AreaCodeID,ContactNo,ExtensionNo,ContactType,UserID,LastUpdated) " & vbCrLf &
                                '        "VALUES (" & ReadyString(Guid.NewGuid.ToString()) & "," & ReadyString(OrganizationID) & "," & ReadyString(countrycode2) & "," & ReadyString(areacode2) & "," & ReadyString(number2) & ",'',''," & ReadyString(WebSession.Instance.PeopleUserID) & ",getdate())"

                                p = New InsertUpdate("LibraryOrganizationContactTel")
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("OrganizationID") = ReadyString(OrganizationID)
                                p("CountryID") = ReadyString(countrycode2)
                                p("AreaCodeID") = ReadyString(areacode2)
                                p("ContactNo") = ReadyString(number2)
                                p("ExtensionNo") = ReadyString("")
                                p("ContactType") = ReadyString("")
                                p("LastUpdated") = ReadyString(dateNow)
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                pSql = p.GetInsert()
                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            ElseIf areatelcoType2 = "Mobile" Then
                                'pSql = "INSERT INTO LibraryOrganizationContactMobile(ID,OrganizationID,CountryID,TelCoPrefixID,ContactNo,UserID,ContactType,LastUpdated) " & vbCrLf &
                                '        "VALUES (" & ReadyString(Guid.NewGuid.ToString()) & "," & ReadyString(OrganizationID) & "," & ReadyString(countrycode2) & "," & ReadyString(areacode2) & "," & ReadyString(number2) & "," & ReadyString(WebSession.Instance.PeopleUserID) & ",'',getdate())"

                                p = New InsertUpdate("LibraryOrganizationContactMobile")
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("OrganizationID") = ReadyString(OrganizationID)
                                p("CountryID") = ReadyString(countrycode2)
                                p("TelCoPrefixID") = ReadyString(areacode2)
                                p("ContactNo") = ReadyString(number2)
                                p("ContactType") = ReadyString("")
                                p("LastUpdated") = ReadyString(dateNow)
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                pSql = p.GetInsert()
                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            End If

                            If Trim(CStr2(dtaExcelData.Rows(i).Item("Email Address"))) <> "" Then
                                'pSql = "INSERT INTO LibraryOrganizationContactEmail(ID,OrganizationID,EmailAddress,UserID,EmailType,LastUpdated) " & vbCrLf &
                                '        "VALUES (" & ReadyString(Guid.NewGuid.ToString()) & "," & ReadyString(OrganizationID) & "," & ReadyString(Trim(CStr2(dtaExcelData.Rows(i).item("Email Address")))) & "," & ReadyString(WebSession.Instance.PeopleUserID) & ",'',getdate())"

                                p = New InsertUpdate("LibraryOrganizationContactEmail")
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("OrganizationID") = ReadyString(OrganizationID)
                                p("EmailAddress") = ReadyString(Trim(CStr2(dtaExcelData.Rows(i).Item("Email Address"))))
                                p("EmailType") = ReadyString("")
                                p("LastUpdated") = ReadyString(dateNow)
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                pSql = p.GetInsert()

                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            End If
                        End If
                    Next

                    Common.WebRequest.ProData.CommitTransaction()

                    Return "Saved"
                Catch ex As Exception
                    Return ex.Message
                End Try
            End If
        End Function

        Function CheckingExcelFile(ByVal objFile As HttpPostedFileBase, ByVal OrgID As String, ByVal DateTime As String) As Object
            Return UploadCustomerInfo(objFile, OrgID, DateTime)
        End Function

        Function UploadCustomerInfo(ByVal objFile As HttpPostedFileBase, ByVal OrgID As String, ByVal DateTime As String) As Object
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim clsCRMDashboard As New CRMDashboard
            Dim pSql As String
            Dim pDT As DataTable
            Dim pFilename As String
            Dim pSourceURL As String = Common.ServerPath & "temp"
            Dim dtaExcelData = New DataTable
            Dim strHeader = {"ID", "Organization Name", "Complete Name", "First Name", "Middle name", "Last Name", "Person Generation Suffix",
                                "Country Code1", "Area or Telco Code1", "Number1",
                                "Country Code2", "Area or Telco Code2", "Number2",
                                "Email Address", "Website", "Building No Address",
                                "Street Address", "City Address ID", "City Address",
                                "Country Address ID", "Country Address", "Business Industry", "Business Industry ID",
                                "Contact Person FirstName", "Contact Person MiddleName", "Contact Person LastName",
                                "Interested In", "Contact Source", "Contact Owner", "Mobile Number"
                            }
            Dim genSuffix As Object = {"Jr.", "Jr", "Junior", "Sr.", "Sr", "Senior", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X"}
            Dim savemsg As String = ""
            Dim errmsg As String = ""
            If (Not objFile Is Nothing) Then

                pFilename = pSourceURL & "\Contacts\" & OrgID & "_CustomerList.xls"
                CreateFolder(pFilename)
                objFile.SaveAs(pFilename)

                Try
                    dtaExcelData = New DataTable
                    dtaExcelData = ExcelToTable(pSourceURL & "\Contacts\" & OrgID & "_CustomerList.xls", strHeader, "")

                    Common.WebRequest.ProData.BeginTransaction()

                    For i As Integer = 0 To dtaExcelData.Rows.Count - 1
                        Dim ID As String
                        Dim CompanyName As String
                        Dim PersonFullName As String
                        Dim PersonSurname As String
                        Dim PersonFirstName As String
                        Dim PersonMiddleName As String
                        Dim PersonSuffixName As String
                        Dim CountryCode1 As String
                        Dim AreaTelCode1 As String
                        Dim Number1 As String
                        Dim CountryCode2 As String
                        Dim AreaTelCode2 As String
                        Dim Number2 As String
                        Dim officeaddressbuildingno As String
                        Dim officeaddressstreet As String
                        Dim officeaddresscityID As String
                        Dim City As String
                        Dim Country As String
                        Dim officeaddresscountryid As String
                        Dim ContactMobileCallingCode As String
                        Dim ContactMobileTelCoPrefixID As String
                        Dim ContactMobilePrefix As String
                        Dim ContactMobileContactNo As String
                        Dim ContactTelCallingCode As String
                        Dim ContactTelAreaCodeID As String
                        Dim ContactTelContactNo As String
                        Dim IndustryClassID As String
                        Dim Classes As String
                        Dim ContactEMailEmailAddress As String
                        Dim ContactWebsiteWebsite As String
                        Dim RepresentativeFirstName As String
                        Dim RepresentativeMiddleName As String
                        Dim RepresentativeLastName As String
                        Dim InterestedIn As String
                        Dim ContactSource As String
                        Dim ContactOwner As String
                        Dim MobileNo As String
                        Dim p
                        Dim countrycode1ID As String
                        Dim areacode1ID As String
                        Dim countrycode2ID As String
                        Dim areacode2ID As String
                        Dim areatelcoType1 As String
                        Dim areatelcoType2 As String

                        ID = If(Trim(CStr2(dtaExcelData.Rows(i).Item("ID"))) = "", Guid.NewGuid.ToString(), Trim(CStr2(dtaExcelData.Rows(i).Item("ID"))))

                        Try
                            CompanyName = Trim(CStr2(dtaExcelData.Rows(i).Item("Organization Name")))
                        Catch ex As Exception
                            CompanyName = ""
                        End Try

                        Try
                            PersonSurname = Trim(CStr2(dtaExcelData.Rows(i).Item("Last Name")))
                        Catch ex As Exception
                            PersonSurname = ""
                        End Try

                        Try
                            PersonFirstName = Trim(CStr2(dtaExcelData.Rows(i).Item("First Name")))
                        Catch ex As Exception
                            PersonFirstName = ""
                        End Try

                        Try
                            PersonMiddleName = Trim(CStr2(dtaExcelData.Rows(i).Item("Middle Name")))
                        Catch ex As Exception
                            PersonMiddleName = ""
                        End Try

                        Try
                            PersonSuffixName = Trim(CStr2(dtaExcelData.Rows(i).Item("Person Generation Suffix")))
                        Catch ex As Exception
                            PersonSuffixName = ""
                        End Try

                        Try
                            PersonFullName = Trim(CStr2(dtaExcelData.Rows(i).Item("Complete Name")))

                            If PersonFullName <> "" Then
                                Dim hasGenSuffix As Boolean = False 'by default
                                Dim tempFullName As String

                                PersonSuffixName = PersonFullName.Split(" ").Last()

                                For Each suf As String In genSuffix
                                    If suf = PersonSuffixName Then
                                        hasGenSuffix = True
                                        PersonSuffixName = suf
                                    End If
                                Next

                                If PersonFullName.Split(" ").Length > 1 Then
                                    If hasGenSuffix Then
                                        tempFullName = Trim(PersonFullName.Replace(PersonSuffixName, ""))
                                        PersonSurname = Trim(tempFullName.Split(" ").Last())
                                        PersonFirstName = Trim(tempFullName.Replace(PersonSurname, ""))
                                    Else
                                        PersonSurname = Trim(PersonFullName.Split(" ").Last())
                                        PersonFirstName = Trim(PersonFullName.Remove(PersonFullName.LastIndexOf(" ") + 1))
                                    End If

                                Else
                                    PersonFirstName = PersonFullName.Split(" ")(0).ToString()
                                End If
                            End If
                        Catch ex As Exception
                            PersonFullName = ""
                        End Try

                        Try
                            CountryCode1 = Trim(CStr2(dtaExcelData.Rows(i).Item("Country Code1")))
                        Catch ex As Exception
                            CountryCode1 = ""
                        End Try

                        Try
                            AreaTelCode1 = Trim(CStr2(dtaExcelData.Rows(i).Item("Area or Telco Code1")))
                        Catch ex As Exception
                            AreaTelCode1 = ""
                        End Try

                        Try
                            Number1 = Trim(CStr2(dtaExcelData.Rows(i).Item("Number1")))
                        Catch ex As Exception
                            Number1 = ""
                        End Try

                        Try
                            CountryCode2 = Trim(CStr2(dtaExcelData.Rows(i).Item("Country Code2")))
                        Catch ex As Exception
                            CountryCode2 = ""
                        End Try

                        Try
                            AreaTelCode2 = Trim(CStr2(dtaExcelData.Rows(i).Item("Area or Telco Code2")))
                        Catch ex As Exception
                            AreaTelCode2 = ""
                        End Try

                        Try
                            Number2 = Trim(CStr2(dtaExcelData.Rows(i).Item("Number2")))
                        Catch ex As Exception
                            Number2 = ""
                        End Try

                        Try
                            officeaddressbuildingno = Trim(CStr2(dtaExcelData.Rows(i).Item("Building No Address")))
                        Catch ex As Exception
                            officeaddressbuildingno = ""
                        End Try

                        Try
                            officeaddressstreet = Trim(CStr2(dtaExcelData.Rows(i).Item("Street Address")))
                        Catch ex As Exception
                            officeaddressstreet = ""
                        End Try

                        Try
                            officeaddresscityID = Trim(CStr2(dtaExcelData.Rows(i).Item("City Address ID")))
                        Catch ex As Exception
                            officeaddresscityID = ""
                        End Try

                        Try
                            City = Trim(CStr2(dtaExcelData.Rows(i).Item("City Address")))
                        Catch ex As Exception
                            City = ""
                        End Try

                        Try
                            officeaddresscountryid = Trim(CStr2(dtaExcelData.Rows(i).Item("Country Address ID")))
                        Catch ex As Exception
                            officeaddresscountryid = ""
                        End Try

                        Try
                            Country = Trim(CStr2(dtaExcelData.Rows(i).Item("Country Address")))
                        Catch ex As Exception
                            Country = ""
                        End Try

                        Try
                            IndustryClassID = Trim(CStr2(dtaExcelData.Rows(i).Item("Business Industry ID")))
                        Catch ex As Exception
                            IndustryClassID = ""
                        End Try

                        Try
                            Classes = Trim(CStr2(dtaExcelData.Rows(i).Item("Business Industry")))
                        Catch ex As Exception
                            Classes = ""
                        End Try

                        Try
                            ContactEMailEmailAddress = Trim(CStr2(dtaExcelData.Rows(i).Item("Email Address")))
                        Catch ex As Exception
                            ContactEMailEmailAddress = ""
                        End Try

                        Try
                            ContactWebsiteWebsite = Trim(CStr2(dtaExcelData.Rows(i).Item("Website")))
                        Catch ex As Exception
                            ContactWebsiteWebsite = ""
                        End Try

                        Try
                            RepresentativeFirstName = Trim(CStr2(dtaExcelData.Rows(i).Item("Contact Person FirstName")))
                        Catch ex As Exception
                            RepresentativeFirstName = ""
                        End Try

                        Try
                            RepresentativeMiddleName = Trim(CStr2(dtaExcelData.Rows(i).Item("Contact Person MiddleName")))
                        Catch ex As Exception
                            RepresentativeMiddleName = ""
                        End Try

                        Try
                            RepresentativeLastName = Trim(CStr2(dtaExcelData.Rows(i).Item("Contact Person LastName")))
                        Catch ex As Exception
                            RepresentativeLastName = ""
                        End Try

                        Try
                            InterestedIn = Trim(CStr2(dtaExcelData.Rows(i).Item("Interested In")))
                        Catch ex As Exception
                            InterestedIn = ""
                        End Try

                        Try
                            ContactSource = Trim(CStr2(dtaExcelData.Rows(i).Item("Contact Source")))
                        Catch ex As Exception
                            ContactSource = ""
                        End Try

                        Try
                            ContactOwner = Trim(CStr2(dtaExcelData.Rows(i).Item("Contact Owner")))
                        Catch ex As Exception
                            ContactOwner = ""
                        End Try

                        Try
                            MobileNo = Regex.Replace(Trim(CStr2(dtaExcelData.Rows(i).Item("Mobile Number"))), "\D", "")

                            If MobileNo.Substring(0, 2).ToString() <> "63" Then
                                errmsg += "Invalid mobile number: " & MobileNo & " in row " & (i + 2) & vbCrLf
                            Else
                                CountryCode1 = MobileNo.Substring(0, 2)
                                AreaTelCode1 = MobileNo.Substring(2, 3)
                                Number1 = MobileNo.Substring(5)
                                areatelcoType1 = "Mobile"
                            End If
                        Catch ex As Exception
                            MobileNo = ""
                        End Try

                        Dim arr As Object = New With {Key .ID = ID,
                                                    .OrganizationName = CompanyName,
                                                    .FirstName = PersonFirstName,
                                                    .MiddleName = PersonMiddleName,
                                                    .LastName = PersonSurname,
                                                    .CountryCode1 = CountryCode1,
                                                    .AreaTelCode1 = AreaTelCode1,
                                                    .Number1 = Number1,
                                                    .CountryCode2 = CountryCode2,
                                                    .AreaTelCode2 = AreaTelCode2,
                                                    .Number2 = Number2,
                                                    .BuildingNo = officeaddressbuildingno,
                                                    .Street = officeaddressstreet,
                                                    .City = City,
                                                    .CityID = officeaddresscountryid,
                                                    .Country = Country,
                                                    .CountryID = officeaddresscountryid,
                                                    .BusinessNameID = IndustryClassID,
                                                    .BusinessName = Classes,
                                                    .Email = ContactEMailEmailAddress,
                                                    .Website = ContactWebsiteWebsite,
                                                    .CpFirstName = RepresentativeFirstName,
                                                    .CpMiddleName = RepresentativeMiddleName,
                                                    .CpLastName = RepresentativeLastName
                                                }
                        If CountryCode1 <> "" And AreaTelCode1 <> "" And Number1 <> "" Then
                            pSql = "SELECT TOP 1 ID as ID FROM MobizLibraryCountries WHERE CallingCode = " & ReadyString(Trim(CountryCode1))
                            pDT = Common.WebRequest.ProData.Execute(pSql)
                            countrycode1ID = pDT.Rows(0)("ID").ToString()

                            pSql = "SELECT TOP 1 ID as ID FROM MobizLibraryAreaCodes WHERE CallingCode = " & ReadyString(Trim(AreaTelCode1))
                            pDT = Common.WebRequest.ProData.Execute(pSql)

                            If pDT.Rows.Count = 0 Then
                                pSql = "SELECT TOP 1 ID as ID FROM MobizLibraryTelcoPrefixes WHERE Prefix = " & ReadyString(Trim(AreaTelCode1))
                                pDT = Common.WebRequest.ProData.Execute(pSql)

                                If pDT.Rows.Count = 0 Then
                                    errmsg += "We can't find area code or mobile prefix no. " & Trim(AreaTelCode1) & vbCrLf
                                Else
                                    areatelcoType1 = "Mobile"
                                    areacode1ID = pDT.Rows(0)("ID").ToString()
                                End If
                            Else
                                areatelcoType1 = "Tel"
                                areacode1ID = pDT.Rows(0)("ID").ToString()
                            End If
                        Else
                            If CountryCode1 <> "" And AreaTelCode1 = "" And Number1 = "" Then
                                errmsg += "Row No. " & (i + 2) & ", column Area or Telco Code1 and Number1 are required." & vbCrLf
                            ElseIf CountryCode1 <> "" And AreaTelCode1 = "" Then
                                errmsg += "Row No. " & (i + 2) & ", column Area or Telco Code1 is required." & vbCrLf
                            ElseIf CountryCode1 <> "" And Number1 = "" Then
                                errmsg += "Row No. " & (i + 2) & ", column Number1 is required." & vbCrLf

                            ElseIf AreaTelCode1 <> "" And CountryCode1 = "" And Number1 = "" Then
                                errmsg += "Row No. " & (i + 2) & ", column CountryCode1 and Number1 are required." & vbCrLf
                            ElseIf AreaTelCode1 <> "" And CountryCode1 = "" Then
                                errmsg += "Row No. " & (i + 2) & ", column CountryCode1 is required." & vbCrLf
                            ElseIf AreaTelCode1 <> "" And Number1 = "" Then
                                errmsg += "Row No. " & (i + 2) & ", column Number1 is required." & vbCrLf

                            ElseIf Number1 <> "" And CountryCode1 = "" And AreaTelCode1 = "" Then
                                errmsg += "Row No. " & (i + 2) & ", column CountryCode1 and Number1 are required." & vbCrLf
                            ElseIf Number1 <> "" And CountryCode1 = "" Then
                                errmsg += "Row No. " & (i + 2) & ", column CountryCode1 is required." & vbCrLf
                            ElseIf Number1 <> "" And AreaTelCode1 = "" Then
                                errmsg += "Row No. " & (i + 2) & ", column Area or Telco Code1 is required." & vbCrLf
                            End If
                        End If

                        If CountryCode2 <> "" And AreaTelCode2 <> "" And Number2 <> "" Then
                            pSql = "SELECT TOP 1 ID as ID FROM MobizLibraryCountries WHERE CallingCode = " & ReadyString(Trim(CountryCode2))
                            pDT = Common.WebRequest.ProData.Execute(pSql)
                            countrycode2ID = pDT.Rows(0)("ID").ToString()

                            pSql = "SELECT TOP 1 ID as ID FROM MobizLibraryAreaCodes WHERE CallingCode = " & ReadyString(Trim(AreaTelCode2))
                            pDT = Common.WebRequest.ProData.Execute(pSql)

                            If pDT.Rows.Count = 0 Then
                                pSql = "SELECT TOP 1 ID as ID FROM MobizLibraryTelcoPrefixes WHERE Prefix = " & ReadyString(Trim(AreaTelCode2))
                                pDT = Common.WebRequest.ProData.Execute(pSql)

                                If pDT.Rows.Count = 0 Then
                                    errmsg += "We can't find area code or mobile prefix no. " & Trim(AreaTelCode2) & vbCrLf
                                Else
                                    areatelcoType2 = "Mobile"
                                    areacode2ID = pDT.Rows(0)("ID").ToString()
                                End If
                            Else
                                areatelcoType2 = "Tel"
                                areacode2ID = pDT.Rows(0)("ID").ToString()
                            End If
                        Else
                            If CountryCode2 <> "" And AreaTelCode2 = "" And Number2 = "" Then
                                errmsg += "Row No. " & (i + 2) & ", column Area or Telco Code2 and Number2 are required." & vbCrLf
                            ElseIf CountryCode2 <> "" And AreaTelCode2 = "" Then
                                errmsg += "Row No. " & (i + 2) & ", column Area or Telco Code2 is required." & vbCrLf
                            ElseIf CountryCode2 <> "" And Number2 = "" Then
                                errmsg += "Row No. " & (i + 2) & ", column Number2 is required." & vbCrLf

                            ElseIf AreaTelCode2 <> "" And CountryCode2 = "" And Number2 = "" Then
                                errmsg += "Row No. " & (i + 2) & ", column CountryCode2 and Number1 are required." & vbCrLf
                            ElseIf AreaTelCode2 <> "" And CountryCode2 = "" Then
                                errmsg += "Row No. " & (i + 2) & ", column CountryCode2 is required." & vbCrLf
                            ElseIf AreaTelCode2 <> "" And Number2 = "" Then
                                errmsg += "Row No. " & (i + 2) & ", column Number2 is required." & vbCrLf

                            ElseIf Number2 <> "" And CountryCode2 = "" And AreaTelCode2 = "" Then
                                errmsg += "Row No. " & (i + 2) & ", column CountryCode2 and Number2 are required." & vbCrLf
                            ElseIf Number2 <> "" And CountryCode2 = "" Then
                                errmsg += "Row No. " & (i + 2) & ", column CountryCode2 is required." & vbCrLf
                            ElseIf Number2 <> "" And AreaTelCode2 = "" Then
                                errmsg += "Row No. " & (i + 2) & ", column Area or Telco Code2 is required." & vbCrLf
                            End If
                        End If

                        If InterestedIn <> "" Then
                            Dim arrInterested As Object = InterestedIn.Split(",")

                            For Each Int As String In arrInterested
                                p = New InsertUpdate("LibraryCRMInterested")
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("ClientID") = ReadyString(ID)
                                p("InterestedIn") = ReadyString(Int)
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                p("LastUpdated") = ReadyString(CDate(DateTime))
                                pSql = p.GetInsert()
                                Common.WebRequest.ProData.ExecuteSQL(pSql)
                            Next
                        End If

                        If IsGuid(ID) Then
                            If CompanyName <> "" And PersonFullName <> "" Then
                                Dim existedOrgID As String
                                Dim ContactPersonID As String = Guid.NewGuid.ToString()
                                pSql = "SELECT TOP 1 ID FROM LibraryOrganization WHERE CompanyName = " & ReadyString(CompanyName)
                                pDT = Common.WebRequest.ProData.Execute(pSql)

                                If pDT.Rows.Count > 0 Then
                                    ID = pDT.Rows(0)("ID").ToString()
                                Else
                                    p = New InsertUpdate("LibraryOrganization")
                                    p("ID") = ReadyString(ID)
                                    p("CompanyName") = ReadyString(CompanyName)
                                    p("OfficeAddressBuildingNo") = ReadyString(officeaddressbuildingno)
                                    p("OfficeAddressStreet") = ReadyString(officeaddressstreet)
                                    p("OfficeAddressCityID") = If(officeaddresscityID = "", "NULL", ReadyString(officeaddresscityID))
                                    p("OfficeAddressCountryID") = If(officeaddresscountryid = "", "NULL", ReadyString(officeaddresscountryid))
                                    p("IndustryClassID") = If(IndustryClassID = "", "NULL", ReadyString(IndustryClassID))
                                    p("ContactOwner") = ReadyString(ContactOwner)
                                    p("ContactSource") = ReadyString(ContactSource)
                                    p("InterestedInProduct") = ReadyString(InterestedIn)
                                    pSql = p.GetInsert()
                                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                                End If

                                p = New InsertUpdate("LibraryPeople")
                                p("ID") = ReadyString(ContactPersonID)
                                p("FirstName") = ReadyString(PersonFirstName)
                                p("MiddleName") = ReadyString(PersonMiddleName)
                                p("Surname") = ReadyString(PersonSurname)
                                p("PersonalSuffixName") = ReadyString(PersonSuffixName)
                                p("ContactOwner") = ReadyString(ContactOwner)
                                p("ContactSource") = ReadyString(ContactSource)
                                p("InterestedInProduct") = ReadyString(InterestedIn)
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                p("IsFromOrg") = 1
                                p("LastUpdated") = ReadyString(CDate(DateTime))
                                pSql = p.GetInsert()
                                Common.WebRequest.ProData.ExecuteSQL(pSql)

                                p = New InsertUpdate("LibraryOrganizationContactPerson")
                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("OrganizationID") = ReadyString(ID)
                                p("PeopleID") = ReadyString(ContactPersonID)
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                p("LastUpdated") = ReadyString(CDate(DateTime))
                                p("IsUseForPersonOrgAffiliates") = 0
                                pSql = p.GetInsert()
                                Common.WebRequest.ProData.ExecuteSQL(pSql)

                                If areatelcoType1 = "Tel" Then
                                    p = New InsertUpdate("LibraryPeopleContactTel")
                                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                                    p("PeopleID") = ReadyString(ContactPersonID)
                                    p("CountryID") = ReadyString(countrycode1ID)
                                    p("AreaCodeID") = ReadyString(areacode1ID)
                                    p("ContactNo") = ReadyString(Number1)
                                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                    p("LastUpdated") = ReadyString(CDate(DateTime))
                                    pSql = p.GetInsert()
                                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                                ElseIf areatelcoType1 = "Mobile" Then
                                    p = New InsertUpdate("LibraryPeopleContactMobile")
                                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                                    p("PeopleID") = ReadyString(ContactPersonID)
                                    p("CountryID") = ReadyString(countrycode1ID)
                                    p("TelCoPrefixID") = ReadyString(areacode1ID)
                                    p("ContactNo") = ReadyString(Number1)
                                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                    p("LastUpdated") = ReadyString(CDate(DateTime))
                                    pSql = p.GetInsert()
                                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                                End If

                                If areatelcoType2 = "Tel" Then
                                    p = New InsertUpdate("LibraryPeopleContactTel")
                                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                                    p("PeopleID") = ReadyString(ContactPersonID)
                                    p("CountryID") = ReadyString(countrycode2ID)
                                    p("AreaCodeID") = ReadyString(areacode2ID)
                                    p("ContactNo") = ReadyString(Number2)
                                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                    p("LastUpdated") = ReadyString(CDate(DateTime))
                                    pSql = p.GetInsert()
                                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                                ElseIf areatelcoType2 = "Mobile" Then
                                    p = New InsertUpdate("LibraryPeopleContactMobile")
                                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                                    p("PeopleID") = ReadyString(ContactPersonID)
                                    p("CountryID") = ReadyString(countrycode2ID)
                                    p("TelCoPrefixID") = ReadyString(areacode2ID)
                                    p("ContactNo") = ReadyString(Number2)
                                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                    p("LastUpdated") = ReadyString(CDate(DateTime))
                                    pSql = p.GetInsert()
                                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                                End If

                                If Trim(ContactEMailEmailAddress) <> "" Then
                                    p = New InsertUpdate("LibraryPeopleContactEmail")
                                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                                    p("PeopleID") = ReadyString(ContactPersonID)
                                    p("EmailAddress") = ReadyString(ContactEMailEmailAddress)
                                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                    p("LastUpdated") = ReadyString(CDate(DateTime))
                                    pSql = p.GetInsert()
                                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                                End If

                            ElseIf CompanyName <> "" Then
                                If officeaddresscityID <> "" Then
                                    If Not IsGuid(officeaddresscityID) Then
                                        errmsg += "Row No. " & (i + 2) & ", column City Address ID must be GUID." & vbCrLf
                                    End If
                                End If

                                If officeaddresscountryid <> "" Or officeaddresscityID <> "" Then
                                    If Not IsGuid(officeaddresscountryid) Then
                                        errmsg += "Row No. " & (i + 2) & ", column Country Address ID must be GUID." & vbCrLf
                                    End If
                                End If

                                p = New InsertUpdate("LibraryOrganization")
                                pSql = "SELECT * FROM LibraryOrganization WHERE ID = " & ReadyString(ID)
                                pDT = Common.WebRequest.ProData.Execute(pSql)

                                p("CompanyName") = ReadyString(CompanyName)
                                p("OfficeAddressBuildingNo") = ReadyString(officeaddressbuildingno)
                                p("OfficeAddressStreet") = ReadyString(officeaddressstreet)
                                p("OfficeAddressCityID") = If(officeaddresscityID = "", "NULL", ReadyString(officeaddresscityID))
                                p("OfficeAddressCountryID") = If(officeaddresscountryid = "", "NULL", ReadyString(officeaddresscountryid))
                                p("IndustryClassID") = If(IndustryClassID = "", "NULL", ReadyString(IndustryClassID))
                                p("ContactOwner") = ReadyString(ContactOwner)
                                p("ContactSource") = ReadyString(ContactSource)
                                p("InterestedInProduct") = ReadyString(InterestedIn)

                                If pDT.Rows.Count = 0 Then
                                    p("ID") = ReadyString(ID)
                                    pSql = p.GetInsert()
                                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                                    If areatelcoType1 = "Tel" Then
                                        p = New InsertUpdate("LibraryOrganizationContactTel")
                                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                                        p("OrganizationID") = ReadyString(ID)
                                        p("CountryID") = ReadyString(countrycode1ID)
                                        p("AreaCodeID") = ReadyString(areacode1ID)
                                        p("ContactNo") = ReadyString(Number1)
                                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                        p("LastUpdated") = ReadyString(CDate(DateTime))
                                        pSql = p.GetInsert()
                                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                                    ElseIf areatelcoType1 = "Mobile" Then
                                        p = New InsertUpdate("LibraryOrganizationContactMobile")
                                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                                        p("OrganizationID") = ReadyString(ID)
                                        p("CountryID") = ReadyString(countrycode1ID)
                                        p("TelCoPrefixID") = ReadyString(areacode1ID)
                                        p("ContactNo") = ReadyString(Number1)
                                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                        p("LastUpdated") = ReadyString(CDate(DateTime))
                                        pSql = p.GetInsert()
                                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                                    End If

                                    If areatelcoType2 = "Tel" Then
                                        p = New InsertUpdate("LibraryPeopleContactTel")
                                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                                        p("OrganizationID") = ReadyString(ID)
                                        p("CountryID") = ReadyString(countrycode2ID)
                                        p("AreaCodeID") = ReadyString(areacode2ID)
                                        p("ContactNo") = ReadyString(Number2)
                                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                        p("LastUpdated") = ReadyString(CDate(DateTime))
                                        pSql = p.GetInsert()
                                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                                    ElseIf areatelcoType2 = "Mobile" Then
                                        p = New InsertUpdate("LibraryPeopleContactMobile")
                                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                                        p("OrganizationID") = ReadyString(ID)
                                        p("CountryID") = ReadyString(countrycode2ID)
                                        p("TelCoPrefixID") = ReadyString(areacode2ID)
                                        p("ContactNo") = ReadyString(Number2)
                                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                        p("LastUpdated") = ReadyString(CDate(DateTime))
                                        pSql = p.GetInsert()
                                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                                    End If

                                    If Trim(ContactEMailEmailAddress) <> "" Then
                                        p = New InsertUpdate("LibraryOrganizationContactEmail")
                                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                                        p("OrganizationID") = ReadyString(ID)
                                        p("EmailAddress") = ReadyString(ContactEMailEmailAddress)
                                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                        p("LastUpdated") = ReadyString(CDate(DateTime))
                                        pSql = p.GetInsert()
                                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                                    End If

                                    If Trim(ContactWebsiteWebsite) <> "" Then
                                        p = New InsertUpdate("LibraryOrganizationContactWebsite")

                                        p("OrganizationID") = ReadyString(ID)
                                        p("Website") = ReadyString(ContactWebsiteWebsite)
                                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                        p("LastUpdated") = ReadyString(CDate(DateTime))
                                        pSql = p.GetInsert()
                                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                                    End If

                                    If RepresentativeFirstName <> "" Or RepresentativeMiddleName <> "" Or RepresentativeLastName <> "" Then
                                        Dim repID As String = Guid.NewGuid.ToString()
                                        p = New InsertUpdate("LibraryPeople")
                                        p("ID") = ReadyString(repID)
                                        p("FirstName") = ReadyString(RepresentativeFirstName)
                                        p("MiddleName") = ReadyString(RepresentativeMiddleName)
                                        p("Surname") = ReadyString(RepresentativeLastName)
                                        pSql = p.GetInsert()
                                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                                        p = New InsertUpdate("LibraryOrganizationContactPerson")
                                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                                        p("OrganizationID") = ReadyString(ID)
                                        p("PeopleID") = ReadyString(repID)
                                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                        p("IsUseForPersonOrgAffiliates") = 0
                                        pSql = p.GetInsert()
                                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                                    End If

                                Else
                                    pSql = p.GetUpdate(" ID = " & ReadyString(ID))
                                    Common.WebRequest.ProData.ExecuteSQL(pSql)
                                End If

                            Else
                                p = New InsertUpdate("LibraryPeople")
                                pSql = "SELECT * FROM LibraryPeople WHERE ID = " & ReadyString(ID)
                                pDT = Common.WebRequest.ProData.Execute(pSql)

                                p("FirstName") = ReadyString(PersonFirstName)
                                p("MiddleName") = ReadyString(PersonMiddleName)
                                p("Surname") = ReadyString(PersonSurname)
                                p("PersonalSuffixName") = ReadyString(PersonSuffixName)
                                p("ContactOwner") = ReadyString(ContactOwner)
                                p("ContactSource") = ReadyString(ContactSource)
                                p("InterestedInProduct") = ReadyString(InterestedIn)
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                p("LastUpdated") = ReadyString(CDate(DateTime))
                                p("IsFromOrg") = 0

                                If pDT.Rows.Count = 0 Then
                                    p("ID") = ReadyString(ID)
                                    pSql = p.GetInsert()
                                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                                    If areatelcoType1 = "Tel" Then
                                        p = New InsertUpdate("LibraryPeopleContactTel")
                                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                                        p("PeopleID") = ReadyString(ID)
                                        p("CountryID") = ReadyString(countrycode1ID)
                                        p("AreaCodeID") = ReadyString(areacode1ID)
                                        p("ContactNo") = ReadyString(Number1)
                                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                        p("LastUpdated") = ReadyString(CDate(DateTime))
                                        pSql = p.GetInsert()
                                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                                    ElseIf areatelcoType1 = "Mobile" Then
                                        p = New InsertUpdate("LibraryPeopleContactMobile")
                                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                                        p("PeopleID") = ReadyString(ID)
                                        p("CountryID") = ReadyString(countrycode1ID)
                                        p("TelCoPrefixID") = ReadyString(areacode1ID)
                                        p("ContactNo") = ReadyString(Number1)
                                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                        p("LastUpdated") = ReadyString(CDate(DateTime))
                                        pSql = p.GetInsert()
                                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                                    End If

                                    If areatelcoType2 = "Tel" Then
                                        p = New InsertUpdate("LibraryPeopleContactTel")
                                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                                        p("PeopleID") = ReadyString(ID)
                                        p("CountryID") = ReadyString(countrycode2ID)
                                        p("AreaCodeID") = ReadyString(areacode2ID)
                                        p("ContactNo") = ReadyString(Number2)
                                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                        p("LastUpdated") = ReadyString(CDate(DateTime))
                                        pSql = p.GetInsert()
                                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                                    ElseIf areatelcoType2 = "Mobile" Then
                                        p = New InsertUpdate("LibraryPeopleContactMobile")
                                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                                        p("PeopleID") = ReadyString(ID)
                                        p("CountryID") = ReadyString(countrycode2ID)
                                        p("TelCoPrefixID") = ReadyString(areacode2ID)
                                        p("ContactNo") = ReadyString(Number2)
                                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                        p("LastUpdated") = ReadyString(CDate(DateTime))
                                        pSql = p.GetInsert()
                                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                                    End If

                                    If Trim(ContactEMailEmailAddress) <> "" Then
                                        p = New InsertUpdate("LibraryPeopleContactEmail")
                                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                                        p("PeopleID") = ReadyString(ID)
                                        p("EmailAddress") = ReadyString(ContactEMailEmailAddress)
                                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                        p("LastUpdated") = ReadyString(CDate(DateTime))
                                        pSql = p.GetInsert()
                                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                                    End If

                                    If officeaddresscityID <> "" Or officeaddresscountryid <> "" Or officeaddressbuildingno <> "" Or officeaddressstreet <> "" Then
                                        If IsGuid(officeaddresscityID) And IsGuid(officeaddresscountryid) Then
                                            p = New InsertUpdate("LibraryPeopleAddress")

                                            If IsGuid(officeaddresscityID) Then
                                                p("CityID") = ReadyString(officeaddresscityID)
                                            Else
                                                pSql = "SELECT * FROM MobizLibraryCities WHERE IsActive = 1 AND Name LIKE " & ReadyStringWild(City)
                                                pDT = Common.WebRequest.ProData.Execute(pSql)

                                                If pDT.Rows.Count = 1 Then
                                                    p("CityID") = pDT.Rows(0)("ID")
                                                ElseIf pDT.Rows.Count > 1 Then
                                                    errmsg += "We found multiple results in column City Address: "
                                                    For cityI As Integer = 0 To pDT.Rows.Count - 1
                                                        errmsg += pDT.Rows(0)("Name") & ", " & vbCrLf
                                                    Next
                                                Else
                                                    errmsg += "No result found for city name: " & City & vbCrLf
                                                End If
                                            End If

                                            If IsGuid(officeaddresscountryid) Then
                                                p("CountryID") = ReadyString(officeaddresscountryid)
                                            Else
                                                pSql = "SELECT * FROM MobizLibraryCountries WHERE Name LIKE " & ReadyStringWild(Country) & " AND Code LIKE " & ReadyStringWild(Country) & " AND Alpha2Code LIKE " & ReadyStringWild(Country)
                                                pDT = Common.WebRequest.ProData.Execute(pSql)

                                                If pDT.Rows.Count = 1 Then
                                                    p("CountryID") = pDT.Rows(0)("ID")
                                                ElseIf pDT.Rows.Count > 1 Then
                                                    errmsg += "We found multiple results in column Country Address: "
                                                    For cityI As Integer = 0 To pDT.Rows.Count - 1
                                                        errmsg += "(" & pDT.Rows(0)("Code") & ", " & pDT.Rows(0)("Name") & "), " & vbCrLf
                                                    Next
                                                Else
                                                    errmsg += "No result found for country name: " & Country & vbCrLf
                                                End If
                                            End If

                                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                                            p("PeopleID") = ReadyString(ID)
                                            p("HouseBuildingNo") = ReadyString(officeaddressbuildingno)
                                            p("Street") = ReadyString(officeaddressstreet)
                                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                            p("LastUpdated") = ReadyString(CDate(DateTime))
                                            pSql = p.GetInsert()
                                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                                        Else
                                            errmsg += "Row No. " & (i + 2) & ", column City Address ID or Country Address ID must be GUID." & vbCrLf
                                        End If
                                    End If

                                Else
                                    pSql = p.GetUpdate(" ID = " & ReadyString(ID))
                                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                                    pSql = "SELECT ID FROM LibraryPeopleAddress WHERE PeopleID = " & ReadyString(ID)
                                    pDT = Common.WebRequest.ProData.Execute(pSql)

                                    If pDT.Rows.Count > 0 Then
                                        If IsGuid(officeaddresscityID) And IsGuid(officeaddresscountryid) Then
                                            p = New InsertUpdate("LibraryPeopleAddress")
                                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                                            p("PeopleID") = ReadyString(ID)
                                            p("HouseBuildingNo") = ReadyString(officeaddressbuildingno)
                                            p("Street") = ReadyString(officeaddressstreet)
                                            p("CityID") = ReadyString(officeaddresscityID)
                                            p("CountryID") = ReadyString(officeaddresscountryid)
                                            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                            p("LastUpdated") = ReadyString(CDate(DateTime))
                                            pSql = p.GetUpdate(" ID = " & ReadyString(pDT.Rows(0)("ID")))
                                            Common.WebRequest.ProData.ExecuteSQL(pSql)
                                        Else
                                            errmsg += "Row No. " & (i + 2) & ", column City Address ID or Country Address ID must be GUID." & vbCrLf
                                        End If
                                    End If
                                End If
                            End If

                            savemsg = "saved"
                        Else
                            errmsg += "Row No. " & (i + 2) & ", column ID must be GUID." & vbCrLf
                        End If
                    Next

                    If errmsg = "" Then
                        Common.WebRequest.ProData.CommitTransaction()
                    End If

                    Return Json(New With {Key .Save = savemsg, .Error = errmsg}, JsonRequestBehavior.AllowGet)
                Catch ex As Exception
                    Return Json(New With {Key .Save = "", .Error = ex.Message}, JsonRequestBehavior.AllowGet)
                End Try
            End If
        End Function

        Function FollowupUpdateUploadContacts() As String
            Dim w As String = "hi how are you"
            If w.Length > 0 Then
                Dim p As Integer = w.LastIndexOf(" ") + 1

                Return w.Remove(p)
            End If
        End Function

        <HttpPost()>
        Function IsNewEntry() As String
            Dim pOut As New JsonDictionary

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            pOut.Add("IsNew", IIf(Convert.ToBoolean(WebSession.Instance.IsNew), "TRUE", "FALSE"))
            pOut.Add("PeopleID", WebSession.Instance.PeopleID)

            Return pOut.Serialize

        End Function

        <HttpPost()>
        Function setSignOut() As String
            Dim strReturn As String

            WebSession.Instance.IsNew = 0
            WebSession.Instance.PeopleID = Nothing
            WebSession.Instance.PeopleUserID = Nothing
            WebSession.Instance.FirstName = ""
            WebSession.Instance.FullName = ""

            strReturn = JsonDictionary.Serialize("SignOut", "TRUE")

            Return strReturn

        End Function

        <HttpPost()>
        Function getCityNameList() As String
            Dim strReturn As String
            Dim dataTblCityList As DataTable

            dataTblCityList = Common.WebRequest.ProData.Execute("SELECT ID, Name FROM LibraryCities ORDER BY Name")


            strReturn = JsonDictionary.Serialize("CityData", dataTblCityList)

            Return strReturn

        End Function

        <HttpPost()>
        Function getCountryNameList() As String
            Dim strReturn As String
            Dim dataTblCityList As DataTable

            dataTblCityList = Common.WebRequest.ProData.Execute("SELECT ID, Name FROM LibraryCountries ORDER BY Name")


            strReturn = JsonDictionary.Serialize("CountryData", dataTblCityList)

            Return strReturn

        End Function

        <HttpPost()>
        Function GetProspectList() As Object
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim pDT As DataTable
            Dim clsCRMDashboard As New CRMDashboard
            Dim jss = New JavaScriptSerializer()
            Dim ctr As Integer = 0
            clsCRMDashboard.PageStart = CInt(Request.Form.GetValues("start")(0))
            clsCRMDashboard.PageNoOfRows = CInt(Request.Form.GetValues("length")(0))
            clsCRMDashboard.PageSearchText = Request.Form.GetValues("search[value]")(0).ToString()
            clsCRMDashboard.IsCountPage = False
            clsCRMDashboard.PageColumnNo = Request.Form.GetValues("order[0][column]")(0).ToString()
            clsCRMDashboard.PageColumnDirection = Request.Form.GetValues("order[0][dir]")(0).ToString()
            clsCRMDashboard.Status = Request.Form.GetValues("columns[0][search][value]")(0).ToString()
            clsCRMDashboard.Filter = Request.Form.GetValues("columns[1][search][value]")(0).ToString()
            clsCRMDashboard.YearWeek = DateRange.GetYearWeek(Now, -CInt2(Request.Params.Get("madewithin")))
            clsCRMDashboard.LastWeek = DateRange.GetYearWeek(Now, -1)
            clsCRMDashboard.CrmType = Request.Params.Get("CrmType")
            clsCRMDashboard.InvType = Request.Params.Get("invtype")

            If Request.Params.Get("mod") = "crm" Then
                pDT = clsCRMDashboard.GetProspectList()
            Else
                pDT = clsCRMDashboard.GetListOfEmployees()
            End If

            Dim a(pDT.Rows.Count - 1) As Object

            For m As Integer = 0 To pDT.Rows.Count - 1
                If pDT.Rows(m).Table.Columns("Error") IsNot Nothing Then
                    Return pDT.Rows(m)("ERROR")
                End If

                a(m) = New With {Key .ID = pDT.Rows(m)("ID"),
                                 .ProspectType = pDT.Rows(m)("ProspectType").ToString(),
                                 .ClientName = pDT.Rows(m)("ClientName"),
                                 .ContactPerson = pDT.Rows(m)("ContactPerson"),
                                 .ContactInfo = If(pDT.Rows(m)("Telephone").ToString() <> "", "<div>T: " & pDT.Rows(m)("Telephone") & "</div>", "") &
                                                If(pDT.Rows(m)("Mobile").ToString() <> "", "<div> M: " & pDT.Rows(m)("Mobile") & "</div>", "") &
                                                If(pDT.Rows(m)("Email").ToString() <> "", "<div> E: " & pDT.Rows(m)("Email") & "</div>", ""),
                                 .City = pDT.Rows(m)("City"),
                                 .Country = pDT.Rows(m)("Country"),
                                 .Affiliation = pDT.Rows(m)("Affiliation"),
                                 .LastInteractionDate = pDT.Rows(m)("LastInteractionDate"),
                                 .Status = If(pDT.Rows(m)("Status") = "1", "Active", "Deactive")}
                ctr = ctr + 1
            Next

            Dim pSql As String = "SELECT COUNT(ID) as Count FROM ( SELECT ID FROM LibraryOrganization UNION ALL SELECT ID FROM LibraryPeople WHERE IsFromOrg = 0 ) as v2"
            clsCRMDashboard.IsCountPage = True
            Dim pDTCountPage As DataTable = IIf(Request.Params.Get("mod") = "crm", clsCRMDashboard.GetProspectList(), clsCRMDashboard.GetListOfEmployees()) 'Common.WebRequest.ProData.Execute(pSql)

            Return Json(New With {Key .data = jss.Deserialize(Of Object)(clsCRMDashboard.GetJson(pDT)), .recordsTotal = pDTCountPage.Rows().Count, .recordsFiltered = pDTCountPage.Rows().Count}, JsonRequestBehavior.AllowGet)

        End Function

        <HttpPost()>
        Function SearchClientContact() As Object
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard
            Dim pDT As DataTable
            Dim results As Object

            clsCRMDashboard.IsCountPage = False
            'clsCRMDashboard.PageColumnNo = Request.Form.GetValues("order[0][column]")(0).ToString()
            'clsCRMDashboard.PageColumnDirection = Request.Form.GetValues("order[0][dir]")(0).ToString()
            'clsCRMDashboard.PageStart = CInt(Request.Form.GetValues("start")(0))
            'clsCRMDashboard.PageNoOfRows = CInt(Request.Form.GetValues("length")(0))
            clsCRMDashboard.SearchContact = Common.WebRequest.Data("SearchContact")
            results = clsCRMDashboard.SearchClientContact()
            clsCRMDashboard.IsCountPage = True
            pDT = clsCRMDashboard.SearchClientContact()
            'Return Json(New With {Key .data = jss.Deserialize(Of Object)(clsCRMDashboard.GetJson(pDT)), .recordsTotal = pDTCountPage.Rows().Count, .recordsFiltered = pDTCountPage.Rows().Count}, JsonRequestBehavior.AllowGet)
            'strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.SearchClientContact())

            Return Json(New With {Key .data = results, .recordcount = pDT.Rows.Count, .records = jss.Deserialize(Of Object)(clsCRMDashboard.GetJson(pDT))}, JsonRequestBehavior.AllowGet)

        End Function

        <HttpPost()>
        Function GetProspectDetails() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard

            clsCRMDashboard.OrgID = Common.WebRequest.Data("ID")

            strReturn = clsCRMDashboard.GetProspectDetails()

            Return strReturn

        End Function

        <HttpPost()>
        Function GetProspectDetailsPeople() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard

            clsCRMDashboard.CurrentModule = Request.Params.Get("mod")
            clsCRMDashboard.PersonID = Common.WebRequest.Data("ID")

            strReturn = clsCRMDashboard.GetProspectDetailsPeople()

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveNewOrgProspect() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard

            clsCRMDashboard.OrgID = Common.WebRequest.Data("OrgID")
            clsCRMDashboard.CompanyID = Common.WebRequest.Data("CompanyID")
            clsCRMDashboard.CompanyName = Common.WebRequest.Data("CompanyName")
            clsCRMDashboard.CompanyBuildingAddress = Common.WebRequest.Data("CompanyBuildingAddress")
            clsCRMDashboard.CompanyStreetAddress = Common.WebRequest.Data("CompanyStreetAddress")
            clsCRMDashboard.CompanyCityAddress = Common.WebRequest.Data("CompanyCityAddress")
            clsCRMDashboard.CompanyRegionAddress = Common.WebRequest.Data("CompanyRegionAddress")
            clsCRMDashboard.Island = Common.WebRequest.Data("Island")
            clsCRMDashboard.CompanyCountryAddress = Common.WebRequest.Data("CompanyCountryAddress")
            clsCRMDashboard.CompanyBackground = Common.WebRequest.Data("CompanyBackground")
            clsCRMDashboard.CompanyMission = Common.WebRequest.Data("CompanyMission")
            clsCRMDashboard.CompanyVision = Common.WebRequest.Data("CompanyVision")
            clsCRMDashboard.CompanyFoundingDate = Common.WebRequest.Data("CompanyFoundingDate")
            clsCRMDashboard.BusinessIndustryID = Common.WebRequest.Data("BusinessIndustryID")
            clsCRMDashboard.ObjTelephone = Common.WebRequest.AllData("Telphone")
            clsCRMDashboard.ObjMobile = Common.WebRequest.AllData("Mobile")
            clsCRMDashboard.ObjEmail = Common.WebRequest.AllData("Email")
            clsCRMDashboard.ObjWebsite = Common.WebRequest.AllData("Website")
            clsCRMDashboard.ObjContactPerson = Common.WebRequest.AllData("ContactPerson")
            clsCRMDashboard.ObjContactPersonTelephone = Common.WebRequest.AllData("ContactPersonTelphone")
            clsCRMDashboard.ObjContactPersonMobile = Common.WebRequest.AllData("ContactPersonMobile")
            clsCRMDashboard.ObjContactPersonEmail = Common.WebRequest.AllData("ContactPersonEmail")
            clsCRMDashboard.ObjOrgAff = Common.WebRequest.AllData("OrgAffiliation")
            clsCRMDashboard.ObjAffiliatedPerson = Common.WebRequest.AllData("OrgAffiliatedPerson")
            clsCRMDashboard.ObjAddress = Common.WebRequest.AllData("Address")
            clsCRMDashboard.DateTime = Common.WebRequest.Data("DateNow")
            clsCRMDashboard.Status = Common.WebRequest.Data("Status")
            clsCRMDashboard.NoOfEmployees = IIf(Common.WebRequest.Data("NoOfEmployees") = "", "NULL", Common.WebRequest.Data("NoOfEmployees"))
            clsCRMDashboard.RevenuePerYear = IIf(Common.WebRequest.Data("Revenue") = "", "NULL", Common.WebRequest.Data("Revenue"))
            clsCRMDashboard.Scope = Common.WebRequest.Data("Scope")
            clsCRMDashboard.NoOfFactories = IIf(Common.WebRequest.Data("Factories") = "", "NULL", Common.WebRequest.Data("Factories"))
            clsCRMDashboard.NoOfOffices = IIf(Common.WebRequest.Data("Offices") = "", "NULL", Common.WebRequest.Data("Offices"))
            clsCRMDashboard.NoOfWarehouses = IIf(Common.WebRequest.Data("Warehouses") = "", "NULL", Common.WebRequest.Data("Warehouses"))
            clsCRMDashboard.NoOfStores = IIf(Common.WebRequest.Data("Stores") = "", "NULL", Common.WebRequest.Data("Stores"))
            clsCRMDashboard.NoOfTrucks = IIf(Common.WebRequest.Data("Trucks") = "", "NULL", Common.WebRequest.Data("Trucks"))

            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.SaveNewOrgProspect())

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveNewPersonProspect() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard

            clsCRMDashboard.CurrentModule = Request.Params.Get("mod")
            clsCRMDashboard.OrgID = Common.WebRequest.Data("OrgID")
            clsCRMDashboard.PersonID = Common.WebRequest.Data("PersonID")
            clsCRMDashboard.FirstName = Common.WebRequest.Data("FirstName")
            clsCRMDashboard.MiddleName = Common.WebRequest.Data("MiddleName")
            clsCRMDashboard.LastName = Common.WebRequest.Data("LastName")
            clsCRMDashboard.Birthday = Common.WebRequest.Data("Birthday")
            clsCRMDashboard.Position = Common.WebRequest.Data("Position")
            clsCRMDashboard.PositionDescription = Common.WebRequest.Data("PositionDescription")
            clsCRMDashboard.AddressID = Common.WebRequest.Data("AddressID")
            clsCRMDashboard.HouseBuildingNo = Common.WebRequest.Data("HouseBuildingNo")
            clsCRMDashboard.Street = Common.WebRequest.Data("Street")
            clsCRMDashboard.City = Common.WebRequest.Data("City")
            clsCRMDashboard.Region = Common.WebRequest.Data("Region")
            clsCRMDashboard.Island = Common.WebRequest.Data("Island")
            clsCRMDashboard.Country = Common.WebRequest.Data("Country")
            clsCRMDashboard.TypeAddress = Common.WebRequest.Data("TypeAddress")
            clsCRMDashboard.ObjTelephone = Common.WebRequest.AllData("Telphone")
            clsCRMDashboard.ObjMobile = Common.WebRequest.AllData("Mobile")
            clsCRMDashboard.ObjEmail = Common.WebRequest.AllData("Email")
            clsCRMDashboard.ObjPersonOrgAff = Common.WebRequest.AllData("OrgAffiliation")
            clsCRMDashboard.DateTime = Common.WebRequest.Data("DateNow")
            clsCRMDashboard.Status = Common.WebRequest.Data("Status")
            clsCRMDashboard.ObjPersonElementary = Common.WebRequest.AllData("Elementary")
            clsCRMDashboard.ObjPersonHighSchool = Common.WebRequest.AllData("HighSchool")
            clsCRMDashboard.ObjPersonCollege = Common.WebRequest.AllData("College")

            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.SaveNewPersonProspect())

            Return strReturn

        End Function

        <HttpPost()>
        Function GetContactDetailsOrg() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard

            clsCRMDashboard.OrgID = Common.WebRequest.Data("ID")

            strReturn = clsCRMDashboard.GetContactDetailsOrg()

            Return strReturn

        End Function

        <HttpPost()>
        Function GetContactDetailsPeople() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard

            clsCRMDashboard.PersonID = Common.WebRequest.Data("ID")

            strReturn = clsCRMDashboard.GetContactDetailsPeople()

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveColdCall() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard

            clsCRMDashboard.ObjClient = Common.WebRequest.AllData("Client")
            clsCRMDashboard.CallNotes = Common.WebRequest.Data("Notes")
            clsCRMDashboard.CallStartDate = Common.WebRequest.Data("CallStartDate")
            clsCRMDashboard.CallEndDate = Common.WebRequest.Data("CallEndDate")
            clsCRMDashboard.CallDuration = Common.WebRequest.Data("CallDuration")
            clsCRMDashboard.IsOrg = Common.WebRequest.Data("IsOrg")
            clsCRMDashboard.DateTime = Common.WebRequest.Data("DateNow")

            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.SaveColdCall())

            Return strReturn

        End Function

        <HttpPost()>
        Function GetInteractionHistory() As Object
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim pDT As DataTable
            Dim clsCRMDashboard As New CRMDashboard
            Dim jss = New JavaScriptSerializer()
            Dim ctr As Integer = 0
            clsCRMDashboard.ClientID = CStr2(Request.Params("ID"))
            clsCRMDashboard.Filter = CStr2(Request.Params("Filter"))
            clsCRMDashboard.PageStart = CInt(Request.Form.GetValues("start")(0))
            clsCRMDashboard.PageNoOfRows = CInt(Request.Form.GetValues("length")(0))
            clsCRMDashboard.PageSearchText = Request.Form.GetValues("search[value]")(0).ToString()
            clsCRMDashboard.IsCountPage = False
            clsCRMDashboard.PageColumnNo = Request.Form.GetValues("order[0][column]")(0).ToString()
            clsCRMDashboard.PageColumnDirection = Request.Form.GetValues("order[0][dir]")(0).ToString()
            pDT = clsCRMDashboard.GetInteractionHistory()

            If pDT.Rows.Count > 0 Then
                If pDT.Rows(0).Table.Columns("Error") IsNot Nothing Then
                    Return pDT.Rows(0)("ERROR")
                End If
            End If

            clsCRMDashboard.IsCountPage = True
            Dim pDTCountPage As DataTable = clsCRMDashboard.GetInteractionHistory()

            Return Json(New With {Key .data = jss.Deserialize(Of Object)(clsCRMDashboard.GetJson(pDT)), .recordsTotal = pDTCountPage.Rows().Count, .recordsFiltered = pDTCountPage.Rows().Count}, JsonRequestBehavior.AllowGet)

        End Function

        <HttpPost()>
        Function SaveToDo() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard

            clsCRMDashboard.ObjToDo = Common.WebRequest.AllData("ToDo")
            clsCRMDashboard.DateTime = Common.WebRequest.Data("DateNow")

            strReturn = clsCRMDashboard.SaveToDo()

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveTakeANote() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard

            clsCRMDashboard.ClientID = Common.WebRequest.Data("ClientID")
            clsCRMDashboard.Note = Common.WebRequest.Data("Note")
            clsCRMDashboard.DateTime = Common.WebRequest.Data("Date")
            'clsCRMDashboard.Attachments = Common.WebRequest.AllData("Attachments")

            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.SaveTakeANote())

            Return strReturn

        End Function

        <HttpPost()>
        Function GetTakeANote() As Object
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard

            clsCRMDashboard.ClientID = Common.WebRequest.Data("ClientID")

            Dim pDT As DataTable = clsCRMDashboard.GetTakeANote()
            Dim arrNote(pDT.Rows.Count - 1) As Object
            Dim jss = New JavaScriptSerializer()
            'pDT.Columns.Add("Pics")
            For i As Integer = 0 To pDT.Rows.Count - 1
                Dim pDTPics As DataTable = clsCRMDashboard.GetAttachedFiles(pDT.Rows(i)("ID").ToString())
                arrNote(i) = New With {Key .ID = pDT.Rows(i)("ID").ToString(),
                                        .TaggedID = pDT.Rows(i)("TaggedID"),
                                        .TaggedType = pDT.Rows(i)("TaggedType"),
                                        .TaggedDetails = pDT.Rows(i)("TaggedDetails"),
                                        .Note = pDT.Rows(i)("Note"),
                                        .UpdatedBy = pDT.Rows(i)("UpdatedBy"),
                                        .LastUpdated = Format(pDT.Rows(i)("LastUpdated"), "MMM dd, yyyy hh:mm tt"),
                                        .Attachments = jss.Deserialize(Of Object)(clsCRMDashboard.GetJson(pDTPics))}

            Next

            'strReturn = JsonDictionary.Serialize("Results", )

            Return Json(New With {Key .data = arrNote}, JsonRequestBehavior.AllowGet)

        End Function

        Function GetNote()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", New CRMDashboard().GetNote())
        End Function

        <HttpPost()>
        Function GetTickets() As Object
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim pDT As DataTable
            Dim clsCRMDashboard As New CRMDashboard
            Dim jss = New JavaScriptSerializer()
            Dim ctr As Integer = 0
            clsCRMDashboard.Filter = CStr2(Request.Params("Filter"))
            clsCRMDashboard.ClientID = CStr2(Request.Params("ClientID"))

            If CBool(Request.Params("IsDatatable")) = True Then
                clsCRMDashboard.PageStart = CInt(Request.Form.GetValues("start")(0))
                clsCRMDashboard.PageNoOfRows = CInt(Request.Form.GetValues("length")(0))
                clsCRMDashboard.PageSearchText = Request.Form.GetValues("search[value]")(0).ToString()
                clsCRMDashboard.IsCountPage = False
                clsCRMDashboard.PageColumnNo = Request.Form.GetValues("order[0][column]")(0).ToString()
                clsCRMDashboard.PageColumnDirection = Request.Form.GetValues("order[0][dir]")(0).ToString()
            End If

            pDT = clsCRMDashboard.GetTickets()

            If pDT.Rows.Count > 0 Then
                If pDT.Rows(0).Table.Columns("Error") IsNot Nothing Then
                    Return pDT.Rows(0)("ERROR")
                End If
            End If

            If CBool(Request.Params("IsDatatable")) = True Then
                clsCRMDashboard.IsCountPage = True
                Dim pDTCountPage As DataTable = clsCRMDashboard.GetTickets()

                Return Json(New With {Key .data = jss.Deserialize(Of Object)(clsCRMDashboard.GetJson(pDT)), .recordsTotal = pDTCountPage.Rows().Count, .recordsFiltered = pDTCountPage.Rows().Count}, JsonRequestBehavior.AllowGet)
            Else
                Return Json(New With {Key .Results = New With {Key .Rows = jss.Deserialize(Of Object)(clsCRMDashboard.GetJson(pDT))}})
            End If

        End Function

        Function TicketList() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = "SELECT DISTINCT crm.ID AS ID , [Status] " & vbCrLf &
                                " , crm.TicketNumber AS [No] , crm.TicketType AS [Type] , [ProductServices] " & vbCrLf &
                                " FROM TransCRM as crm " & vbCrLf &
                                " INNER JOIN (SELECT MAX(LastUpdated) as LastUpdated, TicketNumber FROM TransCRM WHERE ActionName = 'Ticket' GROUP BY TicketNumber) as updated " & vbCrLf &
                                "   ON crm.TicketNumber = updated.TicketNumber and CONVERT(datetime, crm.LastUpdated) = updated.LastUpdated " & vbCrLf &
                                " WHERE ClientID = " & ReadyString(Common.WebRequest.Data("ClientID")) & vbCrLf &
                                " AND ActionName = 'Ticket' AND CONVERT(bit, ISNULL(crm.IsDelete, 0)) <> 1 "
            Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", pDT)
        End Function

        <HttpPost()>
        Function GetTicketsHistory() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard

            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.GetTicketsHistory())

            Return strReturn
        End Function

        <HttpPost()>
        Function GetOppComments() As Object
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard
            Dim pDT As DataTable = clsCRMDashboard.GetOppComments()
            Dim arrNote(pDT.Rows.Count - 1) As Object
            Dim jss = New JavaScriptSerializer()
            'pDT.Columns.Add("Pics")
            For i As Integer = 0 To pDT.Rows.Count - 1
                Dim pDTPics As DataTable = clsCRMDashboard.GetAttachedFiles(pDT.Rows(i)("ID").ToString())
                arrNote(i) = New With {Key .ID = pDT.Rows(i)("ID").ToString(),
                                        .Comment = pDT.Rows(i)("Comment"),
                                        .ActionName = pDT.Rows(i)("ActionName"),
                                        .CommentedBy = pDT.Rows(i)("CommentedBy"),
                                        .CommentedDate = Format(pDT.Rows(i)("CommentedDate"), "MMM dd, yyyy hh:mm tt"),
                                        .Attachments = jss.Deserialize(Of Object)(clsCRMDashboard.GetJson(pDTPics))}

            Next

            'strReturn = JsonDictionary.Serialize("Results", )

            Return Json(New With {Key .data = arrNote}, JsonRequestBehavior.AllowGet)
        End Function

        <HttpPost()>
        Function GetTicketComments() As Object
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim dborgid, dbname As String

            If CBool2(Common.WebRequest.Data("ViewTicketFromMobiz")) Then
#If DEBUG Then
                'MobizABC dev db
                dborgid = "cfe39df6-8809-4973-9515-810df75aed12"
                dbname = WebRequest.Instance.Business.LibraryOrganization.ByID(dborgid)("Database")
#Else
                '1Mobiz prod db 
                dborgid = "84e972ed-f208-4867-9506-5b8f9572562f"
                dbname = WebRequest.Instance.Business.LibraryOrganization.ByID(dborgid)("Database")
#End If
                Common.WebRequest.ProData.Execute("USE " & EscapeSqlName(dbname))
            End If

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard
            Dim pDT As DataTable = clsCRMDashboard.GetTicketComments()
            Dim arrNote(pDT.Rows.Count - 1) As Object
            Dim jss = New JavaScriptSerializer()
            'pDT.Columns.Add("Pics")
            For i As Integer = 0 To pDT.Rows.Count - 1
                Dim pDTPics As DataTable = clsCRMDashboard.GetAttachedFiles(pDT.Rows(i)("ID").ToString())
                arrNote(i) = New With {Key .ID = pDT.Rows(i)("ID").ToString(),
                                        .Comment = pDT.Rows(i)("Comment"),
                                        .ActionName = pDT.Rows(i)("ActionName"),
                                        .CommentedBy = pDT.Rows(i)("CommentedBy"),
                                        .CommentedDate = Format(pDT.Rows(i)("CommentedDate"), "MMM dd, yyyy hh:mm tt"),
                                        .Attachments = jss.Deserialize(Of Object)(clsCRMDashboard.GetJson(pDTPics))}

            Next

            'strReturn = JsonDictionary.Serialize("Results", )

            Return Json(New With {Key .data = arrNote}, JsonRequestBehavior.AllowGet)
        End Function

        <HttpPost()>
        Function SaveOppComment() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard
            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.SaveOppComment())

            Return strReturn
        End Function

        <HttpPost()>
        Function SaveTicketComment() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard
            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.SaveTicketComment())

            Return strReturn
        End Function

        <HttpPost()>
        Function SaveTicket() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard

            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.SaveTicket(Common.WebRequest.AllData("data")))

            Return strReturn

        End Function

        <HttpPost()>
        Function GetClientToDo() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard

            clsCRMDashboard.ClientID = Common.WebRequest.Data("ClientID")
            clsCRMDashboard.UnfinishToDo = If(Common.WebRequest.Data("UnfinishToDo") = "", False, True)

            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.GetClientToDo())

            Return strReturn

        End Function

        Function GetTodoAssignedEmployee()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim todoid As String = Common.WebRequest.Data("TodoID")
            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute("SELECT todo.*, ppol.FirstName + ' ' + ppol.Surname as PeopleName " & vbCrLf &
                                                                        " FROM TransCRMTodoAssignedEmployee as todo " & vbCrLf &
                                                                        " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = todo.EmployeeID " & vbCrLf &
                                                                        " WHERE todo.TodoID = " & ReadyString(todoid)))
        End Function

        <HttpPost()>
        Function GetSchedule() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard

            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.GetSchedule())

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveSchedAnAction() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard

            clsCRMDashboard.ClientID = Common.WebRequest.Data("ClientID")
            clsCRMDashboard.Location = Common.WebRequest.Data("Location")
            clsCRMDashboard.Note = Common.WebRequest.Data("Note")
            clsCRMDashboard.NextActionName = Common.WebRequest.Data("NextActionName")
            clsCRMDashboard.Subject = Common.WebRequest.Data("Subject")
            clsCRMDashboard.IsAllDay = Common.WebRequest.Data("IsAllDay")
            clsCRMDashboard.DateTimeFrom = Common.WebRequest.Data("DateTimeFrom")
            clsCRMDashboard.DateTimeTo = Common.WebRequest.Data("DateTimeTo")
            clsCRMDashboard.TeamAssign = Common.WebRequest.Data("TeamAssign")
            clsCRMDashboard.DateTime = Common.WebRequest.Data("Date")

            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.SaveSchedAnAction())

            Return strReturn

        End Function

        Function SmSUsage() As ActionResult
            Return View()
        End Function

        Function GetTotalSmSUsage() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard

            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.GetTotalSmSUsage())

            Return strReturn
        End Function

        Function GetSmSUsage() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard

            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.GetSmSUsage())

            Return strReturn
        End Function

        Function GetSmSUsagePerTelco() As Object
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim pDT As DataTable
            Dim clsCRMDashboard As New CRMDashboard
            Dim jss = New JavaScriptSerializer()
            Dim ctr As Integer = 0
            clsCRMDashboard.PageStart = CInt(Request.Form.GetValues("start")(0))
            clsCRMDashboard.PageNoOfRows = CInt(Request.Form.GetValues("length")(0))
            clsCRMDashboard.PageSearchText = Request.Form.GetValues("search[value]")(0).ToString()
            clsCRMDashboard.PageColumnNo = Request.Form.GetValues("order[0][column]")(0).ToString()
            clsCRMDashboard.PageColumnDirection = Request.Form.GetValues("order[0][dir]")(0).ToString()
            clsCRMDashboard.IsCountPage = False

            pDT = clsCRMDashboard.GetSmSUsagePerTelco()

            clsCRMDashboard.IsCountPage = True
            Dim pDTCountPage As DataTable = clsCRMDashboard.GetSmSUsagePerTelco()

            Return Json(New With {Key .data = jss.Deserialize(Of Object)(clsCRMDashboard.GetJson(pDT)), .recordsTotal = pDTCountPage.Rows().Count, .recordsFiltered = pDTCountPage.Rows().Count}, JsonRequestBehavior.AllowGet)
        End Function

        Function DownloadContactTemplate() As ActionResult
            Dim pFS As IO.FileStream
            Dim pContentType As String
            Dim pSourceURL As String
            pFS = New IO.FileStream(Common.ServerPath & "/temp/Contacts/ContactsTemplate.xlsx", IO.FileMode.Open)
            pSourceURL = Common.ServerPath & "temp\Contacts\ContactsTemplate.xlsx"
            pContentType = "application/xls"

            Try
                If pFS IsNot Nothing Then pFS.Dispose()
            Catch ex As Exception
            End Try


            Return File(pSourceURL, pContentType, "ContactsTemplate.xlsx")
        End Function

        <HttpPost()>
        Function CacheBusinessIndustry() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard

            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.CacheBusinessIndustry())

            Return strReturn

        End Function

        Function GetCountryCode() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard

            clsCRMDashboard.Search = Common.WebRequest.Data("keywords")
            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.GetCountryCode())

            Return strReturn
        End Function

        Function SaveActivityQuota() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard

            clsCRMDashboard.TeamID = Common.WebRequest.Data("Team")
            clsCRMDashboard.DatePeriod = Common.WebRequest.AllData("Date")
            clsCRMDashboard.Calls = Common.WebRequest.Data("Call")
            clsCRMDashboard.Notes = Common.WebRequest.Data("Note")
            clsCRMDashboard.PhysicalAttended = Common.WebRequest.Data("PhysicalAttended")
            clsCRMDashboard.VirtualAttended = Common.WebRequest.Data("VirtualAttended")
            clsCRMDashboard.Email = Common.WebRequest.Data("Email")
            clsCRMDashboard.Sms = Common.WebRequest.Data("Sms")
            clsCRMDashboard.ScheduleCall = Common.WebRequest.Data("ScheduleCall")
            clsCRMDashboard.SchedulePhysical = Common.WebRequest.Data("SchedulePhysical")
            clsCRMDashboard.ScheduleVirtual = Common.WebRequest.Data("ScheduleVirtual")
            clsCRMDashboard.ScheduleEmail = Common.WebRequest.Data("ScheduleEmail")
            clsCRMDashboard.ScheduleSms = Common.WebRequest.Data("ScheduleSms")
            clsCRMDashboard.TodayDate = Common.WebRequest.Data("Today")

            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.SaveActivityQuota())

            Return strReturn
        End Function

        Function SaveTicketNotes() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard

            clsCRMDashboard.Notes = Common.WebRequest.Data("Note")
            clsCRMDashboard.TodayDate = Common.WebRequest.Data("Today")

            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.SaveTicketNotes())

            Return strReturn
        End Function

        Function CheckActivityOverlapDate() As Object
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As Object
            Dim clsCRMDashboard As New CRMDashboard

            clsCRMDashboard.DatePeriod = Common.WebRequest.AllData("Date")

            strReturn = clsCRMDashboard.CheckActivityOverlapDate()

            Return strReturn
        End Function

        Function GetActivityQuota() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As Object
            Dim clsCRMDashboard As New CRMDashboard

            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.GetActivityQuota())

            Return strReturn
        End Function

        Function GetActivityQuotaData() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As Object
            Dim clsCRMDashboard As New CRMDashboard

            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.GetActivityQuotaData())

            Return strReturn
        End Function

        Function GetTicketNoteData() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As Object
            Dim clsCRMDashboard As New CRMDashboard

            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.GetTicketNoteData())

            Return strReturn
        End Function

        Function GetTicketNotes() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As Object
            Dim clsCRMDashboard As New CRMDashboard

            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.GetTicketNotes())

            Return strReturn
        End Function

        Function GetAvailableTeamDatePeriod() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As Object
            Dim clsCRMDashboard As New CRMDashboard

            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.GetAvailableTeamDatePeriod())

            Return strReturn
        End Function

        Function GetActivityReport() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As Object
            Dim clsCRMDashboard As New CRMDashboard

            clsCRMDashboard.TeamID = Common.WebRequest.Data("Team")
            clsCRMDashboard.Territory = Common.WebRequest.AllData("Territory")
            clsCRMDashboard.User = Common.WebRequest.Data("User")
            clsCRMDashboard.DateRange = Common.WebRequest.Data("DateRange")

            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.GetActivityReport())

            Return strReturn
        End Function

        Function SaveStage() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As String
            Dim clsCRMDashboard As New CRMDashboard

            clsCRMDashboard.TodayDate = Common.WebRequest.Data("Date")
            clsCRMDashboard.Stage = Common.WebRequest.AllData("Stage")
            clsCRMDashboard.GroupStageName = Common.WebRequest.Data("GroupName")
            clsCRMDashboard.GroupStageID = Common.WebRequest.Data("ID")

            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.SaveStage())

            Return strReturn
        End Function

        Function GetGroupStage() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As Object
            Dim clsCRMDashboard As New CRMDashboard

            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.GetGroupStage())

            Return strReturn
        End Function

        Function GetStage() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As Object
            Dim clsCRMDashboard As New CRMDashboard

            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.GetStage())

            Return strReturn
        End Function

        Function RemoveGroupStage() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim strReturn As Object
            Dim clsCRMDashboard As New CRMDashboard

            strReturn = JsonDictionary.Serialize("Results", clsCRMDashboard.RemoveGroupStage())

            Return strReturn
        End Function
    End Class
End Namespace