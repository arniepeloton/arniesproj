Namespace WebProj
    Public Class DeviceDashboardController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /DeviceDashboard

        Function Index() As ActionResult
            'If WebRequest.Instance.HandleAccess = False Then Exit Function
            'If (Not SecurityRole.Instance.HandleRoleAccess("Mobile Device Management Dashboard")) Then Exit Function

            Return View()
        End Function


        Function getDeviceSearchResult() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim clsRecord As New DeviceMDMConfiguration
            Dim keyword As String
            keyword = WebRequest.Instance.Data("Keywords")
            'Return DataTableToJSON.GetJson(clsRecord.GetSearchResult(Keywords))
            Return JsonDictionary.Serialize("DashboardSearchResults", clsRecord.GetDeviceListBySearch(keyword))

        End Function

        Function getDeviceList() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim clsRecord As New DeviceMDMConfiguration

            Return JsonDictionary.Serialize("DashboardSearchResults", clsRecord.GetDeviceList())
        End Function


        Function getDeviceListByID() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim clsRecord As New DeviceMDMConfiguration

            clsRecord.DeviceID = WebRequest.Instance.Data("ID")
            Return JsonDictionary.Serialize("Results", clsRecord.GetDeviceListByID())
        End Function



      
        Function getDeviceAppsList() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim clsRecord As New DeviceDashboard
            Dim clsConfig As New DeviceMDMConfiguration
            Dim strReturn As String = ""
            Dim strIMEI As String = ""
            Dim strUUID As String = ""
            Dim dt As DataTable
            'i need to get uuid or imei so i have to use device id 
            clsConfig.DeviceID = WebRequest.Instance.Data("ID")
            dt = clsConfig.GetDeviceCodesByDeviceID()
            If dt.Rows.Count > 0 Then

                strIMEI = dt.Rows(0).Item("IMEI").ToString()
                strUUID = dt.Rows(0).Item("UUID").ToString()

            End If
           
            If (Not ValidDeviceCode(strUUID)) Then
                clsRecord.favorUUID = False
                If (Not ValidDeviceCode(strIMEI)) Then 'if both uuid and imeid not available then error
                    Return strReturn
                End If
            End If

            Return JsonDictionary.Serialize("DeviceAppsList", clsRecord.GetDeviceApplicationInfoByDeviceID(strUUID, strIMEI))
        End Function


        Function getDeviceAppsListByID() As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim clsRecord As New DeviceDashboard
            clsRecord.ID = WebRequest.Instance.Data("ID")

            Return JsonDictionary.Serialize("DeviceAppsList", clsRecord.GetDeviceApplicationInfoByID())
        End Function


        Function checkApplicationIfExistByPolicyProfileID() As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim IsExist As Boolean
            Dim clsRecord As New DeviceAppsWhiteList
            Dim strReturn As String = ""
            clsRecord.DevicePolicyProfileID = WebRequest.Instance.Data("DevicePolicyProfileID")
            clsRecord.ApplicationName = WebRequest.Instance.Data("ApplicationName")
            clsRecord.ApplicationLabel = WebRequest.Instance.Data("ApplicationLabel")

            If clsRecord.CheckApplicationIfExistByPolicyProfileID().Rows.Count > 0 Then
                IsExist = True
            Else
                IsExist = False
            End If

            strReturn = JsonDictionary.Serialize("Results", IsExist)
            Return strReturn
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




        Function sendWipeOutSignal() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim clsConfig As New DeviceMDMConfiguration
            Dim strDeviceID = WebRequest.Instance.Data("DeviceID")
            If (Not String.IsNullOrWhiteSpace(strDeviceID)) Then
                clsConfig.DeviceID = strDeviceID
                clsConfig.SendWipeOutSignal()
            End If

        End Function
    End Class
End Namespace