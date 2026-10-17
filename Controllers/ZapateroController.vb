Imports System.Net

Namespace MobizApplication
    Public Class ZapateroController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /SignIn

        Function Index() As ActionResult

            'If CStr2(Common.WebRequest.QueryString("mode")).ToLower = "logout" Then
            '    Common.WebSession.UserID = ""
            'End If

            'FormsAuthentication.SignOut()

            'Response.Cache.SetCacheability(HttpCacheability.NoCache)
            'Response.Cache.SetNoStore()

            'Return View()

        End Function

        Function Project() As ActionResult
            Return View()
        End Function

        Function CRM() As ActionResult
            Return View()
        End Function

        Function KB() As ActionResult
            Return View()
        End Function

        Function DownloadDatabase() As ActionResult
            Return View()
        End Function

        Function TraxInfo() As ActionResult
            Return View()
        End Function

        Function Notice() As ActionResult
            Return View()
        End Function

        Function TransactionDetail() As ActionResult
            Return View()
        End Function

        Function TaxPreparation() As ActionResult
            Return View()
        End Function

        Function Main() As ActionResult
            Return View()
        End Function

        <HttpPost()>
        Function CheckNeedEmailVerification() As String
            Dim clsPeople As New SignUp
            Dim strReturn As String
            Dim dt As DataTable

            clsPeople.PeopleID = Common.WebRequest.Data("ID")

            dt = clsPeople.CheckNeedEmailVerification()

            strReturn = JsonDictionary.Serialize("EmailData", dt)

            If (dt.Rows.Count > 0) Then
                WebSession.Instance.IsEmailVerified = False
            Else
                WebSession.Instance.IsEmailVerified = True
            End If

            Return strReturn

        End Function

        <HttpPost()>
        Function CheckPeopleIsFirstPartySignUp() As String
            Dim clsPeople As New SignUp
            Dim strReturn As String

            clsPeople.PeopleID = Common.WebRequest.Data("ID")

            strReturn = JsonDictionary.Serialize("PeopleFirstPartySignUpData", clsPeople.CheckPeopleIsFirstPartySignUp())

            Return strReturn

        End Function

        Function GetRegisteredDevices() As String
            Dim pWebClient As New WebClient
            Dim pResult As String
            Dim pJson As JsonDictionary
            Dim pJsonStr As String
            Dim LinkSource As String
            Dim ExternalServices As New ExternalServicesController

            pResult = ExternalServices.GetRegisteredDevices

            '            pJson = New JsonDictionary()
            '            pJson.Add("ID", WebSession.Instance.PeopleID)
            '            pJson.Add("OrgID", CStr2(Common.WebRequest.Data("OrgID")))
            '            pJsonStr = pJson.ToString()

            '#If DEBUG Then
            '            'LinkSource = "124.106.167.156:8083" 'DUC Server
            '            LinkSource = "localhost:8080"
            '#Else
            '            'LinkSource = "122.52.123.251" 'Mobiz Server
            '            LinkSource = "localhost:8083" 'DUC Server
            '#End If

            '            Try
            '                pResult = pWebClient.UploadString("http://" & LinkSource & "/ExternalServices/GetRegisteredDevices", "POST", pJsonStr)
            '            Catch ex As Exception
            '                AppendToDebug("vsMXWE4SRNRM!", , ex, True)
            '            End Try


            '            Threading.Thread.Sleep(5000)

            '            AppendToDebug(pResult)

            Return pResult
        End Function

        Function GetZReadDataFromServer() As String
            Dim pWebClient As New WebClient
            Dim pResult As String
            Dim pJson As JsonDictionary
            Dim pJsonStr As String
            Dim LinkSource As String
            Dim ExternalServices As New ExternalServicesController

            pResult = ExternalServices.GetZReadData

            '            pJson = New JsonDictionary()
            '            pJson.Add("ID", WebSession.Instance.PeopleID)
            '            pJson.Add("OrgID", CStr2(Common.WebRequest.Data("OrgID")))
            '            pJson.Add("RangeType", "Today")
            '            pJsonStr = pJson.ToString()

            '#If DEBUG Then
            '            'LinkSource = "124.106.167.156:8083" 'DUC Server
            '            LinkSource = "localhost:8080"
            '#Else
            '            'LinkSource = "122.52.123.251" 'Mobiz Server
            '            LinkSource = "localhost:8083" 'DUC Server
            '#End If

            '            AppendToDebug("http://" & LinkSource & "/ExternalServices/GetZReadData")
            '            AppendToDebug(pJsonStr)

            '            Try
            '                pResult = pWebClient.UploadString("http://" & LinkSource & "/ExternalServices/GetZReadData", "POST", pJsonStr)
            '            Catch ex As Exception
            '                AppendToDebug("vsMXWE4SRNRM!", , ex, True)
            '            End Try


            '            Threading.Thread.Sleep(5000)

            '            AppendToDebug(pResult)

            Return pResult
        End Function

        Function SendZReadData() As String
            Dim pWebClient As New WebClient
            Dim pResult As String
            Dim pJson As JsonDictionary
            Dim pJsonStr As String
            Dim LinkSource As String

            pJson = New JsonDictionary()
            pJson.Add("ID", WebSession.Instance.PeopleUserID)
            pJson.Add("OrgID", CStr2(Common.WebRequest.Data("OrgID")))
            pJson.Add("RangeType", "Today")
            pJson.Add("ZReadData", Common.WebRequest.AllData("ZReadData"))

            pJsonStr = pJson.ToString()

#If DEBUG Then
            'LinkSource = "localhost:8083" 'DUC Server
            LinkSource = "localhost:8080"
#Else
            'LinkSource = "122.52.123.251" 'Mobiz Server
            LinkSource = "localhost:8083" 'DUC Server
#End If

            pResult = pWebClient.UploadString("http://" & LinkSource & "/ExternalServices/SendZReadData", "POST", pJsonStr)

            Threading.Thread.Sleep(5000)

            Return JsonDictionary.Serialize("Results", pResult)

        End Function

        Function GetSalesTransactionDetails() As String
            Dim pWebClient As New WebClient
            Dim pResult As String
            Dim pJson As JsonDictionary
            Dim pJsonStr As String
            Dim LinkSource As String
            Dim ExternalServices As New ExternalServicesController




            '            pJson = New JsonDictionary()
            '            pJson.Add("ID", WebSession.Instance.PeopleID)
            '            pJson.Add("OrgID", CStr2(Common.WebRequest.Data("OrgID")))
            '            pJson.Add("FOWSTID", CStr2(Common.WebRequest.Data("LocationID")))
            '            pJson.Add("DeviceID", CStr2(Common.WebRequest.Data("DeviceID")))
            '            pJson.Add("DateFrom", CStr2(Common.WebRequest.Data("DateFrom")))
            '            pJson.Add("DateTo", CStr2(Common.WebRequest.Data("DateTo")))
            '            pJson.Add("IsFromESales", CStr2(Common.WebRequest.Data("IsFromESales")))
            '            pJson.Add("ESalesPercentage", CStr2(Common.WebRequest.Data("ESalesPercentage")))

            '            pJsonStr = pJson.ToString()

            Try

                pResult = ExternalServices.GetSalesTransactionDetails()

                '#If DEBUG Then
                '                LinkSource = "localhost:8083" 'DUC Server
                '                LinkSource = "localhost:8080"
                '#Else
                '                LinkSource = "122.52.123.251" 'Mobiz Server
                '                LinkSource = "localhost:8083" 'DUC Server
                '#End If

                '                AppendToLog("http://" & LinkSource & "/ExternalServices/GetSalesTransactionDetails")
                '                AppendToLog(pJsonStr)

                '                pResult = pWebClient.UploadString("http://" & LinkSource & "/ExternalServices/GetSalesTransactionDetails", "POST", pJsonStr)

                Threading.Thread.Sleep(5000)

                'AppendToLog(pResult)
            Catch ex As Exception
                AppendToLog(ex.Message)
            End Try

            Return pResult
        End Function

        Function TagUntagMonthsData() As String
            Dim mYear As Integer
            Dim mMonth As Integer

            Dim pWebClient As New WebClient
            Dim pResult As String
            Dim pJson As JsonDictionary
            Dim pJsonStr As String
            Dim LinkSource As String
            Dim mDateFrom As String
            Dim mDateTo As String

            mYear = CInt2(Common.WebRequest.Data("Year"))
            mMonth = CInt2(Common.WebRequest.Data("Month"))

            'mDateFrom = mMonth & "/1/" & mYear

            'mDateTo = mMonth & "/" & System.DateTime.DaysInMonth(mYear, i) & "/" & mYear

            pJson = New JsonDictionary()
            pJson.Add("ID", WebSession.Instance.PeopleID)
            pJson.Add("OrgID", CStr2(Common.WebRequest.Data("OrgID")))
            pJson.Add("FOWSTID", CStr2(Common.WebRequest.Data("LocationID")))
            pJson.Add("DeviceID", CStr2(Common.WebRequest.Data("DeviceID")))
            pJson.Add("Year", CStr2(mYear))
            pJson.Add("Month", CStr2(mMonth))
            pJson.Add("IsTag", CStr2(Common.WebRequest.Data("IsTag")))

            pJsonStr = pJson.ToString()

            Try

#If DEBUG Then
                'LinkSource = "localhost:8083" 'DUC Server
                LinkSource = "localhost:8080"
#Else
                'LinkSource = "122.52.123.251" 'Mobiz Server
                LinkSource = "localhost:8083" 'DUC Server
#End If

                pResult = pWebClient.UploadString("http://" & LinkSource & "/ExternalServices/TagUntagMonthsData", "POST", pJsonStr)

                Threading.Thread.Sleep(5000)

            Catch ex As Exception
                AppendToLog(ex.Message)
            End Try

            Return pResult
        End Function

        Function GetYearlySales() As String
            Dim mYear As Integer

            Dim pWebClient As New WebClient
            Dim pResult As String
            Dim pJson As JsonDictionary
            Dim pJsonStr As String
            Dim LinkSource As String
            Dim mDateFrom As String
            Dim mDateTo As String

            mYear = CInt2(Common.WebRequest.Data("Year"))

            For i As Integer = 1 To 12

                mDateFrom = i & "/1/" & mYear

                mDateTo = i & "/" & System.DateTime.DaysInMonth(mYear, i) & "/" & mYear

                pJson = New JsonDictionary()
                pJson.Add("ID", WebSession.Instance.PeopleID)
                pJson.Add("OrgID", CStr2(Common.WebRequest.Data("OrgID")))
                pJson.Add("FOWSTID", CStr2(Common.WebRequest.Data("LocationID")))
                pJson.Add("DeviceID", CStr2(Common.WebRequest.Data("DeviceID")))
                pJson.Add("DateFrom", CStr2(mDateFrom))
                pJson.Add("DateTo", CStr2(mDateTo))
                pJson.Add("IsFromESales", CStr2(Common.WebRequest.Data("IsFromESales")))

                pJsonStr = pJson.ToString()

                Try

#If DEBUG Then
                    'LinkSource = "localhost:8083" 'DUC Server
                    LinkSource = "localhost:8080"
#Else
                    'LinkSource = "122.52.123.251" 'Mobiz Server
                    LinkSource = "localhost:8083" 'DUC Server
#End If
                    pResult = pWebClient.UploadString("http://" & LinkSource & "/ExternalServices/GetSalesTransactionDetails", "POST", pJsonStr)

                    'Threading.Thread.Sleep(1000)

                Catch ex As Exception
                    AppendToLog(ex.Message)
                End Try

            Next

            Return ""

        End Function

        Function GetDataHeader() As String
            Dim pWebClient As New WebClient
            Dim pResult As String
            Dim pJson As JsonDictionary
            Dim pJsonStr As String
            Dim LinkSource As String

            pJson = New JsonDictionary()
            pJson.Add("OrgID", CStr2(Common.WebRequest.Data("OrgID")))
            pJson.Add("FOWSTID", CStr2(Common.WebRequest.Data("LocationID")))
            pJson.Add("DeviceID", CStr2(Common.WebRequest.Data("DeviceID")))
            pJson.Add("DateFrom", CStr2(Common.WebRequest.Data("DateFrom")))

            pJsonStr = pJson.ToString()

#If DEBUG Then
            'LinkSource = "124.106.167.156:8083" 'DUC Server
            LinkSource = "localhost:8080"
#Else
            'LinkSource = "122.52.123.251" 'Mobiz Server
            LinkSource = "localhost:8083" 'DUC Server
#End If

            AppendToDebug("http://" & LinkSource & "/ExternalServices/GetDataHeader")
            AppendToDebug(pJsonStr)

            Try
                pResult = pWebClient.UploadString("http://" & LinkSource & "/ExternalServices/GetDataHeader", "POST", pJsonStr)
            Catch ex As Exception
                AppendToDebug("vsMXWE4SRNRM!", , ex, True)
            End Try


            Threading.Thread.Sleep(5000)

            AppendToDebug(pResult)

            Return pResult
        End Function

        '        Function FollowupData() As String
        '            Dim pWebClient As New WebClient
        '            Dim pResult As String
        '            Dim pJson As JsonDictionary
        '            Dim pJsonStr As String
        '            Dim LinkSource As String

        '            pJson = New JsonDictionary()
        '            pJson.Add("OrgID", CStr2(Common.WebRequest.Data("OrgID")))
        '            pJson.Add("FOWSTID", CStr2(Common.WebRequest.Data("LocationID")))
        '            pJson.Add("DeviceID", CStr2(Common.WebRequest.Data("DeviceID")))
        '            pJson.Add("Year", CStr2(Common.WebRequest.Data("Year")))
        '            pJson.Add("Month", CStr2(Common.WebRequest.Data("Month")))

        '            pJsonStr = pJson.ToString()

        '#If DEBUG Then
        '            'LinkSource = "124.106.167.156:8083" 'DUC Server
        '            LinkSource = "localhost:8080"
        '#Else
        '            'LinkSource = "122.52.123.251" 'Mobiz Server
        '            LinkSource = "localhost:8083" 'DUC Server
        '#End If

        '            Try
        '                pResult = pWebClient.UploadString("http://" & LinkSource & "/ExternalServices/GetDataHeader", "POST", pJsonStr)
        '            Catch ex As Exception
        '                AppendToDebug("vsMXWE4SRNRM!", , ex, True)
        '            End Try


        '            Threading.Thread.Sleep(5000)


        '            Return pResult
        '        End Function

        Function SaveTransactionData() As String
            Dim pWebClient As New WebClient
            Dim pResult As String
            Dim pJson As JsonDictionary
            Dim pJsonStr As String
            Dim LinkSource As String
            Dim ExternalServices As New ExternalServicesController

            AppendToLog("before HandleOrganization ")

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            AppendToLog("after HandleOrganization ")

            pResult = ExternalServices.SaveTransactionData()

            AppendToLog("pResult" & pResult)

            '            pJson = New JsonDictionary()
            '            pJson.Add("ID", WebSession.Instance.PeopleID)
            '            pJson.Add("OrgID", CStr2(Common.WebRequest.Data("OrgID")))
            '            pJson.Add("FOWSTID", CStr2(Common.WebRequest.Data("LocationID")))
            '            pJson.Add("FOWSTName", CStr2(Common.WebRequest.Data("LocationName")))
            '            pJson.Add("DeviceID", CStr2(Common.WebRequest.Data("DeviceID")))
            '            pJson.Add("DateFrom", CStr2(Common.WebRequest.Data("DateFrom")))
            '            pJson.Add("DateTo", CStr2(Common.WebRequest.Data("DateTo")))
            '            pJson.Add("IsLastFile", CStr2(Common.WebRequest.Data("IsLastFile")))
            '            pJson.Add("IsFirstBatch", CStr2(Common.WebRequest.Data("IsFirstBatch")))
            '            pJson.Add("TransCount", CStr2(Common.WebRequest.Data("TransCount")))
            '            pJson.Add("TransData", CStr2(Common.WebRequest.Data("TransData")))

            '            pJsonStr = pJson.ToString()

            '#If DEBUG Then
            '            'LinkSource = "localhost:8083" 'DUC Server
            '            LinkSource = "localhost:8080"
            '#Else
            '            'LinkSource = "122.52.123.251" 'Mobiz Server
            '            LinkSource = "localhost:8083" 'DUC Server
            '#End If

            '            AppendToLog(pJsonStr)
            '            AppendToLog("http://" & LinkSource & "/ExternalServices/SaveTransactionData", )

            '            pResult = pWebClient.UploadString("http://" & LinkSource & "/ExternalServices/SaveTransactionData", "POST", pJsonStr)

            Return pResult
        End Function

        Function GetZRead() As String
            Dim ExternalServices As New ExternalServicesController
            Dim pResult As String

            pResult = ExternalServices.GetZRead()

            Return pResult
        End Function

        Function UpdateZReadReportedDay() As String
            Dim ExternalServices As New ExternalServicesController
            Dim pResult As String

            pResult = ExternalServices.UpdateZReadReportedDay()

            Return pResult
        End Function
    End Class
End Namespace