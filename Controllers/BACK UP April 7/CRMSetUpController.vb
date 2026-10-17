Namespace WebProj
    Public Class CRMSetUpController
        Inherits System.Web.Mvc.Controller
        Dim sDT As New Dictionary(Of String, Object)
        Dim JSSerializer As New JavaScriptSerializer

        '
        ' GET: /CRMSetUp

#Region "ActionResult"
        Function Index() As ActionResult
            'If WebSession.Instance.UserAccess Is Nothing _
            '               OrElse WebSession.Instance.UserAccess.Sales_Sales.IsEnable = False Then

            '    Transfer("/login")
            'Else
            '    If WebSession.Instance.UserAccess.SalesOrder_ApproveOverCreditLimit.IsEnable Then
            Return View()
            '    Else
            '        Transfer("/AccessError")
            '    End If
            'End If
        End Function

        Function Menu() As ActionResult
            Return View()
        End Function

        Function Teams() As ActionResult
            Return View()
        End Function

        Function ListOfBranches() As ActionResult
            WebSession.Instance.BranchName = Common.WebRequest.QueryString("BranchName")

            Return View()
        End Function

        Function ListOfTeams() As ActionResult
            WebSession.Instance.TeamName = Common.WebRequest.QueryString("TeamName")
            Return View()
        End Function

        Function TeamMembers() As ActionResult
            Return View()
        End Function

        Function ListOfEmployees() As ActionResult
            WebSession.Instance.FullName = Common.WebRequest.QueryString("EmployeeName")
            Return View()
        End Function

        Function EditTeamMembers() As ActionResult
            WebSession.Instance.EmployeeID = Common.WebRequest.QueryString("EmployeeID")
            Return View()
        End Function

        Function RemoveEmployee() As ActionResult
            WebSession.Instance.EmployeeID = Common.WebRequest.QueryString("EmployeeID")
            Return View()
        End Function

        Function SalesQuotaPerMember() As ActionResult
            Return View()
        End Function

        Function Quota() As ActionResult
            Return View()
        End Function

        Function QuotaSetUp() As ActionResult
            WebSession.Instance.Month = Common.WebRequest.QueryString("Month")
            Return View()
        End Function

        Function ItemIncentive() As ActionResult
            Return View()
        End Function

        Function ListOfItems() As ActionResult
            WebSession.Instance.IncentiveItemName = Common.WebRequest.QueryString("ItemName")
            Return View()
        End Function

        Function EditItem() As ActionResult
            WebSession.Instance.IncentiveItemID = Common.WebRequest.QueryString("Item")
            Return View()
        End Function

        Function CollectionSetUp() As ActionResult
            Return View()
        End Function

        Function Customers() As ActionResult
            Return View()
        End Function

        Function AddCustomers() As String
            Dim crm As New CRMSetUp

            Return crm.AddCustomers()
        End Function

        Function RemoveCustomer() As ActionResult
            WebSession.Instance.CustomerID = Common.WebRequest.QueryString("CustomerID")
            Return View()
        End Function

        Function ListOfCustomers() As ActionResult
            WebSession.Instance.CustomerName = Common.WebRequest.QueryString("CustomerName")
            Return View()
        End Function

        Function CustomerAlreadyExist() As ActionResult
            Return View()
        End Function

        Function TeamLeadersCut() As ActionResult
            Return View()
        End Function

        Function Done() As ActionResult
            Return View()
        End Function
#End Region

#Region "Get"
        Function getTeamName() As String
            Return WebSession.Instance.TeamName
        End Function

        Function getBranches() As String
            Dim crm As New CRMSetUp
            Dim pKeywords As String

            pKeywords = WebSession.Instance.BranchName

            Return JsonDictionary.Serialize("data", crm.getBranches(pKeywords))
        End Function

        Function getItems() As String
            Dim crm As New CRMSetUp

            Dim vDt As New List(Of Dictionary(Of String, String))

            vDt = JsonDictionary.TableToDictionary(crm.getItems())

            Return JsonDictionary.Serialize(vDt)
        End Function

        Function getItemIncentiveDetail() As String
            Dim crm As New CRMSetUp

            Dim vDt As New List(Of Dictionary(Of String, String))

            vDt = JsonDictionary.TableToDictionary(crm.getItemIncentiveDetail())

            Return JsonDictionary.Serialize(vDt)
        End Function

        Function getItemIncentive() As String
            Dim crm As New CRMSetUp

            Dim vDt As New List(Of Dictionary(Of String, String))

            vDt = JsonDictionary.TableToDictionary(crm.getItemIncentive())
            Return JsonDictionary.Serialize(vDt)
        End Function

        Function GetCustomerInfo() As String
            Dim crm As New CRMSetUp

            Dim vDt As New List(Of Dictionary(Of String, String))

            vDt = JsonDictionary.TableToDictionary(crm.getCustomerInfo())
            Return JsonDictionary.Serialize(vDt)
        End Function

        Function getTeamMembers() As String
            Dim crm As New CRMSetUp
            Dim pKeywords As String

            pKeywords = WebSession.Instance.BranchName

            Return JsonDictionary.Serialize("data", crm.GetTeamMembers(pKeywords))
        End Function

        Function getEmployees() As String
            Dim crm As New CRMSetUp

            sDT = JSSerializer.DeserializeObject(Request.Form(0))

            Dim vDt As New List(Of Dictionary(Of String, String))

            vDt = JsonDictionary.TableToDictionary(crm.getEmployees(sDT))
            Return JsonDictionary.Serialize(vDt)
        End Function

        Function getTeamLeadersCut() As String
            Dim crm As New CRMSetUp

            Dim vDt As New List(Of Dictionary(Of String, String))

            vDt = JsonDictionary.TableToDictionary(crm.getTeamLeadersCut())
            Return JsonDictionary.Serialize(vDt)
        End Function

        Function GetQuotaInfo() As String
            Dim crm As New CRMSetUp

            Return JsonDictionary.Serialize("data", crm.GetQuotaInfo())

            'Dim crm As New CRMSetUp

            'Dim vDt As New List(Of Dictionary(Of String, String))

            'vDt = JsonDictionary.TableToDictionary(crm.GetQuotaInfo())
            'Return JsonDictionary.Serialize(vDt)

        End Function

        Function GetCollectionInfo() As String
            Dim crm As New CRMSetUp

            Dim vDt As New List(Of Dictionary(Of String, String))

            vDt = JsonDictionary.TableToDictionary(crm.GetCollectionInfo())
            Return JsonDictionary.Serialize(vDt)
        End Function

        Function getSearchKeyName()
            Return WebSession.Instance.FullName
        End Function

        Function getCustomers() As String
            Dim crm As New CRMSetUp

            sDT = JSSerializer.DeserializeObject(Request.Form(0))

            Dim vDt As New List(Of Dictionary(Of String, String))

            vDt = JsonDictionary.TableToDictionary(crm.getCustomers(sDT))
            Return JsonDictionary.Serialize(vDt)
        End Function

        Function getTeamCustomers() As String
            Dim crm As New CRMSetUp

            Dim vDt As New List(Of Dictionary(Of String, String))

            vDt = JsonDictionary.TableToDictionary(crm.getTeamCustomers())
            Return JsonDictionary.Serialize(vDt)
        End Function

        Function getTeamQuota() As String
            Dim crm As New CRMSetUp


            Dim vDt As New List(Of Dictionary(Of String, String))

            vDt = JsonDictionary.TableToDictionary(crm.getTeamQuota())
            Return JsonDictionary.Serialize(vDt)

        End Function

        Function getTeams() As String
            Dim crm As New CRMSetUp
            Dim pKeywords As String

            pKeywords = Common.WebRequest.Data("keywords")

            Return JsonDictionary.Serialize("data", crm.getTeams(pKeywords))

        End Function

        Function getTeamInfo() As String
            Dim crm As New CRMSetUp
            Dim pKeywords As String

            Return JsonDictionary.Serialize("data", crm.getTeamInfo())

        End Function

        Function IsNew() As String
            Return WebSession.Instance.IsNew
        End Function

        Function getBranchInfo() As String
            Return WebSession.Instance.BranchName
        End Function
#End Region

#Region "Set"
        Function assignBranchID() As String
            WebSession.Instance.BranchID = Common.WebRequest.Data("m_BranchID")
            WebSession.Instance.BranchName = Common.WebRequest.Data("m_BranchName")
            Return ""
        End Function

        Function assignAsTeamLeader() As String
            Dim crm As New CRMSetUp

            'sDT = JSSerializer.DeserializeObject(Request.Form(0))

            Return crm.assignAsTeamLeader(Common.WebRequest.Data("EmpID"))
        End Function

        Function AssignIsNew() As String

            sDT = JSSerializer.DeserializeObject(Request.Form(0))

            WebSession.Instance.IsNew = sDT("IsNew")

            Return "true"
        End Function

        Function assignSKUID() As String

            sDT = JSSerializer.DeserializeObject(Request.Form(0))

            WebSession.Instance.SkuID = sDT("skuID")

            Return ""

        End Function

        Function assignTeamID() As String

            sDT = JSSerializer.DeserializeObject(Request.Form(0))

            WebSession.Instance.TeamID = sDT("m_TeamID")
            WebSession.Instance.TeamName = sDT("m_TeamName")

            Return ""

        End Function
#End Region

#Region "Functions"
        Function CheckIfCustomerExist() As String
            Dim crm As New CRMSetUp

            sDT = JSSerializer.DeserializeObject(Request.Form(0))

            WebSession.Instance.CustomerID = sDT("CustomerID")

            Return crm.CheckIfCustomerExist()
        End Function

        Function ReCalculateBaseQuota() As String
            Dim crm As New CRMSetUp

            Return crm.ReCalculateBaseQuota()
        End Function

        Function RemoveSelectedEmployee() As String
            Dim crm As New CRMSetUp

            sDT = JSSerializer.DeserializeObject(Request.Form(0))

            Return crm.RemoveSelectedEmployee(sDT)
        End Function

        Function RemoveSelectedCustomer() As String
            Dim crm As New CRMSetUp

            Return crm.RemoveSelectedCustomer()
        End Function

        Function saveItemIncentive() As String
            Dim crm As New CRMSetUp

            sDT = JSSerializer.DeserializeObject(Request.Form(0))

            Return crm.saveItemIncentive(sDT) 'edit this
        End Function

        Function saveSalesQuotaPerMember() As String
            Dim crm As New CRMSetUp

            'sDT = JSSerializer.DeserializeObject(Request.Form(0))

            Return crm.saveSalesQuotaPerMember()
        End Function

        Function saveTeam() As String
            Dim crm As New CRMSetUp

            'sDT = JSSerializer.DeserializeObject(Request.Form(0))

            Return CRM.saveTeam()
        End Function

        Function saveTeamLeadersCut() As String
            Dim crm As New CRMSetUp

            sDT = JSSerializer.DeserializeObject(Request.Form(0))

            Return crm.saveTeamLeadersCut(sDT)
        End Function

        Function saveQuotaInfo() As String
            Dim crm As New CRMSetUp

            sDT = JSSerializer.DeserializeObject(Request.Form(0))

            Return crm.saveQuotaInfo(sDT)
        End Function

        Function saveCollectionInfo() As String
            Dim crm As New CRMSetUp

            sDT = JSSerializer.DeserializeObject(Request.Form(0))

            Return crm.saveCollectionInfo(sDT)
        End Function

        Function IncentiveCollectionPeriod() As String
            Dim crm As New CRMSetUp

            sDT = JSSerializer.DeserializeObject(Request.Form(0))

            Return crm.IncentiveCollectionPeriod(sDT)
        End Function

        Function AddEmployee() As String
            Dim crm As New CRMSetUp

            sDT = JSSerializer.DeserializeObject(Request.Form(0))

            Return crm.AddEmployee(sDT)
        End Function

        Function UpdateEmployee() As String
            Dim crm As New CRMSetUp

            sDT = JSSerializer.DeserializeObject(Request.Form(0))

            Return crm.UpdateEmployee(sDT)
        End Function

        
#End Region
    End Class
End Namespace