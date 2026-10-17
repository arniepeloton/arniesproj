Namespace WebProj
    Public Class CrmController
        Inherits System.Web.Mvc.Controller
        'Dim sDT As New Dictionary(Of String, Object)
        'Dim JSSerializer As New JavaScriptSerializer

        '
        ' GET: /Crm

        Function Index() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess() = False Then Exit Function
            End If

            Return View()

        End Function

        'FOR MOBIZ CRM Sales Team Sys Admin
        Function CRMIndex() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess() = False Then Exit Function
            End If

            Return View()

        End Function

        Function GetAllTeams() As String
            Dim strReturn As String
            Dim clsCRM As New CRM

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsCRM.GetAllTeams)

        End Function

        Function SaveCRMTeam() As String
            Dim strReturn As String
            Dim clsCRM As New CRM

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsCRM.ID = Common.WebRequest.Data("ID")

            strReturn = JsonDictionary.Serialize("Result", clsCRM.SaveTeam)

            Return strReturn

        End Function

        Function GetTeamInfo() As String
            Dim strReturn As String
            Dim clsCRM As New CRM

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsCRM.ID = Common.WebRequest.Data("ID")

            Return JsonDictionary.Serialize("Results", clsCRM.GetTeamInfo)

        End Function

        Function GetTeamMembers() As String
            Dim strReturn As String
            Dim clsCRM As New CRM

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsCRM.ID = Common.WebRequest.Data("ID")

            Return JsonDictionary.Serialize("Results", clsCRM.GetTeamMembers)
        End Function

        Function GetTeamCustomers() As String
            Dim strReturn As String
            Dim clsCRM As New CRM

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsCRM.ID = Common.WebRequest.Data("ID")

            Return JsonDictionary.Serialize("Results", clsCRM.GetTeamCustomers)
        End Function

        Function GetTeamQuota() As String
            Dim strReturn As String
            Dim clsCRM As New CRM

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsCRM.ID = Common.WebRequest.Data("ID")

            Return JsonDictionary.Serialize("Results", clsCRM.GetTeamQuota)
        End Function

        Function IsCustomerOwnedByOtherTeam() As String
            Dim pSql As String
            Dim pDT As DataTable

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "select teamname from LibraryCRMCustomers " &
                    "inner join LibraryCRMTeams on LibraryCRMTeams.ID = LibraryCRMCustomers.TeamID " &
                    "where CustomerID = " & ReadyString(Common.WebRequest.Data("CustomerID")) 

            pDT = Common.WebRequest.ProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", pDT)

        End Function

        Function GetSalesOrderData() As String
            Dim strReturn As String
            Dim clsCRM As New CRM

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsCRM.GetSalesOrderData)
        End Function

        Function GetEmployeeSalesData() As String
            Dim strReturn As String
            Dim clsCRM As New CRM

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", clsCRM.GetEmployeeSalesData)
        End Function

        'Function GetCustomers() As String
        '    Dim strReturn As String
        '    Dim crm As New CRM

        '    strReturn = JsonDictionary.Serialize("Customers", crm.GetCustomers(Common.WebRequest.Data("Type")))

        '    Return strReturn
        'End Function

        'Function GetCustomersContactDetails() As String
        '    Dim strReturn As String
        '    Dim crm As New CRM

        '    strReturn = JsonDictionary.Serialize("Customers", crm.GetCustomersContactDetails())

        '    Return strReturn
        'End Function

        'Function CompanyInformation(ByVal str As String) As ActionResult
        '    If WebSession.Instance.UserAccess Is Nothing _
        '            OrElse WebSession.Instance.UserAccess.Sales_Sales.IsEnable = False Then

        '        Transfer("/login")
        '    Else
        '        If getTeamID() Then
        '            ViewData("Title") = WebSession.Instance.CustomerName
        '            Return View()
        '        Else
        '            Transfer("/welcome")
        '        End If

        '    End If

        'End Function

        'Function CRMInformation() As ActionResult
        '    Return View()
        'End Function

        'Function UnAuthorizedAccess() As ActionResult

        '    Return View()

        'End Function

        'Function getLeads() As String
        '    Dim crm As New CRM
        '    Dim dt As New DataTable
        '    Dim vDt As New List(Of Dictionary(Of String, String))
        '    Dim str As String

        '    dt = crm.getLeads

        '    str = crm.convertToJSON(dt, "Leads")

        '    Return str

        'End Function

        'Function GetCustomerCount() As String
        '    Dim strReturn As String
        '    Dim crm As New CRM

        '    strReturn = JsonDictionary.Serialize("Customers", crm.GetCustomerCount())

        '    Return strReturn
        'End Function

        'Function getProspects() As String
        '    Dim crm As New CRM
        '    Dim dt As New DataTable
        '    Dim vDt As New List(Of Dictionary(Of String, String))
        '    Dim str As String

        '    dt = crm.getProspects

        '    str = crm.convertToJSON(dt, "Prospects")

        '    Return str

        'End Function

        'Function getClosedDeals() As String
        '    Dim crm As New CRM
        '    Dim dt As New DataTable
        '    Dim vDt As New List(Of Dictionary(Of String, String))
        '    Dim str As String

        '    dt = crm.getClosedDeals

        '    str = crm.convertToJSON(dt, "ClosedDeals")

        '    Return str

        'End Function

        ''Function saveRating() As String
        ''    Dim crm As New CRM

        ''    sDT = JSSerializer.DeserializeObject(Request.Form(0))

        ''    Dim pOut As JsonDictionary

        ''    pOut = New JsonDictionary

        ''    If crm.saveRating(sDT) Then
        ''        pOut.Add("res", True)
        ''    Else
        ''        pOut.Add("res", False)
        ''    End If

        ''    Return pOut.Serialize

        ''End Function

        'Function saveValue() As String
        '    Dim crm As New CRM
        '    Dim str As String
        '    Dim strReturn As String

        '    Dim pOut As New JsonDictionary

        '    If crm.saveValue Then
        '        pOut.Add("CustomerValue", "True")
        '    Else
        '        pOut.Add("CustomerValue", "False")
        '    End If

        '    Return pOut.Serialize
        'End Function

        ''Function saveClosedDate() As String
        ''    Dim crm As New CRM

        ''    sDT = JSSerializer.DeserializeObject(Request.Form(0))

        ''    Dim pOut As JsonDictionary

        ''    pOut = New JsonDictionary

        ''    If crm.saveClosedDate(sDT) Then
        ''        pOut.Add("res", True)
        ''    Else
        ''        pOut.Add("res", False)
        ''    End If

        ''    Return pOut.Serialize

        ''End Function

        'Function getTeamID() As Boolean
        '    Dim crm As New CRM

        '    Return crm.getTeamID
        'End Function

        'Function getCustomerInformation() As String
        '    Dim crm As New CRM
        '    Dim str As String

        '    str = crm.getCustomerInformation()
        '    Return str

        'End Function

        'Function getCustomerNotes() As String
        '    Dim crm As New CRM
        '    Dim str As String

        '    str = crm.getCustomerNotes()

        '    Return str

        'End Function

        ''Function assignCustomerID(ByVal ID As String)
        ''    Dim crm As New CRM

        ''    sDT = JSSerializer.DeserializeObject(Request.Form(0))

        ''    WebSession.Instance.CustomerName = crm.getCustomerName(sDT("ID"))
        ''    WebSession.Instance.CustomerID = sDT("ID")

        ''End Function

        ''Function getTODO() As String
        ''    Dim crm As New CRM
        ''    Dim dt As New DataTable
        ''    Dim str As String

        ''    sDT = JSSerializer.DeserializeObject(Request.Form(0))

        ''    dt = crm.getTODO(sDT("m_Option"))

        ''    str = crm.convertTodoTableToJSON(dt)

        ''    Return str
        ''End Function

        'Function getDashBoardData() As String
        '    Dim crm As New CRM
        '    Dim str As String

        '    str = crm.getDashBoardData()

        '    Return str
        'End Function

        ''Function getSalesByQuarter() As String
        ''    Dim crm As New CRM
        ''    Dim str As String

        ''    sDT = JSSerializer.DeserializeObject(Request.Form(0))

        ''    str = crm.getSalesByQuarter(CInt(sDT("Value")))

        ''    Return str
        ''End Function

        '<HttpPost()>
        'Function getSalesByMonthly() As String
        '    Dim crm As New CRM
        '    Dim str As String

        '    'sDT = JSSerializer.DeserializeObject(Request.Form(0))

        '    str = crm.getSalesByMonthly(Common.WebRequest.Data("Team"), Common.WebRequest.Data("Month"), Common.WebRequest.Data("Year"))

        '    Return str
        'End Function

        ''Function getCollection() As String
        ''    Dim crm As New CRM
        ''    Dim str As String

        ''    sDT = JSSerializer.DeserializeObject(Request.Form(0))

        ''    str = crm.getCollection(sDT("Team"), sDT("Month"), sDT("Year"))

        ''    Return str
        ''End Function

        'Function getTeams() As String
        '    Dim crm As New CRM
        '    Dim str As String

        '    str = crm.getTeams()

        '    Return str
        'End Function

        ''Function getTeamInfo() As String
        ''    Dim crm As New CRM
        ''    Dim str As String 

        ''    sDT = JSSerializer.DeserializeObject(Request.Form(0))

        ''    str = crm.getTeamInfo(sDT("ID"))

        ''    Return str
        ''End Function

        'Function IsAdmin() As String
        '    Dim crm As New CRM
        '    Dim strReturn As String

        '    strReturn = JsonDictionary.Serialize("IsAdmin", crm.isAdmin)

        '    Return strReturn
        'End Function

        'Function getWheather() As String
        '    Dim crm As New CRM
        '    Dim str As String

        '    sDT = JSSerializer.DeserializeObject(Request.Form(0))

        '    str = crm.getWheather(sDT("City"))

        '    Return str
        'End Function
    End Class
End Namespace