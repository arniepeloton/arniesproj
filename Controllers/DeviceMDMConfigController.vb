Imports System.Security.Cryptography.RijndaelManaged
Imports System.Security.Cryptography




Namespace WebProj
    Public Class DeviceMDMConfigController
        Inherits System.Web.Mvc.Controller


        '
        ' GET: /DeviceMDMConfig


        Function Index() As ActionResult
            'If WebRequest.Instance.HandleAccess = False Then Exit Function
            'If (Not SecurityRole.Instance.HandleRoleAccess("Mobile Device Management Policy Settings")) Then Exit Function

            Return View()
        End Function



        ' <HttpPost()>
        Function getDetails() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim jd As New JsonDictionary
            Dim requestParam As String = ""
            Dim strUuid As String
            Dim strImei As String
            Dim s As New DeviceMDMConfiguration

            strUuid = Common.WebRequest.QueryString("UUID")
            strImei = Common.WebRequest.QueryString("IMEI")

            If (Not ValidDeviceCode(strUuid)) Then
                s.favorUUID = False
            Else
                s.favorUUID = True
            End If
            s.UUID = strUuid
            s.IMEI = strImei


            requestParam = getDevicePolicyProfileIDByDeviceUUID()

            s.DevicePolicyProfileID = requestParam

            If requestParam.Length > 0 Then
                jd.Add("DashboardList", s.GetDetailsMDM())
                jd.Add("PolicyProfileLastUpdate", s.GetPolicyProfilesLatestUpdate)
                jd.Add("DeviceLastUpdate", s.GetDevicePolicyLastUpdate)
                jd.Add("WhiteListedApps", s.GetBlockedAppsList)
                jd.Add("PerformWipeOut", s.PerformWipeOut)

                s.DeactivateWipeOutSignal()
            Else
                jd.Add("DashboardList", "NoPolicy")
            End If
            Return jd.Serialize()
        End Function



        ' <HttpPost()>
        Function getPolicyMostStrict() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim jd As New JsonDictionary
            Dim finalResult As String = ""
            Dim strUuid As String
            Dim strImei As String
            Dim s As New DeviceMDMConfiguration


            strUuid = Common.WebRequest.QueryString("UUID")
            strImei = Common.WebRequest.QueryString("IMEI")

            If (Not ValidDeviceCode(strUuid)) Then 'use imei 
                s.favorUUID = False
                s.IMEI = strImei
                If (Not ValidDeviceCode(strImei)) Then 'if both uuid and imeid not available then error
                    jd.Add("DashboardList", "NoPolicy")
                    Return jd.Serialize()
                End If
            Else 'use uuid
                s.favorUUID = True
                s.UUID = strUuid
            End If


            Dim dtResult As DataTable = s.GetDetailsMDM_MostStrict()
            If dtResult.Rows.Count > 0 Then
                jd.Add("DashboardList", dtResult)
                jd.Add("PolicyProfileLastUpdate", s.GetPolicyProfilesLatestUpdate_MostStrict)
                jd.Add("DeviceLastUpdate", s.GetDevicePolicyLastUpdate)
                finalResult = jd.Serialize()
                Return finalResult
            Else
                jd.Add("DashboardList", "NoPolicy")
                Return jd.Serialize()
            End If

        End Function


        ' <HttpPost()>
        'Function determinePolicyDownload() As String
        '    If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        '    Dim s As New DeviceMDMConfiguration
        '    Dim jd As New JsonDictionary

        '    s.UUID = Common.WebRequest.QueryString("UUID")
        '    s.DevicePolicyProfileID = "c4aea38f-9fd3-44e8-8511-795e8f2bfc0b"

        '    jd.Add("key1", s.GetDevicePolicyProfileIDByDeviceUUID())
        '    jd.Add("key2", s.GetPolicyItemsByProfileID())

        '    Return jd.Serialize()

        'End Function





        ' <HttpPost()>
        Function getDevicePolicyProfileIDByDeviceUUID() As String
            Dim result = ""
            Dim s As New DeviceMDMConfiguration
            Dim strUuid As String
            Dim strImei As String

            strUuid = Common.WebRequest.QueryString("UUID")
            strImei = Common.WebRequest.QueryString("IMEI")


            If (Not ValidDeviceCode(strUuid)) Then 'use imei 
                s.favorUUID = False
                s.IMEI = strImei
                If (Not ValidDeviceCode(strImei)) Then 'if both uuid and imeid not available then error
                    result = ""
                    Return result
                End If
            Else 'use uuid
                s.favorUUID = True
                s.UUID = strUuid
            End If

            result = s.GetDevicePolicyProfileIDByDeviceUUID()
            Return result
        End Function




        ' <HttpPost()>
        Function CheckPeopleUserAccount() As String
            Dim clsPeople As New DeviceMDMConfiguration
            Dim dtaTable As New DataTable
            Dim intCntr As Integer
            Dim pOut As JsonDictionary

            Dim pUserName As String
            Dim pPass As String
            Dim FullName As String = ""


            '  clsPeople.UserName = Common.WebRequest.Data("Userame")
            ' clsPeople.UserPassword = Common.WebRequest.Data("Userpassword")
            '  clsPeople.UserName = Common.WebRequest.QueryString("Username")
            ' clsPeople.UserPassword = Common.WebRequest.QueryString("Userpassword



            pUserName = ConvertSecuredRequest(Common.WebRequest.QueryString("Username"))
            pPass = ConvertSecuredRequest(Common.WebRequest.QueryString("Userpassword"))
            clsPeople.UserName = pUserName
            clsPeople.UserPassword = pPass
            dtaTable = clsPeople.CheckPeopleUserAccount()
            pOut = New JsonDictionary
            If (dtaTable.Rows.Count > 0) Then
                intCntr = 0
                Do Until (intCntr >= dtaTable.Rows.Count)

                    If (ProPack.Security.Hash.Verify(clsPeople.UserPassword, dtaTable.Rows(intCntr).Item("UserPassword").ToString())) Then
                        clsPeople.PeopleID = dtaTable.Rows(intCntr).Item("PeopleID").ToString()
                        WebSession.Instance.UserID = dtaTable.Rows(intCntr).Item("PeopleID").ToString()
                        WebSession.Instance.PeopleUserID = dtaTable.Rows(intCntr).Item("PeopleID").ToString()
                        WebSession.Instance.FirstName = dtaTable.Rows(intCntr).Item("FirstName").ToString()
                        WebSession.Instance.FullName = dtaTable.Rows(intCntr).Item("PeopleName").ToString()
                        FullName = dtaTable.Rows(intCntr).Item("PeopleName").ToString()
                        If CDec2(Common.WebRequest.Data("IsCookieless")) <> 0 Then
                            pOut.Add("ASP.NET_SessionId", HttpContext.Session.SessionID)
                            '// the ff commented code dont work
                            'For Each pCookieName As String In Response.Cookies
                            '    pCookie = Response.Cookies(pCookieName)
                            '    pOut.Add(pCookie.Name, pCookie.Value)
                            'Next pCookieName
                        End If

                        If CStr2(Common.WebRequest.Data("LoginHash")).Length = 50 Then

                        End If
                        Exit Do
                    End If

                    intCntr = intCntr + 1
                Loop

            End If

            pOut.Add("PeopleUserAccountData", clsPeople.PeopleID)
            pOut.Add("FullName", FullName)
            Return pOut.Serialize

        End Function



        ' <HttpPost()>
        Function ConvertSecuredRequest(ByVal input As String) As String
            Dim converResult As String = Server.UrlDecode(input).ToString()
            Return Decrypt(converResult, "J6K5D5T5")
        End Function

        'Public Function RetrievePassedUrl(String me) As String
        '    Dim returnUrl = Server.UrlDecode()
        '    Return returnUrl
        'End Function


        Public Function GetRijndaelManaged(secretKey As [String]) As RijndaelManaged
            Dim keyBytes = New Byte(15) {}
            Dim secretKeyBytes = Encoding.UTF8.GetBytes(secretKey)
            Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length))

            Dim rmg As New RijndaelManaged()
            rmg.Mode = CipherMode.CBC
            rmg.Padding = PaddingMode.PKCS7
            rmg.KeySize = 128
            rmg.BlockSize = 128
            rmg.Key = keyBytes
            rmg.IV = keyBytes
            Return rmg
        End Function

        Public Function Encrypt(plainBytes As Byte(), rijndaelManaged As RijndaelManaged) As Byte()
            Return rijndaelManaged.CreateEncryptor().TransformFinalBlock(plainBytes, 0, plainBytes.Length)
        End Function

        Public Function Decrypt(encryptedData As Byte(), rijndaelManaged As RijndaelManaged) As Byte()
            Return rijndaelManaged.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length)
        End Function

        ''' <summary>
        ''' Encrypts plaintext using AES 128bit key and a Chain Block Cipher and returns a base64 encoded string
        ''' </summary>
        ''' <param name="plainText">Plain text to encrypt</param>
        ''' <param name="key">Secret key</param>
        ''' <returns>Base64 encoded string</returns>
        Public Function Encrypt(plainText As [String], key As [String]) As [String]
            Dim plainBytes = Encoding.UTF8.GetBytes(plainText)
            Return Convert.ToBase64String(Encrypt(plainBytes, GetRijndaelManaged(key)))
        End Function

        ''' <summary>
        ''' Decrypts a base64 encoded string using the given key (AES 128bit key and a Chain Block Cipher)
        ''' </summary>
        ''' <param name="encryptedText">Base64 Encoded String</param>
        ''' <param name="key">Secret Key</param>
        ''' <returns>Decrypted String</returns>
        Public Function Decrypt(encryptedText As [String], key As [String]) As [String]
            Dim encryptedBytes = Convert.FromBase64String(encryptedText)
            Return Encoding.UTF8.GetString(Decrypt(encryptedBytes, GetRijndaelManaged(key)))
        End Function




        ' <HttpPost()>
        Function getPolicyDetailsByProfileID() As String
            WebRequest.Instance.HandleOrganization(True)
            Dim s As New DeviceMDMConfiguration
            WebRequest.Instance.HandleOrganization(True) '= False Then Return WebRequest.InvalidOrganization
            s.DevicePolicyProfileID = WebRequest.Instance.Data("ID")
            Return JsonDictionary.Serialize("Results", s.GetPolicyDetailsByProfileID())
        End Function





        ' <HttpPost()>
        Function getPolicyItemsByPolicyProfileID() As String
            WebRequest.Instance.HandleOrganization(True)
            Dim s As New DeviceMDMConfiguration
            WebRequest.Instance.HandleOrganization(True) '= False Then Return WebRequest.InvalidOrganization
            s.DevicePolicyProfileID = WebRequest.Instance.Data("ID")
            Return JsonDictionary.Serialize("Results", s.GetPolicyItemsByProfileID())
        End Function

        Function savePolicyProfile() As String

            Dim s As New DeviceMDMConfiguration
            WebRequest.Instance.HandleOrganization(True) '= False Then Return WebRequest.InvalidOrganization

            s.DevicePolicyProfileID = WebRequest.Instance.Data("DevicePolicyProfileID")
            s.DevicePolicyDescription = WebRequest.Instance.Data("DevicePolicyDescription")
            s.DevicePolicyProfileName = WebRequest.Instance.Data("DevicePolicyProfileName")
            s.IsActive = WebRequest.Instance.Data("IsActive")
            Return JsonDictionary.Serialize("Results", s.SavePolicyProfile)

        End Function



        'Function SavePolicyItem() As String

        '    Dim s As New DeviceMDMConfiguration
        '    WebRequest.Instance.HandleOrganization(True) '= False Then Return WebRequest.InvalidOrganization
        '    s.ID = WebRequest.Instance.Data("ID")
        '    s.ConfigCode = WebRequest.Instance.Data("ConfigCode")
        '    s.DevicePolicyProfileID = WebRequest.Instance.Data("DevicePolicyProfileID")
        '    s.ConfigValue = WebRequest.Instance.Data("ConfigValue")
        '    s.DevicePolicyProfileID = WebRequest.Instance.Data("DevicePolicyProfileID")

        '    s.ID = ""
        '    Return JsonDictionary.Serialize("Results", s.SavePolicyItem)

        'End Function


        'Function savePolicyItems() As String
        '    Dim s As New DeviceMDMConfiguration
        '    WebRequest.Instance.HandleOrganization(True) '= False Then Return WebRequest.InvalidOrganization

        '    Return JsonDictionary.Serialize("Results", s.SavePolicyItem())
        'End Function




        ' <HttpPost()>
        Function updatePolicies() As String

            Dim clsDeviceConfig As New DeviceMDMConfiguration
            WebRequest.Instance.HandleOrganization(True)

            clsDeviceConfig.ConfigCode = WebRequest.Instance.Data("ConfigCode")
            clsDeviceConfig.DevicePolicyProfileID = WebRequest.Instance.Data("DevicePolicyProfileID")
            clsDeviceConfig.ConfigValue = WebRequest.Instance.Data("ConfigValue")
            Return clsDeviceConfig.SavePolicyItem()
        End Function


        ' <HttpPost()>
        Function updatePolicies_MostStrict() As String
            WebRequest.Instance.HandleOrganization(True)
            Dim dtConfig As DataTable
            Dim strMostStrictPolicyID As String = ""
            Dim clsDeviceConfig As New DeviceMDMConfiguration
            Dim OrgID As String = ""

            OrgID = Common.WebRequest.QueryString("OrgID")
            clsDeviceConfig.DevicePolicyProfileID = WebRequest.Instance.Data("DevicePolicyProfileID")
            dtConfig = clsDeviceConfig.GetDeviceCodesByDevicePolicyProfileID() 'get the uuid, imei and device id 
            If dtConfig.Rows.Count > 0 Then
                clsDeviceConfig.UUID = dtConfig.Rows(0).Item("UUID").ToString()
                clsDeviceConfig.IMEI = dtConfig.Rows(0).Item("IMEI").ToString()
                clsDeviceConfig.DeviceID = dtConfig.Rows(0).Item("DEVICEID").ToString()
                clsDeviceConfig.ConfigCode = WebRequest.Instance.Data("ConfigCode")
                clsDeviceConfig.ConfigValue = WebRequest.Instance.Data("ConfigValue")
                clsDeviceConfig.OrgID = OrgID
                strMostStrictPolicyID = clsDeviceConfig.CheckIfMostStrictPolicyExist
                If strMostStrictPolicyID.Length > 0 Then
                    clsDeviceConfig.DevicePolicyMostStrictID = strMostStrictPolicyID
                End If
                Return clsDeviceConfig.SavePolicyItem_MostStrict()
            End If
            Return ""
        End Function







        '  <HttpPost()>
        Function getPeopleOrganizationList() As String
            Dim strID As String
            Dim clsPeople As New People
            Dim strReturn As String

            strID = Common.WebRequest.QueryString("UserID")
            ' strID = WebSession.Instance.PeopleUserID
            strReturn = JsonDictionary.Serialize("PeopleData", clsPeople.getPeopleOrganizationList(strID))

            Return strReturn

        End Function





        ' <HttpPost()>
        Function CheckDeviceUniqueIMEI() As String
            Dim clsDeviceConfig As New DeviceMDMConfiguration
            Dim strReturn As String
            Dim withRet As Boolean
            Dim strUUID As String = ""
            Dim strIMEID As String

            '  If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strUUID = Common.WebRequest.QueryString("Uuid")
            strIMEID = Common.WebRequest.QueryString("Imei")


            If (Not ValidDeviceCode(strUUID)) Then
                clsDeviceConfig.favorUUID = False
                If (Not ValidDeviceCode(strIMEID)) Then 'if both uuid and imeid not available then error
                    strReturn = JsonDictionary.Serialize("IsRegistered", "invaliddc")
                    Return strReturn
                End If
            End If


            clsDeviceConfig.UUID = strUUID
            clsDeviceConfig.PhoneName = Common.WebRequest.QueryString("Phone")
            clsDeviceConfig.IMEI = strIMEID
            clsDeviceConfig.OS = Common.WebRequest.QueryString("Os")
            clsDeviceConfig.PhoneModel = Common.WebRequest.QueryString("Phonetype")
            clsDeviceConfig.OrgID = Common.WebRequest.QueryString("OrgID")
            withRet = clsDeviceConfig.CheckDeviceUniqueIDRegistered()


            If withRet Then
                strReturn = JsonDictionary.Serialize("IsRegistered", "registered")
            Else

                If clsDeviceConfig.IsPendingRegDeviceByID Then
                    strReturn = JsonDictionary.Serialize("IsRegistered", "pending")
                Else
                    RegisterDevice()
                    strReturn = JsonDictionary.Serialize("IsRegistered", "request")
                End If
            End If
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



        ' <HttpPost()>
        Function RegisterDevice() As String
            Dim clsDevice As New DeviceMDMConfiguration
            Dim strReturn As String

            '  If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDevice.ID = ""
            clsDevice.PhoneName = Common.WebRequest.QueryString("Phone")
            clsDevice.PhoneModel = Common.WebRequest.QueryString("Phonetype")
            clsDevice.UUID = Common.WebRequest.QueryString("Uuid")
            clsDevice.IMEI = Common.WebRequest.QueryString("Imei")
            clsDevice.OS = Common.WebRequest.QueryString("Os")

            WebSession.Instance.DeviceID = clsDevice.SaveDeviceInfo()
            strReturn = JsonDictionary.Serialize("DeviceData", WebSession.Instance.DeviceID)

            Return strReturn

        End Function




        ' <HttpPost()>
        Function getDatabaseByOrgID(ByVal strOrgID) As String
                Dim clsDevice As New DeviceMDMConfiguration
                clsDevice.OrgID = strOrgID
                Return clsDevice.GetDatabaseByOrgID()
        End Function





        '<HttpPost()>
        Function receiveAppList() As Boolean

            Dim devDash As New DeviceDashboard()
            Dim appList As String
            Dim strUUID As String = ""
            Dim strIMEI As String = ""
            Dim finalResult As String = ""
            Dim errorReturn = False
            Request.InputStream.Position = 0
            appList = Common.WebRequest.Data("AppList")


            finalResult = appList.Length.ToString()

            Dim js As New System.Web.Script.Serialization.JavaScriptSerializer
            Dim deviceAppList As List(Of DeviceAndroidApps) = js.Deserialize(Of List(Of DeviceAndroidApps))(appList)

            If deviceAppList.Count > 0 Then
                strUUID = deviceAppList.Item(0).DeviceUUID
                strIMEI = deviceAppList.Item(0).DeviceIMEI
                devDash.DataBaseName = getDatabaseByOrgID(deviceAppList.Item(0).OrgID)
                If (Not ValidDeviceCode(strUUID)) Then
                    devDash.favorUUID = False
                    If (Not ValidDeviceCode(strIMEI)) Then 'if both uuid and imeid not available then error
                        errorReturn = True
                        Return errorReturn
                    End If
                End If


                devDash.DeviceUUID = strUUID
                devDash.DeviceIMEI = strIMEI
                devDash.DeleteDeviceAppToAvoidDuplicate()

                For value As Integer = 0 To deviceAppList.Count - 1
                    devDash.ApplicationName = deviceAppList.Item(value).getApplicationName
                    devDash.ApplicationDescription = deviceAppList.Item(value).ApplicationDescription
                    devDash.ApplicationInfo = deviceAppList.Item(value).ApplicationInfo
                    devDash.ApplicationLabel = deviceAppList.Item(value).ApplicationLabel
                    devDash.UID = deviceAppList.Item(value).UID
                    devDash.Directory = deviceAppList.Item(value).Directory
                    devDash.SaveDeviceApplicationDetails()

                Next
            End If

            errorReturn = False
            Return errorReturn
        End Function




        <HttpPost()>
        Function updateDevicePolicyAppointment()
            Dim s As New DeviceMDMConfiguration
            s.ID = Common.WebRequest.Data("DeviceID") 'deviceid
            s.DevicePolicyProfileID = Common.WebRequest.Data("DevicePolicyProfileID")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            s.UpdateDevicePolicyAppointment()
        End Function


        <HttpPost()>
        Function getApplicationWhiteListByPolicyProfileID()
            Dim strReturn As String
            Dim s As New DeviceAppsWhiteList
            s.DevicePolicyProfileID = Common.WebRequest.Data("DevicePolicyProfileID")

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            strReturn = JsonDictionary.Serialize("Results", s.GetApplicationWhiteListByPolicyProfileID())

            Return strReturn
        End Function


        <HttpPost()>
        Function checkApplicationIfExistByPolicyProfileID()
            Dim strReturn As String
            Dim IsExist As Boolean
            Dim s As New DeviceAppsWhiteList
            s.DevicePolicyProfileID = Common.WebRequest.Data("DevicePolicyProfileID")
            s.ApplicationLabel = Common.WebRequest.Data("ApplicationLabel")
            s.ApplicationName = Common.WebRequest.Data("ApplicationName")

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If s.CheckApplicationIfExistByPolicyProfileID().Rows.Count > 0 Then
                IsExist = True
            Else
                IsExist = False
            End If

            strReturn = JsonDictionary.Serialize("Results", IsExist)
            Return strReturn
        End Function




        <HttpPost()>
        Function searchPolicyProfileByKeyword()
            Dim strReturn As String
            Dim strkeyword As String

            strkeyword = Common.WebRequest.Data("KeyWord")
            Dim s As New DeviceMDMConfiguration

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            strReturn = JsonDictionary.Serialize("Results", s.GetPolicyDetailsByProfileIDWithKeyWords(strkeyword))

            Return strReturn
        End Function



        ' <HttpPost()>
        Function getDatabaseByOrgID(ByVal strOrgID As String) As String
            Dim clsDevice As New DeviceMDMConfiguration
            clsDevice.OrgID = strOrgID
            Return clsDevice.GetDatabaseByOrgID()
        End Function


      





        '<HttpPost()>
        Function receiveCertificateValidityStatus() As String

            Dim devCert As New DeviceInvalidCertificate()
            Dim dbSource As String
            Dim cert As String
            Request.InputStream.Position = 0
            cert = Common.WebRequest.Data("Certificate")


            Dim js As New System.Web.Script.Serialization.JavaScriptSerializer
            Dim mCert As DeviceCertificate = js.Deserialize(Of DeviceCertificate)(cert)
            Dim strOrgID As String



            If Not (mCert Is Nothing) Then
                devCert.DevicePolicyProfileID = mCert.getDevicePolicyProfileID()
                devCert.IMEI = mCert.getIMEI
                devCert.UUID = mCert.getUUID
                devCert.IsCertificateValid = mCert.getIsCertificateValid()
                devCert.ReasonForInvalidation = mCert.getReasonForInvalidation()
                devCert.UserFullName = mCert.getUserFullName
                devCert.UserID = mCert.getUserID
            End If

            If (Not ValidDeviceCode(devCert.UUID)) Then
                devCert.favorUUID = False
                If (Not ValidDeviceCode(devCert.IMEI)) Then 'if both uuid and imeid not available then error
                    Return ""
                End If
            End If


            strOrgID = mCert.getOrgID


            If Not String.IsNullOrWhiteSpace(strOrgID) Then
                dbSource = getDatabaseByOrgID(strOrgID)
                If devCert.IsCertificateValid.ToLower.Trim.Equals("true") Then
                    devCert.DeviceCertificateValidate(dbSource)
                Else
                    devCert.DeviceCertificateInvalidate(dbSource)
                End If

                devCert.SaveCertificateStatus(dbSource)
            End If


            Return ""
        End Function




        <HttpPost()>
        Function getCertificateLog() As String
            Dim strReturn As String
            Dim s As New DeviceInvalidCertificate
            Dim clsConfig As New DeviceMDMConfiguration
            Dim keyword = ""
            Dim strUUID As String
            Dim strIMEI As String
            Dim strDeviceID As String
           
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            keyword = Common.WebRequest.Data("KeyWord")
            strDeviceID = Common.WebRequest.Data("DeviceID")



            Dim dt As DataTable
            clsConfig.DeviceID = strDeviceID
            dt = clsConfig.GetDeviceCodesByDeviceID()

            If dt.Rows.Count > 0 Then
                strIMEI = dt.Rows(0).Item("IMEI").ToString()
                strUUID = dt.Rows(0).Item("UUID").ToString()

                If (Not ValidDeviceCode(strUUID)) Then
                    s.favorUUID = False
                    If (Not ValidDeviceCode(strIMEI)) Then 'if both uuid and imeid not available then error
                        Return ""
                    End If
                End If

                s.UUID = strUUID
                s.IMEI = strIMEI

                strReturn = JsonDictionary.Serialize("Results", s.GetCertificateLog(keyword))
                Return strReturn
            End If
            Return ""
        End Function



        <HttpPost()>
        Function getCertificateLogAll() As String
            Dim strReturn As String
            Dim s As New DeviceInvalidCertificate
            Dim clsConfig As New DeviceMDMConfiguration
            Dim keyword = ""
            Dim strUUID As String
            Dim strIMEI As String
            Dim strDeviceID As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            keyword = Common.WebRequest.Data("KeyWord")
            strDeviceID = Common.WebRequest.Data("DeviceID")



            Dim dt As DataTable
            clsConfig.DeviceID = strDeviceID
            dt = clsConfig.GetDeviceCodesByDeviceID()

            If dt.Rows.Count > 0 Then
                strIMEI = dt.Rows(0).Item("IMEI").ToString()
                strUUID = dt.Rows(0).Item("UUID").ToString()

                If (Not ValidDeviceCode(strUUID)) Then
                    s.favorUUID = False
                    If (Not ValidDeviceCode(strIMEI)) Then 'if both uuid and imeid not available then error
                        Return ""
                    End If
                End If

                s.UUID = strUUID
                s.IMEI = strIMEI

                strReturn = JsonDictionary.Serialize("Results", s.GetCertificateLogAll(keyword))
                Return strReturn
            End If
            Return ""
        End Function





       
    End Class
End Namespace