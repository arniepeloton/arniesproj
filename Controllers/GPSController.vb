Namespace WebProj
    Public Class GPSController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /GPS

        Function Index() As ActionResult
            Return View()
        End Function

        Function Settings() As ActionResult
            Return View()
        End Function

        Function SaveGPSCoordinateViaMobile() As ActionResult

            'For PC 5/27/2025

            'Dim MyLocation As String()
            'Dim p As New InsertUpdate("MobizTransDeviceLocation")
            'Dim pSql As String
            'Dim pLine As Dictionary(Of String, String)
            'Dim dtOrganizations As DataTable
            'Dim lID As String

            'MyLocation = Split(Common.WebRequest.QueryString("GPSCoordinates"), "-")

            'pSql = "Select OrgID from MobizTransDeviceOrganizationPairing where UUID = " & ReadyString(MyLocation(0)) & " AND IsMonitoring = 1"

            'dtOrganizations = Common.WebRequest.ProData.Execute(pSql)

            'lID = Guid.NewGuid.ToString

            'AppendToLog(Common.WebRequest.QueryString("GPSCoordinates"))

            'Try
            '    p("ID") = ReadyString(lID)
            '    p("UUID") = ReadyString(MyLocation(0))
            '    p("Lat") = ReadyString(MyLocation(1))
            '    p("Long") = ReadyString(MyLocation(2))

            '    p("RegisteredDate") = ReadyString(CDate(MyLocation(3).Replace("%20", " ")))
            '    p("Source") = ReadyString(MyLocation(4))

            '    If MyLocation.Length > 5 Then
            '        p("UTCTime") = ReadyString(MyLocation(5))
            '    End If

            '    pSql = p.GetInsert
            '    Common.WebRequest.ProData.ExecuteSQL(pSql)

            '    If dtOrganizations.Rows.Count > 0 Then
            '        For i As Integer = 0 To dtOrganizations.Rows.Count - 1
            '            pSql = "Insert into TransDeviceLocationOrganization(TransDeviceLocationID, OrgID) values(" & ReadyString(lID) & "," & ReadyString(dtOrganizations.Rows(i).Item("OrgID")) & ")"
            '            Common.WebRequest.ProData.ExecuteSQL(pSql)
            '        Next
            '    End If
            'Catch ex As Exception
            '    AppendToLog(ex.Message.ToString)
            'End Try
        End Function

#Region "OldGPSCode"

        '  <HttpPost()>
        Function getGPSTrackList() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim strID As String
            Dim clsLocation As New GPS
            Dim clsConfig As New DeviceMDMConfiguration
            Dim strReturn As String = ""
            Dim dt As DataTable
            Dim strIMEI As String
            Dim strUUID As String
            Dim strDateFrom As String
            Dim strDateTo As String

            clsConfig.DeviceID = WebRequest.Instance.Data("DeviceID")
            strDateFrom = WebRequest.Instance.Data("DateFrom")
            strDateTo = WebRequest.Instance.Data("DateTo")

            dt = clsConfig.GetDeviceCodesByDeviceID()
            If dt.Rows.Count > 0 Then
                strIMEI = dt.Rows(0).Item("IMEI").ToString()
                strUUID = dt.Rows(0).Item("UUID").ToString()

                If (Not ValidDeviceCode(strUUID)) Then
                    clsLocation.favorUUID = False
                    If (Not ValidDeviceCode(strIMEI)) Then 'if both uuid and imeid not available then error
                        Return ""
                    End If
                End If

                clsLocation.Uuid = strUUID
                clsLocation.Imei = strIMEI
                strReturn = JsonDictionary.Serialize("Track", clsLocation.GetLocationList(strDateFrom, strDateTo))
                Return strReturn

            End If
            Return strReturn
        End Function

        '  <HttpPost()>
        Function getGPSCurrentLocation() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim strID As String
            Dim clsLocation As New GPS
            Dim strReturn As String = ""
            Dim strDeviceID As String = ""
            Dim strIMEI As String = ""
            Dim strUUID As String = ""
            Dim clsConfig As New DeviceMDMConfiguration


            Dim dt As DataTable

            strDeviceID = Common.WebRequest.Data("DeviceID")
            clsConfig.DeviceID = strDeviceID
            dt = clsConfig.GetDeviceCodesByDeviceID()


            If dt.Rows.Count > 0 Then
                strIMEI = dt.Rows(0).Item("IMEI").ToString()
                strUUID = dt.Rows(0).Item("UUID").ToString()

                If (Not ValidDeviceCode(strUUID)) Then
                    clsLocation.favorUUID = False
                    If (Not ValidDeviceCode(strIMEI)) Then 'if both uuid and imeid not available then error
                        Return ""
                    End If
                End If

                clsLocation.Uuid = strUUID
                clsLocation.Imei = strIMEI
                strReturn = JsonDictionary.Serialize("Track", clsLocation.GetCurrentLocation())
                Return strReturn

            End If
        End Function

        '<HttpPost()>
        Function receiveLocationList() As Boolean

            Dim gpsElem As New GPS
            Dim locationList As String
            Dim strUUID As String = ""
            Dim strIMEI As String = ""
            Dim strUserID As String = ""
            Dim finalResult As String = ""
            Dim goodReturn = True
            Dim OrgList As List(Of String)

            Request.InputStream.Position = 0
            locationList = Common.WebRequest.Data("LocationList")

            If locationList.Length > 0 Then

            Else
                goodReturn = False
                Return goodReturn
            End If


            finalResult = locationList.Length.ToString()
            Dim js As New System.Web.Script.Serialization.JavaScriptSerializer
            Dim gpsAppList As List(Of GPSLocationList) = js.Deserialize(Of List(Of GPSLocationList))(locationList)



            If gpsAppList.Count > 0 Then
                strUUID = gpsAppList.Item(0).getuuid
                strIMEI = gpsAppList.Item(0).getimei
                strUserID = gpsAppList.Item(0).personid
                ' devDash.DataBaseName = getDatabaseByOrgID(gpsAppList.Item(0).OrgID)
                If (Not ValidDeviceCode(strUUID)) Then
                    gpsElem.favorUUID = False
                    If (Not ValidDeviceCode(strIMEI)) Then 'if both uuid and imeid not available then error
                        goodReturn = False
                        Return goodReturn
                    End If
                End If

                gpsElem.Uuid = strUUID
                gpsElem.Imei = strIMEI
                gpsElem.PersonID = strUserID
                OrgList = getDatabaseNameByUserID(strUserID)

                '     devDash.DeleteDeviceAppToAvoidDuplicate()

                For value As Integer = 0 To gpsAppList.Count - 1
                    gpsElem.LocationID = gpsAppList.Item(value).getlocationID
                    gpsElem.Longitude = gpsAppList.Item(value).getlongitude
                    gpsElem.Latitude = gpsAppList.Item(value).getlatitude
                    gpsElem.SortOrder = gpsAppList.Item(value).getsortOrder
                    gpsElem.Imei = gpsAppList.Item(value).getimei
                    gpsElem.Uuid = gpsAppList.Item(value).getuuid
                    gpsElem.DateCreated = gpsAppList.Item(value).getdateCreated
                    gpsElem.OrgID = gpsAppList.Item(value).getorgid
                    gpsElem.DeviceID = gpsAppList.Item(value).deviceid

                    '   For Each dbSource As String In OrgList 'as per sir, gps info should be saved in all Org db which the user is connected with
                    '    gpsElem.dbName = dbSource
                    gpsElem.SaveDeviceLocationDetails()
                    'Next
                Next
            End If

            goodReturn = True
            Return goodReturn
        End Function

        '  <HttpPost()>
        Function getDatabaseNameByUserID(ByVal userID As String) As List(Of String)
            Dim clsPeople As New People
            Dim strReturn As String
            Dim dbList As New List(Of String)
            Dim dbName As String
            ' strID = WebSession.Instance.PeopleUserID
            Dim orgdt As DataTable
            orgdt = clsPeople.getPeopleOrganizationList(userID)
            If (orgdt.Rows.Count > 0) Then
                For ctr As Integer = 0 To orgdt.Rows.Count - 1
                    dbName = getDatabaseByOrgID(orgdt.Rows(ctr).Item("OrganizationID").ToString)
                    If Not String.IsNullOrWhiteSpace(dbName) Then
                        dbList.Add(dbName)
                    End If
                Next
            End If
            Return dbList
        End Function


        Function ValidDeviceCode(ByVal strCode As String) As Boolean
            Dim intParseCode As Integer
            Dim sum As Integer = 0

            If strCode Is Nothing Then
                Return False
            End If

            If strCode.Length <= 0 Then
                Return False
            End If

            If Integer.TryParse(strCode, intParseCode) Then
                For Each ch As Char In strCode
                    Dim i As Integer
                    If Integer.TryParse(ch, i) Then
                        sum += i
                    End If
                Next

                If sum > 0 Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return True
            End If
        End Function

        ' <HttpPost()>
        Function getDatabaseByOrgID(ByVal strOrgID As String) As String
            Dim clsDevice As New DeviceMDMConfiguration
            clsDevice.OrgID = strOrgID
            Return clsDevice.GetDatabaseByOrgID()
        End Function


        Function getLatestDeviceLocationByDeviceID(ByVal DeviceID As String) As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim clsGPS As New GPS
            clsGPS.DeviceID = DeviceID
            Return JsonDictionary.Serialize("Results", clsGPS.GetLatestLocationByDeviceID)
        End Function

        Function SaveGPSSettings() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim clsGPS As New GPS

            Return JsonDictionary.Serialize("Results", clsGPS.SaveGPSSettings)
        End Function

        Function GetGPSSettings() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim clsGPS As New GPS

            Return JsonDictionary.Serialize("Results", clsGPS.GetGPSSettings)
        End Function

        ' <HttpPost()>
        Function getDeviceIDByUUIDIMEI(ByVal mUuid As String, ByVal mImei As String) As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim clsGPS As New GPS
            Dim deviceID As String
            '  If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            clsGPS.Uuid = mUuid
            clsGPS.Imei = mImei
            deviceID = clsGPS.GetDeviceIDByDeviceCodes()

            Return deviceID
        End Function

#End Region
    End Class
End Namespace