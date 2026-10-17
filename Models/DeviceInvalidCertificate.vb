Public Class DeviceInvalidCertificate

    Public Property ID As String = ""
    Public Property IsCertificateValid As String
    Public Property ReasonForInvalidation As String
    Public Property UUID As String
    Public Property IMEI As String
    Public Property UserFullName As String
    Public Property UserID As String
    Public Property DevicePolicyProfileID As String
    Public Property MaxReturnRow As Integer = 50
    Public Property favorUUID As Boolean = True




    Function SaveCertificateStatus(dbSource As String) As String
        Dim strIDReturn As String = ""
        If Not dbSource Is Nothing Then
            If dbSource.Length > 0 Then



                Common.WebRequest.ProData.ExecuteSQL(" Use " & dbSource)

                Dim p As New InsertUpdate("LibraryDeviceInvalidCertificate")
                Dim pSql As String


                Try
                    If IsCertificateValid.ToLower.Trim.Equals("true") Then
                        p("IsCertificateValid") = "'True'"
                    Else
                        p("IsCertificateValid") = "'False'"
                    End If
                    p("ReasonForInvalidation") = ReadyString(ReasonForInvalidation)
                    p("DateReported") = "getDate()"
                    p("DateLastUpdate") = "getDate()"
                    p("UUID") = ReadyString(UUID)
                    p("IMEI") = ReadyString(IMEI)
                    p("UserFullName") = ReadyString(UserFullName)
                    p("UserID") = IIf(UserID.Trim().Length <= 0, "NULL", ReadyString(UserID))
                    p("DevicePolicyProfileID") = IIf(DevicePolicyProfileID.Trim().Length <= 0, "NULL", ReadyString(DevicePolicyProfileID))
                    p("IsViewed") = "'False'"
                    p("IsActive") = "'True'"


                    If (ID.Trim().Length <= 0) Then
                        strIDReturn = Guid.NewGuid.ToString()
                        p("ID") = ReadyString(strIDReturn)
                        pSql = p.GetInsert()
                    Else
                        strIDReturn = ID
                        pSql = p.GetUpdate(" ID = " & ReadyString(ID))
                    End If

                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                    Common.WebRequest.ProData.CommitTransaction()
                    Return strIDReturn
                Catch ex As Exception
                    ' Common.WebRequest.ProData.RollbackTransaction()
                    Return strIDReturn
                End Try


            End If
        End If
        Return strIDReturn
    End Function

    Function GetCertificateLog(ByVal strKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "dev.ID"
                    .Searchable = False
                End With
                With .Add("IsCertificateValid", SqlDbType.Bit)
                    .Expression = "dev.IsCertificateValid"
                End With
                With .Add("ReasonForInvalidation", SqlDbType.NVarChar)
                    .Expression = "dev.ReasonForInvalidation"
                End With
                With .Add("DateReported", SqlDbType.DateTime)
                    .Expression = "dev.DateReported"
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "dev.IsActive"
                End With
                With .Add("UUID", SqlDbType.NVarChar)
                    .Expression = "dev.UUID"
                End With
                With .Add("IMEI", SqlDbType.NVarChar)
                    .Expression = "dev.IMEI"
                End With
                With .Add("UserFullName", SqlDbType.NVarChar)
                    .Expression = "dev.UserFullName"
                End With
                With .Add("OrgID", SqlDbType.UniqueIdentifier)
                    .Expression = "dev.OrgID"
                End With
                With .Add("OrgName", SqlDbType.NVarChar)
                    .Expression = "dev.OrgName"
                End With
                With .Add("DevicePolicyProfileID", SqlDbType.UniqueIdentifier)
                    .Expression = "dev.DevicePolicyProfileID"
                End With
                With .Add("IsViewed", SqlDbType.Bit)
                    .Expression = "dev.IsViewed"
                End With
                With .Add("DevicePolicyProfileName", SqlDbType.NVarChar)
                    .Expression = "dpp.DevicePolicyProfileName"
                End With
            End With

            .SqlFrom = "FROM LibraryDeviceInvalidCertificate dev LEFT JOIN LibraryDevicePolicyProfile dpp on dpp.DevicePolicyProfileID = dev.DevicePolicyProfileID  "
            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY dev.DateReported DESC"
            If favorUUID Then
                .SqlWhere = "WHERE UUID = " & ReadyString(UUID) & " AND IsCertificateValid = 'False'  "
            Else
                .SqlWhere = "WHERE IMEI = " & ReadyString(IMEI) & " AND IsCertificateValid = 'False'  "
            End If
            pDT = .Requery
        End With

        Return pDT

    End Function

    Function GetCertificateLogAll(ByVal strKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "dev.ID"
                    .Searchable = False
                End With
                With .Add("IsCertificateValid", SqlDbType.Bit)
                    .Expression = "dev.IsCertificateValid"
                End With
                With .Add("ReasonForInvalidation", SqlDbType.NVarChar)
                    .Expression = "dev.ReasonForInvalidation"
                End With
                With .Add("DateReported", SqlDbType.DateTime)
                    .Expression = "dev.DateReported"
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "dev.IsActive"
                End With
                With .Add("UUID", SqlDbType.NVarChar)
                    .Expression = "dev.UUID"
                End With
                With .Add("IMEI", SqlDbType.NVarChar)
                    .Expression = "dev.IMEI"
                End With
                With .Add("UserFullName", SqlDbType.NVarChar)
                    .Expression = "dev.UserFullName"
                End With
                With .Add("OrgID", SqlDbType.UniqueIdentifier)
                    .Expression = "dev.OrgID"
                End With
                With .Add("OrgName", SqlDbType.NVarChar)
                    .Expression = "dev.OrgName"
                End With
                With .Add("DevicePolicyProfileID", SqlDbType.UniqueIdentifier)
                    .Expression = "dev.DevicePolicyProfileID"
                End With
                With .Add("IsViewed", SqlDbType.Bit)
                    .Expression = "dev.IsViewed"
                End With
                With .Add("DevicePolicyProfileName", SqlDbType.NVarChar)
                    .Expression = "dpp.DevicePolicyProfileName"
                End With
            End With

            .SqlFrom = "FROM LibraryDeviceInvalidCertificate dev LEFT JOIN LibraryDevicePolicyProfile dpp on dpp.DevicePolicyProfileID = dev.DevicePolicyProfileID  "
            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY dev.DateReported DESC"
            If favorUUID Then
                .SqlWhere = "WHERE UUID = " & ReadyString(UUID)
            Else
                .SqlWhere = "WHERE IMEI = " & ReadyString(IMEI)
            End If
            pDT = .Requery
        End With

        Return pDT

    End Function







    Function DeviceCertificateInvalidate(dbSource As String) As String
        Common.WebRequest.ProData.ExecuteSQL(" Use " & dbSource)
        Dim pSql As String
        If favorUUID Then
            pSql = "Update LibraryDevice set IsCertificateValid=0 where UUID = " & ReadyString(UUID)
        Else
            pSql = "Update LibraryDevice set IsCertificateValid=0 where IMEI = " & ReadyString(IMEI)
        End If
        Common.WebRequest.ProData.Execute(pSql)
    End Function



    Function DeviceCertificateValidate(dbSource As String) As String
        Common.WebRequest.ProData.ExecuteSQL(" Use " & dbSource)
        Dim pSql As String
        If favorUUID Then
            pSql = "Update LibraryDevice set IsCertificateValid=1 where UUID = " & ReadyString(UUID)
        Else
            pSql = "Update LibraryDevice set IsCertificateValid=1 where IMEI = " & ReadyString(IMEI)
        End If
        Common.WebRequest.ProData.Execute(pSql)
    End Function





End Class
