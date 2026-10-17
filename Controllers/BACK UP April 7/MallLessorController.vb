Public Class MallLessorController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()

    End Function

    <HttpPost()>
    Public Function SetMall() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If IsGuid(WebRequest.Instance.FowstID) = False Then Return JsonDictionary.Serialize("Error", "Invalid FOWST")

        If CStr2(Common.WebRequest.Data("LoginHash")).Length <> 50 Then
            Return JsonDictionary.Serialize("Error", "Invalid Login Hash")
        End If
        LoginData.Add(WebRequest.Instance.OrgID, _
            WebRequest.Instance.FowstID, _
            Common.WebRequest.Data("Mall"), _
            WebSession.Instance.PeopleUserID, _
            Common.WebRequest.Data("LoginHash"))

        Return JsonDictionary.Serialize("Result", "ok")

    End Function

    <HttpGet()>
    Public Function GetLogin() As String

        Dim pLoginHash As String
        Dim pDR, pDrDb As DataRow
        Dim pOut As JsonDictionary
        Dim pThread As Threading.Thread

        pLoginHash = Common.WebRequest.QueryString("LoginHash")
        If CStr2(pLoginHash).Length <> 50 Then
            Return JsonDictionary.Serialize("Error", "Invalid login hash")
        End If

        pThread = TimedThread( _
            Sub()
                Do
                    pDR = LoginData.GetByLoginHash(pLoginHash)
                    If pDR Is Nothing Then
                        Try
                            Threading.Thread.Sleep(5000)
                        Catch ex As Threading.ThreadInterruptedException
                            Exit Sub
                        End Try
                    Else
                        Exit Sub
                    End If
                Loop
            End Sub
        )
        pThread.Join(60 * 1000)
        If pThread.IsAlive Then pThread.Interrupt()

        If pDR Is Nothing Then
            Return JsonDictionary.Serialize("Error", "Login again")
        End If
        If CDate(pDR("Start")).Date <> Now.Date Then
            Return JsonDictionary.Serialize("Error", "Expired login from another day. Please login again")
        End If

        pDrDb = WebRequest.Instance.Business.LibraryOrganization.ByID(CStr2(pDR("OrganizationID")))
        If pDrDb Is Nothing Then
            Return JsonDictionary.Serialize("Error", "No matching organization")
        End If
        If CStr2(pDrDb("Database")) = "" Then
            Return JsonDictionary.Serialize("Error", "Organization is not a subscriber")
        End If
        Common.WebRequest.ProData.ExecuteSQL("USE [" & pDrDb("Database") & "]")

        pDR = FillInfo(pDR)
        If pDR Is Nothing Then
            Return JsonDictionary.Serialize("Error", "No valid record. Login again")
        End If

        pOut = New JsonDictionary
        pOut.Add("Result", "ok")
        pOut.Add("data", pDR)

        Return pOut.Serialize

    End Function

    Private Function FillInfo(vDR As DataRow) As DataRow

        Dim pDR As DataRow
        Dim pSql As String

        pSql = "SELECT MobizLibraryOrganization.CompanyName" & vbCrLf & _
            " ,MobizLibraryOrganizationFOWST.Name AS FowstName" & vbCrLf & _
            " ,ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeoplePreNominalSuffix.Code)) + ' ', ' '), '') +" & vbCrLf & _
            " 	ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.Surname)) + ', ',', '), '') +" & vbCrLf & _
            " 	ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.FirstName)) + ' ',' '), '') +" & vbCrLf & _
            " 	ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.MiddleName)) + ' ',' '), '') +" & vbCrLf & _
            " 	ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryPeople.PersonalSuffixName)),', '), '') +" & vbCrLf & _
            " 	ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(MobizLibraryPeople.ID))),' '), '') AS PeopleName" & vbCrLf & _
            ",tblMall.ReportOutputFolder" & vbCrLf &
            " FROM MobizLibraryOrganization" & vbCrLf & _
            " JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.OrganizationID = MobizLibraryOrganization.ID" & vbCrLf & _
            " JOIN LibraryOrganizationEmployeeFOWST ON LibraryOrganizationEmployeeFOWST.FowstID = MobizLibraryOrganizationFOWST.ID" & vbCrLf & _
            " JOIN LibraryOrganizationEmployee ON LibraryOrganizationEmployee.PeopleID = LibraryOrganizationEmployeeFOWST.PeopleID" & vbCrLf & _
            " JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = LibraryOrganizationEmployeeFOWST.PeopleID" & vbCrLf & _
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf
        Select Case CStr2(vDR("Mall"))
            Case "Ayala Mall", ""
                pSql &= " JOIN MobizLibraryOrganizationFOWSTMallInfoAyala tblMall ON tblMall.FOWSTID = MobizLibraryOrganizationFOWST.ID" & vbCrLf
            Case "SM Mall"
                pSql &= " JOIN MobizLibraryOrganizationFOWSTMallInfoSM tblMall ON tblMall.FOWSTID = MobizLibraryOrganizationFOWST.ID" & vbCrLf
            Case Else
                Return Nothing

        End Select
        pSql &= " WHERE MobizLibraryPeople.ID = " & ReadyString(vDR("PeopleID")) & vbCrLf &
            " AND LibraryOrganizationEmployeeFOWST.FowstID = " & ReadyString(vDR("FowstID")) & vbCrLf
        pDR = Common.WebRequest.ProData.ExecuteRow(pSql)

        Return pDR

    End Function

    <HttpPost()>
    Public Function UpdateOutputFolder() As String

        Dim pLoginHash As String
        Dim pDR, pDrDb As DataRow
        Dim pOut As JsonDictionary
        Dim pSql As String
        Dim pIU As InsertUpdate

        pLoginHash = Common.WebRequest.Data("LoginHash")
        If CStr2(pLoginHash).Length <> 50 Then
            Return JsonDictionary.Serialize("Error", "Invalid login hash")
        End If
        pDR = LoginData.GetByLoginHash(pLoginHash)
        If pDR Is Nothing Then
            Return JsonDictionary.Serialize("Error", "No valid record. Login again")
        End If
        If CDate(pDR("Start")).Date <> Now.Date Then
            Return JsonDictionary.Serialize("Error", "Expired login from another day. Please login again")
        End If

        Select Case CStr2(pDR("Mall"))
            Case "Ayala Mall", ""
                pIU = New InsertUpdate("MobizLibraryOrganizationFOWSTMallInfoAyala")

            Case "SM Mall"
                pIU = New InsertUpdate("MobizLibraryOrganizationFOWSTMallInfoSM")

            Case Else
                Return JsonDictionary.Serialize("Error", "Invalid mall: " & pDR("Mall"))
        End Select

        pIU("ReportOutputFolder") = ReadyString(Common.WebRequest.Data("OutputFolder"))
        pSql = pIU.GetUpdate("FOWSTID = " & ReadyString(pDR("FowstID")))
        If Common.WebRequest.ProData.ExecuteSQL(pSql, True) > 0 Then
            Return JsonDictionary.Serialize("Result", "ok")
        Else
            Return JsonDictionary.Serialize("Error", "Nothing was updated. No matching record.")
        End If

    End Function

    <HttpGet()>
    Function GenerateReportFile() As String
        Dim strGeneratedLink As String
        Dim clsMallLessorSM As New MallLessorSM
        Dim clsMallLessorAyala As New MallLessorAyala
        Dim dtaTable As DataTable
        Dim dtDate As Date
        Dim strTenantCode As String
        Dim strTenantName As String
        Dim strReturn As String

        Dim pLoginHash As String
        Dim pDR, pDrDb As DataRow
        Dim pOut As JsonDictionary

        pLoginHash = Common.WebRequest.QueryString("LoginHash")
        If CStr2(pLoginHash).Length <> 50 Then
            Return JsonDictionary.Serialize("Error", "Invalid login hash")
        End If
        pDR = LoginData.GetByLoginHash(pLoginHash)
        If pDR Is Nothing Then
            Return JsonDictionary.Serialize("Error", "Login again")
        End If
        If CDate(pDR("Start")).Date <> Now.Date Then
            Return JsonDictionary.Serialize("Error", "Expired login from another day. Please login again")
        End If

        pDrDb = WebRequest.Instance.Business.LibraryOrganization.ByID(CStr2(pDR("OrganizationID")))
        If pDrDb Is Nothing Then
            Return JsonDictionary.Serialize("Error", "No matching organization")
        End If
        If CStr2(pDrDb("Database")) = "" Then
            Return JsonDictionary.Serialize("Error", "Organization is not a subscriber")
        End If
        Common.WebRequest.ProData.ExecuteSQL("USE [" & pDrDb("Database") & "]")

        If (Common.WebRequest.Data("SalesDate") = "Yesterday") Then
            dtDate = DateTime.Today.AddDays(-1)
        Else
            dtDate = DateTime.Today
        End If

        If (CStr2(pDR("Mall")) = "Ayala Mall") Then
            dtaTable = clsMallLessorAyala.getAyalaTenantInfo(CStr2(pDR("FowstID")))

            If (dtaTable.Rows.Count > 0) Then
                strGeneratedLink = clsMallLessorAyala.GenerateLessorSalesReport(dtDate, dtaTable.Rows(0).Item("TenantCode").ToString(), dtaTable.Rows(0).Item("TenantName").ToString(),
                    CStr2(pDR("OrganizationID")), CStr2(pDR("FowstID")))
                strGeneratedLink = "MallLessorAyala/DownloadReportFile?ReportLink=" & Uri.EscapeDataString(strGeneratedLink)
            End If

            pOut = New JsonDictionary
            pOut.Add("Result", "ok")
            pOut.Add("GeneratedReport", strGeneratedLink)
        ElseIf (CStr2(pDR("Mall")) = "SM Mall") Then

            strGeneratedLink = clsMallLessorSM.GenerateLessorSalesReport(dtDate, False, CStr2(pDR("OrganizationID")), CStr2(pDR("FowstID")))
            strGeneratedLink = "MallLessorSM/DownloadReportFile?ReportOrgID=" & CStr2(pDR("OrganizationID")) & "&ReportFowstID=" & CStr2(pDR("FowstID")) & "&ReportLink=" & Uri.EscapeDataString(strGeneratedLink)

            pOut = New JsonDictionary
            pOut.Add("Result", "ok")
            pOut.Add("GeneratedReport", strGeneratedLink)
        End If


        Return pOut.Serialize

    End Function

    Public Class LoginData
        Private Shared mFilename As String

        Shared Sub New()

            mFilename = Common.ServerPath & "temp\LoginData.xml"

        End Sub

        Private Shared Sub Save(vDT As DataTable)

            vDT.WriteXml(mFilename, XmlWriteMode.WriteSchema)

        End Sub

        Private Shared Function Load() As DataTable

            Dim pDT As DataTable
            Static sLock As New Object

            SyncLock sLock
                Try
                    If IO.File.Exists(mFilename) = False Then
                        Return NewTable()
                    End If
                    pDT = New DataTable
                    pDT.ReadXml(mFilename)

                    Return pDT
                Catch ex As System.Exception
                    Stop2()
                    Return NewTable()
                End Try
            End SyncLock

        End Function

        Private Shared Function NewTable() As DataTable

            Dim pDT As DataTable

            pDT = New DataTable
            pDT.TableName = "data"
            With pDT.Columns
                .Add("OrganizationID", GetType(Guid))
                With .Add("FowstID", GetType(Guid))
                    .Unique = True
                End With
                .Add("LoginHash")
                .Add("Start", GetType(Date))
                .Add("PeopleID", GetType(Guid))
                .Add("Mall")
            End With

            Return pDT

        End Function

        Public Shared Sub Add(vOrganizationID As String, vFowstID As String, vMall As String, vPeopleID As String, vLoginHash As String)

            Dim pDT As DataTable
            Dim pDR As DataRow
            Static sLock As New Object

            Dim pDTFowst As DataTable
            Dim clsOrganization As New Organization
            Dim strMall As String

            pDTFowst = clsOrganization.getOrgFOWSTInfo(vFowstID)
            strMall = ""
            If (pDTFowst.Rows.Count > 0) Then
                If (Convert.ToBoolean(pDTFowst.Rows(0).Item("IsFOWSTInsideAMall"))) Then
                    strMall = pDTFowst.Rows(0).Item("FOWSTMallName").ToString()
                End If
            End If


            SyncLock sLock
                pDR = GetByFowst(vFowstID, pDT)
                If pDR IsNot Nothing Then pDR.Delete()
                pDR = pDT.NewRow
                pDR("OrganizationID") = New Guid(vOrganizationID)
                pDR("FowstID") = New Guid(vFowstID)
                pDR("LoginHash") = vLoginHash
                pDR("Start") = Now
                pDR("PeopleID") = New Guid(vPeopleID)
                pDR("Mall") = strMall
                pDT.Rows.Add(pDR)
                Save(pDT)
            End SyncLock

        End Sub

        Private Shared Function GetByFowst(vFowstID As String, Optional ByRef oDT As DataTable = Nothing) As DataRow

            Dim pDV As DataView

            oDT = Load()

            pDV = New DataView(oDT, "FowstID = " & ReadyString(vFowstID), "", DataViewRowState.CurrentRows)
            If pDV.Count > 0 Then
                Return pDV(0).Row
            End If

        End Function

        Public Shared Function GetByLoginHash(vLoginHash As String, Optional ByRef oDT As DataTable = Nothing) As DataRow

            Dim pDV As DataView

            oDT = Load()

            pDV = New DataView(oDT, "LoginHash = " & ReadyString(vLoginHash), "", DataViewRowState.CurrentRows)
            If pDV.Count > 0 Then
                Return pDV(0).Row
            End If

        End Function
    End Class
End Class
