Public Class ErrorController
    Inherits System.Web.Mvc.Controller

    Function Index(ByVal url As String) As ActionResult

        Const pVendorPrefix = "vportal/"
        Dim pIsVendor As Boolean
        Dim pOnlineStoreName As String

        If String.IsNullOrWhiteSpace(url) Then
            Response.StatusCode = 500
            ViewData("Url") = url

            Return View()
        End If

        url = Trim2(url, "/")
        If url.StartsWith(pVendorPrefix) Then
            pOnlineStoreName = url.Substring(pVendorPrefix.Length)
            pIsVendor = True
        Else
            pOnlineStoreName = url
        End If

        pOnlineStoreName = Trim2(pOnlineStoreName, "/")
        If Regex.Replace(pOnlineStoreName, "[^A-Za-z0-9]", "") <> pOnlineStoreName Then
            Exit Function
        End If
        If ReservedNames.Contains(pOnlineStoreName.ToLower()) Then
            Return Content("ref: vbNYFX7HV8G5!")
        End If

        Dim pDR As DataRow
        If pIsVendor Then
            pDR = WebRequest.Instance.Business.LibraryOrganization.ByID("", , pOnlineStoreName)
            If pDR IsNot Nothing Then
                WebRequest.Instance.OrgID = ReadyGuid(pDR("ID"))
                Dim pUrl = "/vendorportal/signin?OrgID=" & ReadyGuid(pDR("ID"))
                Response.Redirect(pUrl)
                Exit Function
            End If
        Else
            pDR = WebRequest.Instance.Business.LibraryOrganization.ByID("", pOnlineStoreName)
            If pDR IsNot Nothing Then
                WebRequest.Instance.OrgID = ReadyGuid(pDR("ID"))
                Dim pUrl = "/ecommerce?OrgID=" & ReadyGuid(pDR("ID"))
                Response.Redirect(pUrl)
                Exit Function
            End If
        End If

        'Transfer("Ecommerce?store=" & url)
        Response.StatusCode = 500
        ViewData("Url") = url

        Return View()

    End Function

    Private Function ReservedNames() As List(Of String)

        Dim pOut As New List(Of String)

        pOut.Add("login")

        ' ReservedNames-BEGIN-TOKEN
pOut.Add("accountadjustments")
pOut.Add("accountmanagement")
pOut.Add("accountrecovery")
pOut.Add("alerts")
pOut.Add("androidjstest")
pOut.Add("assetsmanagement")
pOut.Add("billofmaterials")
pOut.Add("bomprojectexcel")
pOut.Add("browser")
pOut.Add("canvass")
pOut.Add("captcha")
pOut.Add("cashhandling")
pOut.Add("centers")
pOut.Add("checkprinter")
pOut.Add("citizenship")
pOut.Add("city")
pOut.Add("collections")
pOut.Add("compilepages")
pOut.Add("confirmemployment")
pOut.Add("contractdesigner")
pOut.Add("country")
pOut.Add("creditandcollection")
pOut.Add("crm")
pOut.Add("crmdashboard")
pOut.Add("crmsetup")
pOut.Add("customer")
pOut.Add("customersearch")
pOut.Add("cyclecount")
pOut.Add("dashboard")
pOut.Add("dashboardsandreports")
pOut.Add("dbconnect")
pOut.Add("dbprovisioner")
pOut.Add("decimals")
pOut.Add("deliveryinfo")
pOut.Add("demanddata")
pOut.Add("deviceapproval")
pOut.Add("deviceappswhitelist")
pOut.Add("device")
pOut.Add("devicedashboard")
pOut.Add("devicemdmconfig")
pOut.Add("devicepolicyappoint")
pOut.Add("deviceprinter")
pOut.Add("discountsettings")
pOut.Add("download")
pOut.Add("drpdashboard")
pOut.Add("drpparameters")
pOut.Add("drpparamters")
pOut.Add("drpppo")
pOut.Add("drpstocksbypo")
pOut.Add("ecommerce")
pOut.Add("ecommercedataexporter")
pOut.Add("ecommercedataimporter")
pOut.Add("ecommercedatasync")
pOut.Add("employee")
pOut.Add("erp")
pOut.Add("erptomobizexporter")
pOut.Add("erptomobizimporter")
pOut.Add("error")
pOut.Add("exceptionhandling")
pOut.Add("expenses")
pOut.Add("externalservices")
pOut.Add("filenotfound")
pOut.Add("fixnetdue")
pOut.Add("followuppo")
pOut.Add("fowstbin")
pOut.Add("franchisor")
pOut.Add("generatetemplate")
pOut.Add("gps")
pOut.Add("gpsdashboard")
pOut.Add("gpsgeofence")
pOut.Add("hacktest")
pOut.Add("home")
pOut.Add("inventorycount")
pOut.Add("inventoryform")
pOut.Add("inventoryledger")
pOut.Add("invoicepickeditems")
pOut.Add("itemmodifiers")
pOut.Add("jmtest")
pOut.Add("knowledgemanagementandquestionaire")
pOut.Add("konstruksyon")
pOut.Add("lazyupdate")
pOut.Add("login3")
pOut.Add("loyaltyprogram")
pOut.Add("malllessorayala")
pOut.Add("malllessor")
pOut.Add("malllessorsm")
pOut.Add("manualflush")
pOut.Add("materialsrequisition")
pOut.Add("mobizapp")
pOut.Add("mobizexport")
pOut.Add("occupation")
pOut.Add("offlinedata")
pOut.Add("onlinestorecategories")
pOut.Add("onlinestorehome")
pOut.Add("onlinestoremycompanylist")
pOut.Add("onlinestoremylist")
pOut.Add("onlinestoreorders")
pOut.Add("ordermanagement")
pOut.Add("organization")
pOut.Add("organizationshort")
pOut.Add("paymentterms")
pOut.Add("people")
pOut.Add("personprofilesummary")
pOut.Add("philhr")
pOut.Add("pickproducts")
pOut.Add("pimfeature")
pOut.Add("pimfeaturegroup")
pOut.Add("pimmeasure")
pOut.Add("pimmeasuresign")
pOut.Add("pimproduct")
pOut.Add("pimproductfamily")
pOut.Add("pimproductprice")
pOut.Add("pimtemplate")
pOut.Add("pimunspsccommodityfeature")
pOut.Add("pimupload")
pOut.Add("planogram")
pOut.Add("planograms")
pOut.Add("possettings")
pOut.Add("possetup")
pOut.Add("privateserver")
pOut.Add("productgroup")
pOut.Add("productmaker")
pOut.Add("profile")
pOut.Add("projectpurchaseorder")
pOut.Add("prowebgrid")
pOut.Add("purchaseorder")
pOut.Add("receiptsettings")
pOut.Add("receiving")
pOut.Add("reportspos")
pOut.Add("reporttoprincipal")
pOut.Add("retaildashboard")
pOut.Add("revenue")
pOut.Add("salarygrade")
pOut.Add("salesmenu")
pOut.Add("salespayments")
pOut.Add("salesreturns")
pOut.Add("scdandpwdreports")
pOut.Add("schemasyncreceiver")
pOut.Add("schemasyncsender")
pOut.Add("securityrole")
pOut.Add("selftest")
pOut.Add("serveragent")
pOut.Add("setupchecklist")
pOut.Add("shelfandbinlabeling")
pOut.Add("shoppertrak")
pOut.Add("signin")
pOut.Add("signup")
pOut.Add("sms")
pOut.Add("sqliteandroid")
pOut.Add("sqlite")
pOut.Add("stock")
pOut.Add("stockinfo")
pOut.Add("subscription")
pOut.Add("survey")
pOut.Add("systembehaviorsettings")
pOut.Add("tableview")
pOut.Add("tagsku")
pOut.Add("tenantmonitoringsystem")
pOut.Add("test")
pOut.Add("triggers")
pOut.Add("updateserver")
pOut.Add("upload")
pOut.Add("utilities")
pOut.Add("vendor")
pOut.Add("vendorportal")
pOut.Add("verifyemail")
pOut.Add("vocabularylabel")
pOut.Add("warehousereleasingtracker")
pOut.Add("welcome3")
pOut.Add("welcome")
pOut.Add("wifi")
pOut.Add("zapatero")

' ReservedNames-END-TOKEN

        Return pOut

    End Function

    Public Function Http404(ByVal url As String) As String

        Return "Page not found</br>" & HttpUtility.HtmlEncode(url)

    End Function

    Public Function Denied(ByVal ReturnUrl As String)

        Return "Access denied</br>" & HttpUtility.HtmlEncode(ReturnUrl)

    End Function
End Class
