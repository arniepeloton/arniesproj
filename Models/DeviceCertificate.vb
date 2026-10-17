<Serializable()> _
Public Class DeviceCertificate
    Public IsCertificateValid As [String]
    Public ReasonForInvalidation As [String]
    Public UUID As [String]
    Public IMEI As [String]
    Public UserFullName As [String]
    Public UserID As [String]
    Public OrgID As [String] = ""
    Public DevicePolicyProfileID As [String]

    Public Sub New()
    End Sub

    Public Function getIsCertificateValid() As [String]
        Return IsCertificateValid
    End Function
    Public Sub setIsCertificateValid(_IsCertificateValid As [String])
        Me.IsCertificateValid = _IsCertificateValid
    End Sub


    Public Function getReasonForInvalidation() As [String]
        Return ReasonForInvalidation
    End Function
    Public Sub setReasonForInvalidation(_ReasonForInvalidation As [String])
        Me.ReasonForInvalidation = _ReasonForInvalidation
    End Sub



    Public Function getUUID() As [String]
        Return UUID
    End Function
    Public Sub setUUID(_UUID As [String])
        Me.UUID = _UUID
    End Sub


    Public Function getIMEI() As [String]
        Return IMEI
    End Function
    Public Sub setIMEI(_IMEI As [String])
        Me.IMEI = _IMEI
    End Sub




    Public Function getUserFullName() As [String]
        Return UserFullName
    End Function
    Public Sub setUserFullName(_UserFullName As [String])
        Me.UserFullName = _UserFullName
    End Sub



    Public Function getUserID() As [String]
        Return UserID
    End Function
    Public Sub setUserID(_UserID As [String])
        Me.UserID = _UserID
    End Sub



    Public Function getOrgID() As [String]
        Return OrgID
    End Function
    Public Sub setOrgID(_OrgID As [String])
        Me.OrgID = _OrgID
    End Sub



    Public Function getDevicePolicyProfileID() As [String]
        Return DevicePolicyProfileID
    End Function
    Public Sub setDevicePolicyProfileID(_DevicePolicyProfileID As [String])
        Me.DevicePolicyProfileID = _DevicePolicyProfileID
    End Sub

End Class

