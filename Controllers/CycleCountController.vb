Namespace WebProj
    Public Class CycleCountController
        Inherits System.Web.Mvc.Controller
        Dim sDT As New Dictionary(Of String, Object)
        Dim JSSerializer As New JavaScriptSerializer
        '
        ' GET: /CycleCount

        Function Index() As ActionResult
            'If WebSession.Instance.UserAccess Is Nothing _
            '    OrElse WebSession.Instance.UserAccess.InventoryCycleCount_UseXDA.IsEnable = False Then

            '    Transfer("/login")
            'Else
            Transfer("/CycleCount/Branches")
            'End If
        End Function

        Function SKU() As ActionResult
            'If WebSession.Instance.UserAccess Is Nothing _
            '    OrElse WebSession.Instance.UserAccess.InventoryCycleCount_UseXDA.IsEnable = False Then

            '    Transfer("/login")
            'Else
            Return View()
            'End If
        End Function

        Function Branches() As ActionResult
            'If WebSession.Instance.UserAccess Is Nothing _
            '    OrElse WebSession.Instance.UserAccess.InventoryCycleCount_UseXDA.IsEnable = False Then

            '    Transfer("/login")
            'Else
            Return View()
            'End If
        End Function

        Function getAllBranches() As String
            Dim cyclecount As New CycleCount
            Dim str As String

            str = cyclecount.getAllBranches()
            Return str
        End Function

        Function Warehouse() As ActionResult
            'If WebSession.Instance.UserAccess Is Nothing _
            '    OrElse WebSession.Instance.UserAccess.InventoryCycleCount_UseXDA.IsEnable = False Then

            '    Transfer("/login")
            'Else
            Return View()
            'End If
        End Function

        Function CountInfo() As ActionResult
            'If WebSession.Instance.UserAccess Is Nothing _
            '    OrElse WebSession.Instance.UserAccess.InventoryCycleCount_UseXDA.IsEnable = False Then

            '    Transfer("/login")
            'Else
            Return View()
            'End If
        End Function

        Function getAllWarehouses() As String
            Dim cyclecount As New CycleCount
            Dim str As String

            str = cyclecount.getAllWarehouses()
            Return str
        End Function

        Function startCount() As ActionResult
            Return View()
        End Function

        Function SKUInfo() As ActionResult
            'If WebSession.Instance.UserAccess Is Nothing _
            '    OrElse WebSession.Instance.UserAccess.InventoryCycleCount_UseXDA.IsEnable = False Then

            '    Transfer("/login")
            'Else
            Return View()
            'End If
        End Function

        Function UnitMeasures() As ActionResult
            'If WebSession.Instance.UserAccess Is Nothing _
            '    OrElse WebSession.Instance.UserAccess.InventoryCycleCount_UseXDA.IsEnable = False Then

            '    Transfer("/login")
            'Else
            Return View()
            'End If
        End Function

        Function searchItem() As String
            Dim cyclecount As New CycleCount
            Dim str As String

            sDT = JSSerializer.DeserializeObject(Request.Form(0))

            str = cyclecount.searchStock(sDT)
            Return str
        End Function

        Function setBranchID() As String

            sDT = JSSerializer.DeserializeObject(Request.Form(0))
            WebSession.Instance.branchID = CInt(sDT("ID"))

            Return "True"

        End Function

        Function setWarehouseID() As String

            sDT = JSSerializer.DeserializeObject(Request.Form(0))
            WebSession.Instance.WarehouseID = sDT("ID")

            Return "True"

        End Function

        Function setItemID() As String

            sDT = JSSerializer.DeserializeObject(Request.Form(0))
            WebSession.Instance.skuID = sDT("ID")

            Return "True"

        End Function

        Function setCountType() As String

            sDT = JSSerializer.DeserializeObject(Request.Form(0))
            WebSession.Instance.countType = CInt(sDT("Type"))

            Return "True"

        End Function

        Function getUnitMeasure() As String
            Dim cyclecount As New CycleCount
            Dim str As String

            str = cyclecount.searchUnitMeasure()
            Return str
        End Function

        'Function LogIn() As JsonResult
        '    Dim pUsername As String
        '    Dim pPassword As String
        '    Dim pBankUser As New Dictionary(Of String, String)

        '    Try
        '        pUsername = Common.WebRequest.Data("txtUsername")
        '        pPassword = Common.WebRequest.Data("txtPassword")
        '        If WebRequest.Instance.Business.MainSecurity.SetAccessByUsernameAndPassword(pUsername, pPassword) Then
        '            WebSession.Instance.UserID = WebRequest.Instance.Business.MainSecurity.UserAccess.EmployeeID
        '            Return Json("[{""Result"":""Success""}]", JsonRequestBehavior.AllowGet)
        '        Else
        '            Return Json("[{""Result"":""Error""}]", JsonRequestBehavior.AllowGet)
        '        End If
        '    Catch ex As Exception
        '        Return Json("[{""Result"":""Error""}]", JsonRequestBehavior.AllowGet)
        '    End Try


        'End Function

        'Public Function getTime() As JsonResult
        '    HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*")
        '    'Return Json("[{""Result"":""4/18/2012 11:33:33 AM""}]", JsonRequestBehavior.AllowGet)
        '    Return Json("{""Result"":""asd""}")
        '    'Return Json("{""menu"": { " &
        '    '        """header"": ""SVG Viewer"", " &
        '    '        """items"": [" &
        '    '            "{""id"": ""Open""}," &
        '    '            "{""id"": ""OpenNew"", ""label"": ""Open New""}" &
        '    '        "]" &
        '    '    "}}", JsonRequestBehavior.AllowGet)
        'End Function
    End Class
End Namespace