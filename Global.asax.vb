' notes for this project:
' Conditional compilation constants:
' _MYTYPE ="Web": ProjName="WebErp"
Imports System.Threading.Tasks
Imports WebProj.RepeatingTasks

Public Class MvcApplication1
    Inherits ProWeb.MvcApplication

    'Shared mRunner As New Runner

    Shared Sub RegisterRoutes(ByVal routes As RouteCollection)
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}")

        ' MapRoute takes the following parameters, in order:
        ' (1) Route name
        ' (2) URL with parameters
        ' (3) Parameter defaults

        routes.MapRoute( _
            "Captcha", _
            "Captcha/{vKey}", _
            New With {.controller = "Captcha", .action = "Index"}, _
            New With {.vKey = New ListConstraint(eListConstraintType.Exclude, "Create")}
        )

        routes.MapRoute( _
            "Download", _
            "Download/{vFilename}", _
            New With {.controller = "Download", .action = "Index"} _
        )

        routes.MapRoute( _
            "ControllerDownload", _
            "{controller}/Download/{vFilename}", _
            New With {.action = "Download"} _
        )

        routes.MapRoute( _
            "Default", _
            "{controller}/{action}/{*id}", _
            New With {.controller = "Home", .action = "Index"}
        )

        routes.MapRoute( _
            "Error", _
            "{*url}", _
            New With {.controller = "Error", .action = "Http404"} _
        )

    End Sub

    ' Sub Application_Error() now resides in Global2.asax.vb

    Private Sub MvcApplication_ApplicationStart() Handles Me.ApplicationStart

        ' Throw New NotImplementedException
        DebugLogEnabled = True
        AreaRegistration.RegisterAllAreas()
        RegisterRoutes(RouteTable.Routes)
        ControllerBuilder.Current.SetControllerFactory(New ProControllerFactory())
#If DEBUG Then
        ProReport.DebugXmlFilename = Common.ServerPath & "temp\Stimulsoft XML Data.xml"
#End If

        Common.SetWebSession =
            Function() As WebSession
                Return New WebSession()
            End Function
        Common.SetWebRequest =
            Function() As WebRequest
                Return New WebRequest
            End Function
        UtilityRedundant.AppName = "Mobiz"
        UtilityRedundant.AppPath = Common.ServerPath
        ProPack.Client.ProgName = "Mobiz"
        DeployController.ZipPassword = "Mobiz_geh09op017agygs0m1ln"
        ProWeb.MvcApplication.ErrorMessage = "Oops, we’re experiencing some technical issues.  Please come back again later."
        AppendToLog("ApplicationStart vbW3WDHZ3SDW!", , "Global_Start")

        ' this will include subfolders
        With CrcAppenderController.Paths
            .Add("\Views\*.vbhtml")
            .Add("\Scripts3\*.js")
        End With

        If Debugger.IsAttached Then
            ' this will never be executed. debugger will be attached after MvcApplication_ApplicationStart finishes
            AppendToLog("ApplicationStart vb99SAPHTDQX! Debugger.IsAttached", , "Global_Start")
        End If
        GetServerIdentity()
        '        If DontRunStartups() = False Then
        '            Call New DbSchemaCoder().StartAsync()
        '            'QuartzScheduler.Init() ' \WebErp\vb\classes\QuartzScheduler.vb

        '            TimedThread(
        '                Sub()
        '                    RunStartups()
        '#If Debug = False Then
        '        'RunNikeSeaReport() ' commented on 2022_04_11__09_39_38_5520
        '#End If
        '                End Sub)
        '        End If

    End Sub

    Private Function DontRunStartups() As Boolean

        Dim pFilename = Common.ServerPath & "settings\DontRunStartups.txt"
        Dim pText = CStr2(FileText(pFilename))
        Dim pOut = CBool2(pText)
        FileText(pFilename) = pOut
        AppendToLog("vbP28YTZLKZ6! DontRunStartups = " & pOut, , "Global_Start")

        Return pOut

    End Function

    Private Sub GetServerIdentity()

        Dim pFilename = Common.ServerPath & "settings\ServerIdentity.txt"
        If IO.File.Exists(pFilename) Then
            modClient.ServerIdentity = FileText(pFilename).Trim()
        Else
            FileText(pFilename) = ""
        End If

    End Sub
    Private Sub RunStartups()

        Dim pBusiness As Business
        Dim pProData As ProData

        Try
            pProData = New ProData()
            pProData.FailedConnectRetryDuration = 60 * 1000
            pProData.Connect(Common.ConnectionInfo)
            pBusiness = New Business(pProData)
            pBusiness.LibraryStockMeasurements.GetOrCreateUnitMeasureID("Slice") ' 2017_05_10__11_40_03_5406
            pBusiness.LibraryStockMeasurements.GetOrCreateUnitMeasureID("Piece") ' 2017_05_10__11_40_03_5406
            pBusiness.LibraryStockMeasurements.GetOrCreateUnitMeasureID("Night") ' 2017_08_17__17_13_16_9945
            pBusiness.LibraryStockMeasurements.GetOrCreateUnitMeasureID("Day") ' 2017_08_17__17_13_16_9945
            pBusiness.LibraryStockMeasurements.GetOrCreateUnitMeasureID("Week") ' 2017_08_17__17_13_16_9945
            pBusiness.LibraryStockMeasurements.GetOrCreateUnitMeasureID("Month") ' 2017_08_17__17_13_16_9945
            pBusiness.LibraryStockMeasurements.GetOrCreateUnitMeasureID("Quarter") ' 2017_08_17__17_13_16_9945
            pBusiness.LibraryStockMeasurements.GetOrCreateUnitMeasureID("Year") ' 2017_08_17__17_13_16_9945
            ErpToMobizExportCycler.Execute()

#If DEBUG Then
            Return
#End If
            Call New UpdateServerController().Execute()

            RunDeviceMonitoringCheck()
            RunAWSDataSender()

            Task.Run(
              Sub()
                  Try
                      pBusiness.TransInvoice.AutoRunCreateDevAlertsSkipNumberAll()
                  Catch ex As Exception
                      Stop2()
                      AppendToLog(ex.Message & vbCrLf & GetStackTrace(, ex), , "Global_Start")
                  End Try
                  Try
                      pBusiness.TransInventoryLedger.CacheBalanceEachFowst()
                  Catch ex As Exception
                      Stop2()
                      AppendToLog(ex.Message & vbCrLf & GetStackTrace(, ex), , "Global_Start")
                  End Try
              End Sub)
        Catch ex As Exception
            AppendToLog("vb3ZF66S2W4L! " & ex.Message & vbCrLf & GetStackTrace(, ex), , "Global_Start")
        End Try

    End Sub

    Private Sub RunNikeSeaReport()
        'Dim pBusiness As Business
        Dim pProData As ProData

        Dim dtSubscriptionDatabase As DataTable
        ' Dim pTriggers As New Triggers(Common.WebRequest.ProData) ' ?!!
        Dim pTriggers As Triggers

        Try
            pProData = New ProData()
            pProData.FailedConnectRetryDuration = 60 * 1000
            pProData.Connect(Common.ConnectionInfo)
            pTriggers = New Triggers(pProData)

            Dim pSql As String
            pSql = "select * from Librarysubscription"

            dtSubscriptionDatabase = pProData.Execute(pSql)

            Task.Run(
              Sub()
                  Try
                      For i As Integer = 0 To dtSubscriptionDatabase.Rows.Count - 1

                          If (CStr(dtSubscriptionDatabase.Rows(i).Item("Database")).Contains("Nike")) Then

                              pTriggers.ConnectToDatabase(CStr(dtSubscriptionDatabase.Rows(i).Item("Database")))

                              '        mBusiness.ProData.Execute("USE " & EscapeSqlName(CStr(dtSubscriptionDatabase.Rows(i).Item("Database"))))

                              pTriggers.NikeSEAReport(dtSubscriptionDatabase.Rows(i).Item("Database"))
                              pTriggers.NikeSEAReportVersion2(CStr2(dtSubscriptionDatabase.Rows(i).Item("OrganizationID")), dtSubscriptionDatabase.Rows(i).Item("Database"))
                              pTriggers.ShopperTrakReport(dtSubscriptionDatabase.Rows(i).Item("Database"))
                              Exit For
                              'ElseIf (CStr(dtSubscriptionDatabase.Rows(i).Item("Database")).Contains("MobizAficionadoTesting")) Then
                              '    Try
                              '        mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
                              '        mBusiness.ProData.Execute("USE " & EscapeSqlName(CStr(dtSubscriptionDatabase.Rows(i).Item("Database"))))
                              '        Common.WebRequest.ProData.Execute("USE " & EscapeSqlName(CStr(dtSubscriptionDatabase.Rows(i).Item("Database"))))
                              '        SendDailyAlerts(CStr2(dtSubscriptionDatabase.Rows(i).Item("OrganizationID")), dtSubscriptionDatabase.Rows(i).Item("Database"))
                              '    Catch ex As Exception
                              '        AppendToLog(ex.Message)
                              '    End Try
                          ElseIf (CStr(dtSubscriptionDatabase.Rows(i).Item("Database")).Contains("ToraSports")) Then

                              pTriggers.OnitsukaReport(CStr(dtSubscriptionDatabase.Rows(i).Item("Database")))
                          End If
                      Next
                  Catch ex As Exception
                      Stop2()
                      AppendToLog("vbRTCKKXHJ5N! " & ex.Message & vbCrLf & GetStackTrace(, ex), , "Global_Start")
                  End Try
              End Sub)
        Catch ex As Exception
            AppendToLog("vb922V6S7RMF! " & ex.Message & vbCrLf & GetStackTrace(, ex), , "Global_Start")
        End Try

    End Sub

    Public Shared Sub RunAWSDataSender()
        Dim pTriggers As Triggers
        Dim pProData As ProData

        Try
            pProData = New ProData()
            pProData.FailedConnectRetryDuration = 60 * 1000
            pProData.Connect(Common.ConnectionInfo)

            pTriggers = New Triggers(pProData)

            Task.Run(
                Sub()
                    Try
#If DEBUG Then
                        AppendToLog("Connecting to MobizABC Database")
                        pTriggers.ConnectToDatabase("MobizABC")
#Else
                        AppendToLog("RunAWSDataSender function Connecting to MobizCinco_Corporation Database")
                        pTriggers.ConnectToDatabase("MobizCinco_Corporation")
#End If

                        pTriggers.RunAWSDataSender()
                    Catch ex As Exception
                        AppendToLog(ex.Message)
                    End Try
                End Sub)



        Catch ex As Exception

        End Try

    End Sub

    Public Shared Sub RunDeviceMonitoringCheck()
        Dim pTriggers As Triggers
        Dim pProData As ProData

        Try
            pProData = New ProData()
            pProData.FailedConnectRetryDuration = 60 * 1000
            pProData.Connect(Common.ConnectionInfo)

            pTriggers = New Triggers(pProData)

            Task.Run(
                Sub()
                    Try
#If DEBUG Then
                        AppendToLog("Connecting to MobizABC Database")
                        pTriggers.ConnectToDatabase("MobizABC")
                        pTriggers.RunDeviceMonitoringCheck("MobizABC")
#Else
                        AppendToLog("RunDeviceMonitoringCheck function Connecting to MobizCinco_Corporation Database")
                        pTriggers.ConnectToDatabase("MobizCinco_Corporation")
                        pTriggers.RunDeviceMonitoringCheck("MobizCinco_Corporation")
#End If

                    Catch ex As Exception
                        AppendToLog(ex.Message)
                    End Try
                End Sub)



        Catch ex As Exception

        End Try

    End Sub

    Private Sub MvcApplication_SessionEnd() Handles Me.SessionEnd

    End Sub

    Private Sub MvcApplication_SessionStart() Handles Me.SessionStart

        DeleteSessionCache()

    End Sub

    Private Sub DeleteSessionCache()

        Dim pFolder As String

        Try
            pFolder = Common.ServerPath & "temp\sessions\" & Session.SessionID
            If IO.Directory.Exists(pFolder) Then
                IO.Directory.Delete(pFolder, True)
            End If
        Catch ex As Exception
            Stop2()
        End Try

    End Sub

    Private Sub DelayRequest()

        Dim pUrl As String  ' lowercase

        pUrl = Request.FilePath.Trim.ToLower
        If pUrl.Contains("content/url to simulate slow connection/") Then
            Threading.Thread.Sleep(5000)
        End If

    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        'MyBase.Application_BeginRequest(sender, e)

        If HttpContext.Current.Request.HttpMethod = "OPTIONS" Then
            HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache")
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*")
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST")
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "*")
            HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000")
            HttpContext.Current.Response.End()
            Exit Sub
        End If
        
        ' ask 1st if header already exist.
        If Now.Date = #6/7/2017# Then
            AppendToLog("Application_BeginRequest. ref: vbGP565AJ95D!", , "Application_BeginRequest vbGP565AJ95D")
        End If
        Try
            If HttpRuntime.UsingIntegratedPipeline _
                AndAlso HttpContext.Current.Response.Headers.Get("Access-Control-Allow-Origin") Is Nothing Then ' "This operation requires IIS integrated pipeline mode."

                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*")
            End If
        Catch ex As Exception
            ' "This operation requires IIS integrated pipeline mode."
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*")
        End Try
        HttpContext.Current.Response.AppendHeader("X-Frame-Options", "SAMEORIGIN") ' https://www.owasp.org/index.php/Clickjacking_Defense_Cheat_Sheet

        '/* Fix for the Flash Player Cookie bug in Non-IE browsers.
        ' * Since Flash Player always sends the IE cookies even in FireFox
        ' * we have to bypass the cookies by sending the values as part of the POST or GET
        ' * and overwrite the cookies with the passed in values.
        ' * 
        ' * The theory is that at this point (BeginRequest) the cookies have not been read by
        ' * the Session and Authentication logic and if we update the cookies here we'll get our
        ' * Session and Authentication restored correctly
        ' */

        Try
            Dim session_cookie_name As String = "ASP.NET_SessionId"

            If Common.WebRequest.Data(session_cookie_name) IsNot Nothing Then
                UpdateCookie(session_cookie_name, Common.WebRequest.Data(session_cookie_name))
                'Exit Sub
            ElseIf Not HttpContext.Current.Request(session_cookie_name) Is Nothing Then
                UpdateCookie(session_cookie_name, HttpContext.Current.Request(session_cookie_name))
                'Exit Sub
            ElseIf Not Common.WebRequest.QueryString(session_cookie_name) Is Nothing Then
                UpdateCookie(session_cookie_name, Common.WebRequest.QueryString(session_cookie_name))
                'Exit Sub
            End If
        Catch ex As Exception
            'Stop2() ' just ignore any error
            Response.StatusCode = 500
            Response.Write("Error Initializing Session")
        End Try

        Try
            Dim auth_param_name As String = ".ASPXAUTH" '"AUTHID"
            Dim auth_cookie_name As String = FormsAuthentication.FormsCookieName

            If Not HttpContext.Current.Request.Form(auth_param_name) Is Nothing Then
                UpdateCookie(auth_cookie_name, HttpContext.Current.Request.Form(auth_param_name))
                'Exit Sub
            ElseIf Not Common.WebRequest.QueryString(auth_param_name) Is Nothing Then
                UpdateCookie(auth_cookie_name, Common.WebRequest.QueryString(auth_param_name))
                'Exit Sub
            End If
        Catch ex As Exception
            'Stop2() ' just ignore any error
            Response.StatusCode = 500
            Response.Write("Error Initializing Forms Authentication")
        End Try

        'mRunner.Start()

    End Sub

    Private Sub Application_EndRequest()

    End Sub

    Sub UpdateCookie(ByVal cookie_name As String, ByVal cookie_value As String)
        Dim cookie As System.Web.HttpCookie = HttpContext.Current.Request.Cookies.Get(cookie_name)
        If cookie Is Nothing Then
            cookie = New HttpCookie(cookie_name)
            HttpContext.Current.Request.Cookies.Add(cookie)
        End If
        cookie.Value = cookie_value
        HttpContext.Current.Request.Cookies.Set(cookie)
    End Sub

    Private Sub MvcApplication_EndRequest(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.EndRequest

        Common.DisposeWebRequest() ' this is important to rollback transactions (if any)

    End Sub

    Private Sub MvcApplication_BeginRequest(sender As Object, e As System.EventArgs) Handles Me.BeginRequest

    End Sub

#If DEBUG Then
    Public Overrides Sub Init()
        ' http://stackoverflow.com/a/25466524
        MyBase.Init()
        'Me.AddOnAcquireRequestStateAsync(  
    End Sub
#End If

    Public Shared Sub SessionTest()

        For Each pItem As KeyValuePair(Of String, SessionState.HttpSessionState) In Sessions
            Debug.Print("vbMCHWECE8PM! " & pItem.Value("WebSession"))
        Next pItem

    End Sub

    Public Class ProControllerFactory
        Inherits DefaultControllerFactory
        ' http://weblogs.asp.net/nmarun/archive/2009/10/14/custom-404-when-no-route-matches.aspx

        Public Overrides Function CreateController(requestContext As RequestContext, controllerName As String) As IController

            Dim pOut As IController
            Dim pControllerType As Type
            Dim pRelativePath As String

            Try
                If GetControllerType(requestContext, controllerName) IsNot Nothing Then
                    pOut = MyBase.CreateController(requestContext, controllerName)
                    If pOut IsNot Nothing Then
                        Return pOut
                    End If
                End If
            Catch ex As Exception

            End Try

            pRelativePath = requestContext.HttpContext.Request.Path
            If DownloadController.IsCompiledExist(pRelativePath) Then
                pControllerType = GetControllerType(requestContext, "Download")
                requestContext.RouteData.Values("Controller") = "Download"
                requestContext.RouteData.Values("action") = "Compiled"
                'requestContext.RouteData.Values("url") = pRelativePath
            Else
                pControllerType = GetControllerType(requestContext, "Error")
                requestContext.RouteData.Values("Controller") = "Error"
                requestContext.RouteData.Values("action") = "Index"
                requestContext.RouteData.Values("url") = pRelativePath
            End If

            pOut = GetControllerInstance(requestContext, pControllerType)

            Return pOut

        End Function

    End Class ' ProControllerFactory
End Class
