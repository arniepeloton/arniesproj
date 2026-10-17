Public Class SelfTestController
    Inherits System.Web.Mvc.Controller

    '
    ' GET: /SelfTest

    Function Index() As ActionResult
        Return View()
    End Function

    Function Failed() As ActionResult
        Return View()
    End Function

    <HttpPost()>
    Public Function AddToDeviceFowstTest() As String
        Dim p As New InsertUpdate("LibraryDevice")
        Dim pSql As String
        Dim strIDReturn As String
        Dim pDT As DataTable
        Try
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "SELECT ID FROM LibraryDevice WHERE UUID = " & ReadyString(Common.WebRequest.Data("UUID"))
            pDT = Common.WebRequest.ProData.Execute(pSql)

            If (pDT.Rows.Count > 0) Then



                pSql = "DELETE FROM TransCustomerPayment where ID in " &
              "(SELECT CustomerPaymentID FROM TransCustomerPaymentInvoices where InvoiceID IN " &
              "(SELECT ID FROM TransInvoice WHERE IsTestOnly = 1 AND DeviceID = " & ReadyString(pDT.Rows(0).Item("ID")) & "))"
                Common.WebRequest.ProData.Execute(pSql)

                pSql = "DELETE FROM TransCustomerPaymentInvoices WHERE InvoiceID IN " &
                    "(SELECT ID FROM TransInvoice WHERE IsTestOnly = 1 AND DeviceID = " & ReadyString(pDT.Rows(0).Item("ID")) & ")"
                Common.WebRequest.ProData.Execute(pSql)

                pSql = "DELETE FROM TransInventoryLedger WHERE TransactionID IN " &
                                "(SELECT ID FROM TransInvoice WHERE IsTestOnly = 1 AND DeviceID = " & ReadyString(pDT.Rows(0).Item("ID")) & ")"
                Common.WebRequest.ProData.Execute(pSql)

                pSql = "DELETE FROM TransMemoSalesReturn WHERE InvoiceID IN " &
                                "(SELECT ID FROM TransInvoice WHERE IsTestOnly = 1 AND DeviceID = " & ReadyString(pDT.Rows(0).Item("ID")) & ")"
                Common.WebRequest.ProData.Execute(pSql)

                'update invoice num first in order to delete invoice items
                pSql = "UPDATE TransInvoice SET InvoiceNumber = 0 WHERE IsTestOnly = 1 AND DeviceID = " & ReadyString(pDT.Rows(0).Item("ID"))
                Common.WebRequest.ProData.Execute(pSql)

                pSql = "DELETE FROM TransInvoiceItems WHERE InvoiceID IN " &
                  "(SELECT ID FROM TransInvoice WHERE IsTestOnly = 1 AND DeviceID = " & ReadyString(pDT.Rows(0).Item("ID")) & ")"
                Common.WebRequest.ProData.Execute(pSql)


                pSql = "DELETE FROM TransInvoice WHERE IsTestOnly = 1 AND DeviceID = " & ReadyString(pDT.Rows(0).Item("ID"))
                Common.WebRequest.ProData.Execute(pSql)


                pSql = "DELETE FROM LibraryDeviceFowst WHERE DeviceID = " & ReadyString(pDT.Rows(0).Item("ID"))
                Common.WebRequest.ProData.ExecuteSQL(pSql)

                pSql = "DELETE FROM LibraryDevice WHERE UUID = " & ReadyString(Common.WebRequest.Data("UUID"))
                Common.WebRequest.ProData.ExecuteSQL(pSql)

            End If

            p("UUID") = ReadyString(Common.WebRequest.Data("UUID"))
            p("IsActive") = 1
            p("LastUpdated") = "GETDATE()"


            p("ID") = ReadyString(Common.WebRequest.Data("DeviceID"))

            pSql = p.GetInsert()

            Common.WebRequest.ProData.ExecuteSQL(pSql)



            'add to device fowst
            p = New InsertUpdate("LibraryDeviceFowst")
            p("DeviceID") = ReadyString(Common.WebRequest.Data("DeviceID"))
            p("FowstID") = ReadyString(Common.WebRequest.Data("FowstID"))
            p("LastUpdated") = "GETDATE()"

            pSql = p.GetInsert()

            Common.WebRequest.ProData.ExecuteSQL(pSql)


            pSql = "SELECT ID FROM LibraryOrganizationEmployeeFOWST WHERE PeopleID = " & ReadyString(Common.WebRequest.Data("PeopleID")) & " AND FowstID = " & ReadyString(Common.WebRequest.Data("FowstID"))
            pDT = Common.WebRequest.ProData.Execute(pSql)

            If (pDT.Rows.Count < 1) Then

                'add to emp fowst
                p = New InsertUpdate("LibraryOrganizationEmployeeFOWST")
                p("PeopleID") = ReadyString(Common.WebRequest.Data("PeopleID"))
                p("FowstID") = ReadyString(Common.WebRequest.Data("FowstID"))
                p("LastUpdated") = "GETDATE()"

                pSql = p.GetInsert()

                Common.WebRequest.ProData.ExecuteSQL(pSql)

            End If


            pSql = "SELECT ID FROM LibraryOrganizationEmployee WHERE PeopleID = " & ReadyString(Common.WebRequest.Data("PeopleID"))
            pDT = Common.WebRequest.ProData.Execute(pSql)

            If (pDT.Rows.Count < 1) Then

                'add to emp 
                p = New InsertUpdate("LibraryOrganizationEmployee")
                p("PeopleID") = ReadyString(Common.WebRequest.Data("PeopleID"))
                p("DateHired") = "GETDATE()"
                p("LastUpdated") = "GETDATE()"

                pSql = p.GetInsert()

                Common.WebRequest.ProData.ExecuteSQL(pSql)

            End If



            'Else
            '    pSql = "SELECT ID FROM LibraryDeviceFowst WHERE DeviceID = " & ReadyString(Common.WebRequest.Data("DeviceID")) & " AND FowstID =" & ReadyString(Common.WebRequest.Data("FowstID"))
            '    pDT = Common.WebRequest.ProData.Execute(pSql)

            '    If (pDT.Rows.Count < 1) Then
            '        p = New InsertUpdate("LibraryDeviceFowst")
            '        p("DeviceID") = ReadyString(pDT.Rows(0).Item("ID").ToString())
            '        p("FowstID") = ReadyString(Common.WebRequest.Data("FowstID"))
            '        p("LastUpdated") = "GETDATE()"

            '        pSql = p.GetInsert()

            '        Common.WebRequest.ProData.ExecuteSQL(pSql)

            '    End If

            'End If


            Return JsonDictionary.Serialize("Result", "ok")

        Catch ex As Exception

            Return JsonDictionary.Serialize("Error", ex.Message)

        End Try


    End Function

    <HttpPost()>
    Public Function AddToPeopleOrgTest() As String
        Dim p As New InsertUpdate("MobizLibraryPeopleOrganization")
        Dim pSql As String
        Dim strIDReturn As String
        Dim pDT As DataTable
        Dim orgID As String = "7c794971-1cd2-4ad4-922f-36a93edde09d"
        Try

            pSql = "SELECT ID, IsTerminated FROM MobizLibraryPeopleOrganization WHERE PeopleID = " & ReadyString(Common.WebRequest.Data("PeopleID")) & " AND OrganizationID =" & ReadyString(orgID)
            pDT = Common.WebRequest.ProData.Execute(pSql)

            If (pDT.Rows.Count < 1) Then
                '    If (Convert.ToBoolean(pDT.Rows(0).Item("IsTerminated")) = 1) Then
                '        pSql = "UPDATE MobizLibraryPeopleOrganization SET IsTerminated = 0, DateTimeTerminated = NULL WHERE PeopleID = " & ReadyString(Common.WebRequest.Data("PeopleID")) & " AND OrganizationID =" & ReadyString(orgID)
                '        Common.WebRequest.ProData.Execute(pSql)
                '    End If
                'Else

                p("PeopleID") = ReadyString(Common.WebRequest.Data("PeopleID"))
                p("OrganizationID") = ReadyString(orgID) 'test db
                p("IsConfirmed") = 0
                p("DateTimeInvited") = "GETDATE()" 'required
                p("UserID") = ReadyString(Common.WebRequest.Data("PeopleID"))

                pSql = p.GetInsert()


                Common.WebRequest.ProData.BeginTransaction()
                Common.WebRequest.ProData.ExecuteSQL(pSql)


                Common.WebRequest.ProData.CommitTransaction()


            End If


            Return JsonDictionary.Serialize("Result", "ok")

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
            Return JsonDictionary.Serialize("Error", ex.Message)

        End Try


    End Function

    '<HttpPost()>
    'Public Function UpdatePeopleOrgTest() As String
    '    Dim pIU As InsertUpdate
    '    Dim pSql As String
    '    Dim pDT As DataTable

    '    Dim orgID As String = "7c794971-1cd2-4ad4-922f-36a93edde09d"

    '    Try
    '        'remove. it means, done testing
    '        pSql = "UPDATE MobizLibraryPeopleOrganization SET IsTerminated = 1, DateTimeTerminated = GETDATE() WHERE PeopleID = " & ReadyString(Common.WebRequest.Data("PeopleID")) & " AND OrganizationID =" & ReadyString(orgID)
    '        Common.WebRequest.ProData.Execute(pSql)

    '        Return JsonDictionary.Serialize("Result", "ok")

    '    Catch ex As Exception
    '        Common.WebRequest.ProData.RollbackTransaction()
    '        Return JsonDictionary.Serialize("Error", ex.Message)

    '    End Try


    'End Function

    <HttpPost()>
    Public Function UpdateDeleteTestInvoice() As String
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pDT As DataTable

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        'If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        Try
            '  Common.WebRequest.ProData.BeginTransaction()

            pSql = "DELETE FROM TransCustomerPayment where ID in " &
                "(SELECT CustomerPaymentID FROM TransCustomerPaymentInvoices where InvoiceID IN " &
                "(SELECT ID FROM TransInvoice where IsTestOnly = 1 AND DeviceID = " & ReadyString(Common.WebRequest.Data("DeviceID")) & " AND LastUpdated >= " & ReadyString(Common.WebRequest.Data("TestDate").Trim()) & "))"
            Common.WebRequest.ProData.Execute(pSql)

            pSql = "DELETE FROM TransCustomerPaymentInvoices WHERE InvoiceID IN " &
                "(SELECT ID FROM TransInvoice WHERE IsTestOnly = 1 AND DeviceID = " & ReadyString(Common.WebRequest.Data("DeviceID")) & " AND LastUpdated >= " & ReadyString(Common.WebRequest.Data("TestDate").Trim()) & ")"
            Common.WebRequest.ProData.Execute(pSql)

            pSql = "DELETE FROM TransInventoryLedger WHERE TransactionID IN " &
                            "(SELECT ID FROM TransInvoice WHERE IsTestOnly = 1 AND DeviceID = " & ReadyString(Common.WebRequest.Data("DeviceID")) & " AND LastUpdated >= " & ReadyString(Common.WebRequest.Data("TestDate").Trim()) & ")"
            Common.WebRequest.ProData.Execute(pSql)

            pSql = "DELETE FROM TransMemoSalesReturn WHERE InvoiceID IN " &
                            "(SELECT ID FROM TransInvoice WHERE IsTestOnly = 1 AND DeviceID = " & ReadyString(Common.WebRequest.Data("DeviceID")) & " AND LastUpdated >= " & ReadyString(Common.WebRequest.Data("TestDate").Trim()) & ")"
            Common.WebRequest.ProData.Execute(pSql)

            'update invoice num first in order to delete invoice items
            pSql = "UPDATE TransInvoice SET InvoiceNumber = 0 WHERE IsTestOnly = 1 AND DeviceID = " & ReadyString(Common.WebRequest.Data("DeviceID")) & " AND LastUpdated >= " & ReadyString(Common.WebRequest.Data("TestDate").Trim())
            Common.WebRequest.ProData.Execute(pSql)


            If (UpdateInvoice(Common.WebRequest.Data("DeviceID"), Common.WebRequest.Data("TestDate").Trim())) Then
                Return JsonDictionary.Serialize("Result", "ok")
            Else
                Return JsonDictionary.Serialize("Error", "Not Deleted")

            End If

            'Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            'Common.WebRequest.ProData.RollbackTransaction()

            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function


    Public Function UpdateInvoice(ByVal deviceid As String, ByVal testdate As String) As Boolean
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pDT As DataTable
        Dim counter As Integer = 0

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        'If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        Try



            While (counter < 3)
                pSql = "SELECT ID FROM TransInvoice WHERE InvoiceNumber > 0 AND IsTestOnly = 1 AND DeviceID = " & ReadyString(deviceid) & " AND LastUpdated >= " & ReadyString(testdate)
                pDT = Common.WebRequest.ProData.Execute(pSql)

                If (pDT.Rows.Count > 0) Then

                    'update invoice num first in order to delete invoice items
                    pSql = "UPDATE TransInvoice SET InvoiceNumber = 0 WHERE IsTestOnly = 1 AND DeviceID = " & ReadyString(deviceid) & " AND LastUpdated >= " & ReadyString(testdate)
                    Common.WebRequest.ProData.Execute(pSql)

                Else
                    pSql = "DELETE FROM TransInvoiceItems WHERE InvoiceID IN " &
                    "(SELECT ID FROM TransInvoice WHERE IsTestOnly = 1 AND DeviceID = " & ReadyString(deviceid) & " AND LastUpdated >= " & ReadyString(testdate) & ")"
                    Common.WebRequest.ProData.Execute(pSql)


                    pSql = "DELETE FROM TransInvoice WHERE IsTestOnly = 1 AND DeviceID = " & ReadyString(deviceid) & " AND LastUpdated >= " & ReadyString(testdate)
                    Common.WebRequest.ProData.Execute(pSql)

                    Exit While
                End If

                counter = counter + 1

            End While

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function


    <HttpPost()>
    Public Function GetTransInvoiceTotal() As String
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pDT As DataTable
        Dim pSql2 As String
        Dim pDT2 As DataTable
        Dim pSql3 As String
        Dim pDT3 As DataTable
        Dim pSql4 As String
        Dim pDT4 As DataTable
        Dim totalamtActual As Decimal
        Dim pOut As JsonDictionary
        Dim invoiceDate As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        'If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        Try

            pSql = "SELECT DISTINCT itm.InvoiceID from TransInvoiceItems itm INNER JOIN TransInvoice inv ON itm.InvoiceID = inv.ID WHERE inv.IsTestOnly = 1 AND inv.DeviceID = " &
                ReadyString(Common.WebRequest.Data("DeviceID").Trim()) & " AND inv.FowstID >= " & ReadyString(Common.WebRequest.Data("FowstID").Trim()) &
                " AND CONVERT(datetime, CONVERT(varchar, inv.LastUpdated, 120)) >= " & ReadyString(Common.WebRequest.Data("TestDate").Trim())

            pDT = Common.WebRequest.ProData.Execute(pSql)


            For Each pDR As DataRow In pDT.Rows
                pSql2 = "SELECT SUM(UnitPrice * Quantity) as Total from TransInvoiceItems WHERE InvoiceID = " & ReadyString(pDR("InvoiceID"))

                pDT2 = Common.WebRequest.ProData.Execute(pSql2)

                totalamtActual = CDec2(totalamtActual) + CDec2(pDT2.Rows(0).Item("Total"))
            Next pDR


            pSql3 = "SELECT itm.ID FROM TransInvoiceItems itm INNER JOIN TransInvoice inv ON itm.InvoiceID = inv.ID WHERE inv.IsTestOnly = 1 AND inv.InvoiceNumber > 0 AND inv.DeviceID = " &
                ReadyString(Common.WebRequest.Data("DeviceID").Trim()) & " AND inv.FowstID = " & ReadyString(Common.WebRequest.Data("FowstID").Trim()) &
                " AND CONVERT(datetime, CONVERT(varchar, inv.LastUpdated, 120)) >= " & ReadyString(Common.WebRequest.Data("TestDate").Trim())

            pDT3 = Common.WebRequest.ProData.Execute(pSql3)

            pSql4 = "SELECT itm.InvoiceID, itm.ProductID, itm.Quantity, itm.ActualQuantity, itm.Unitmeasure, Count(itm.ProductID) as Count " &
            "FROM TransInvoiceItems itm INNER JOIN TransInvoice inv ON itm.InvoiceID = inv.ID WHERE inv.IsTestOnly = 1 AND inv.InvoiceNumber > 0 AND inv.DeviceID = " &
            ReadyString(Common.WebRequest.Data("DeviceID").Trim()) & " AND inv.FowstID >= " & ReadyString(Common.WebRequest.Data("FowstID").Trim()) &
            " AND CONVERT(datetime, CONVERT(varchar, inv.LastUpdated , 120)) >= " & ReadyString(Common.WebRequest.Data("TestDate").Trim()) &
            " GROUP BY itm.ProductID, itm.InvoiceID,itm.Quantity, itm.ActualQuantity, itm.UnitMeasure " &
            "HAVING Count(*) > 1"

            pDT4 = Common.WebRequest.ProData.Execute(pSql4)


            'Common.WebRequest.ProData.CommitTransaction()

            pOut = New JsonDictionary
            pOut.Add("Data", totalamtActual)
            pOut.Add("Count", pDT.Rows.Count)
            pOut.Add("ItemCount", pDT3.Rows.Count)

            If pDT4.Rows.Count > 0 Then
                pOut.Add("Duplicate", "Yes")
            Else
                pOut.Add("Duplicate", "No")

            End If

        Catch ex As Exception
            'Common.WebRequest.ProData.RollbackTransaction()
            pOut.Add("Error", ex.Message)

        End Try

        Return pOut.Serialize
    End Function

    <HttpPost()>
    Public Function CheckIfTestPassed() As String
        Dim pSql As String
        Dim pDT As DataTable
        Dim pOut As JsonDictionary
        Dim orgid As String = ""
        Dim deviceid As String = ""

        'If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        pOut = New JsonDictionary

        Try
            If CStr2(Common.WebRequest.Data("ActualOrgID")) <> "" Then
                orgid = Common.WebRequest.Data("ActualOrgID").ToString().Trim()
            Else
                Return JsonDictionary.Serialize("Error", "No record")
            End If

            If CStr2(Common.WebRequest.Data("DeviceID")) <> "" Then
                deviceid = Common.WebRequest.Data("DeviceID").ToString().Trim()
            Else
                Return JsonDictionary.Serialize("Error", "No record")
            End If

            pSql = "SELECT ID, Passed FROM MobizTestResults WHERE DeviceID = " &
                ReadyString(deviceid) & " AND OrganizationID = " & ReadyString(orgid)

            pDT = Common.WebRequest.ProData.Execute(pSql)

            If pDT.Rows.Count > 0 Then
                If pDT.Rows(0).Item("Passed") = True Then
                    pOut.Add("Result", "Yes")
                Else
                    pOut.Add("Result", "No")
                End If

            Else
                pOut.Add("Error", "No record")

            End If

        Catch ex As Exception
            pOut.Add("Error", ex.Message)

        End Try

        Return pOut.Serialize
    End Function



    <HttpPost()>
    Function SendTestResult() As String
        Dim pOut As JsonDictionary
        Dim strBody As String
        Dim email As String
        Dim actualorgID As String
        Dim deviceID As String
        Dim peopleUserID As String
        Dim deviceModel As String
        Dim devicePlatform As String
        Dim remarks As String
        Dim result As String
        Dim duration As String
        Dim transCount As Integer
        Dim lineCount As Integer
        Dim localDuration As String
        Dim p As New InsertUpdate("MobizTestResults")
        Dim pSql As String
        Dim strIDReturn As String
        Dim pDT As DataTable
        Dim orgName As String
        Dim orgSKU As Integer

        Try
            'Construct the message
            email = "janine.son@mobiz-apps.com, robert.chong@mobiz-apps.com, francis.hernandez@mobiz-apps.com, francis.adan@mobiz-apps.com, arvin.meralpis@mobiz-apps.com"
            actualorgID = Common.WebRequest.Data("ActualOrgID").ToString().Trim()
            deviceID = Common.WebRequest.Data("DeviceID").ToString().Trim()
            peopleUserID = Common.WebRequest.Data("PeopleUserID").ToString().Trim()
            deviceModel = Common.WebRequest.Data("DeviceModel").ToString().Trim()
            devicePlatform = Common.WebRequest.Data("DevicePlatform").ToString().Trim()
            remarks = Common.WebRequest.Data("Remarks").ToString().Trim()
            result = Common.WebRequest.Data("Result").ToString().Trim()

            'TODO
            If CStr2(Common.WebRequest.Data("TransCount")) <> "" Then
                transCount = Common.WebRequest.Data("TransCount").ToString().Trim()
            End If

            If CStr2(Common.WebRequest.Data("LineCount")) <> "" Then
                lineCount = Common.WebRequest.Data("LineCount").ToString().Trim()
            End If

            If CStr2(Common.WebRequest.Data("LocalDuration")) <> "" Then
                localDuration = Common.WebRequest.Data("LocalDuration").ToString().Trim()
            End If



            If CStr2(Common.WebRequest.Data("OrgSKU")) <> "" Then
                orgSKU = Common.WebRequest.Data("OrgSKU").ToString().Trim()
            End If

            Dim serverDuration As String

            If CStr2(Common.WebRequest.Data("StartDateServer")) <> "" Then
                Dim startDate As Date = Convert.ToDateTime(Common.WebRequest.Data("StartDateServer").ToString().Trim())

                serverDuration = (Date.Now - startDate).ToString("hh':'mm':'ss")

            End If


            'add 
            p("PeopleID") = ReadyString(peopleUserID)

            If result = "pass" Then
                p("Passed") = 1
            Else
                p("Passed") = 0
            End If


            p("Remarks") = ReadyString(remarks)
            p("DeviceModel") = ReadyString(deviceModel)
            p("Platform") = ReadyString(devicePlatform)
            p("NoOfTransactions") = transCount
            p("NoOfLineItems") = lineCount
            p("LocalDuration") = ReadyString(localDuration)
            p("ServerDuration") = ReadyString(serverDuration)
            p("NoOfOrgSKU") = orgSKU
            p("TestDate") = "GETDATE()"

            pSql = "SELECT ID, Passed FROM MobizTestResults WHERE DeviceID = " &
                ReadyString(deviceID) & " AND OrganizationID = " &
                ReadyString(actualorgID)

            pDT = Common.WebRequest.ProData.Execute(pSql)

            If pDT.Rows.Count > 0 Then
                pSql = p.GetUpdate("ID = " & ReadyString(pDT.Rows(0).Item("ID")))
            Else
                p("DeviceID") = ReadyString(deviceID)
                p("OrganizationID") = ReadyString(actualorgID)
                pSql = p.GetInsert()
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)


            If CStr2(Common.WebRequest.Data("OrgName")) <> "" Then
                orgName = Common.WebRequest.Data("OrgName").ToString().Trim()
            End If


            '<h2>Mobiz Data Integrity Test Report " & Date.Now.ToString() & "</h2><hr/>
            strBody = "<div style='font-size: 16; font-family: Arial;'>" &
                    "<h3>Result: " & result & "</h3><hr/>" &
                    "<p>Issue/s found: " & remarks & "</p>" &
                    "<p>Organization: " & orgName & "</p>" &
                    "<p>Transaction Count: " & transCount & "</p>" &
                    "<p>Total No. of Items: " & lineCount & "</p>" &
                    "<p>Raw Data vs Local Data Test Duration (includes generating random data): " & localDuration & "</p>" &
                    "<p>Local Data vs Server Data Test Duration (includes flushing data): " & serverDuration & "</p>" &
                    "<br/><h3>Device Details</h3><hr/>" &
                    "<p>Model: " & deviceModel & "</p>" &
                    "<p>Platform: " & devicePlatform & "</p><br/><br/>" &
                    "<i>This is an automatically generated email. Please do not reply.</i></div>"


#If DEBUG Then
            SendMail("customercare@mobiz-apps.com",
               "janine.son@mobiz-apps.com",
               "Mobiz",
               "Mobiz Data Integrity Test Report " & Date.Now.ToString(),
               strBody,
               "smtp.live.com",
               587,
               "customercare@mobiz-apps.com",
               "MobizCustCare2221", , , , , True)

#Else
                             SendMail("customercare@mobiz-apps.com",
                 email,
                 "Mobiz",
                 "Mobiz Data Integrity Test Report " & Date.Now.ToString(),
                 strBody,
                 "smtp.live.com",
                 587,
                 "customercare@mobiz-apps.com",
                 "MobizCustCare2221", , , , , True)

#End If


            pOut = New JsonDictionary
            pOut.Add("Result", "True")

            Return pOut.Serialize

        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", "An error occured while sending email: " & ex.Message)
        End Try

    End Function

End Class
