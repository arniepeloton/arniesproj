<Serializable()> _
Public Class DeviceAndroidApps
    Public ApplicationDescription As [String]
    Public ApplicationInfo As [String]
    Public ApplicationLabel As [String]
    Public ApplicationName As [String]
    Public Directory As [String]
    Public UID As [String]
    Public DeviceUUID As [String]
    Public OrgID As [String]
    Public DeviceIMEI As [String]

    Public Sub New()
    End Sub

    Public Function getApplicationName() As [String]
        Return ApplicationName
    End Function
    Public Sub setApplicationName(_ApplicationName As [String])
        Me.ApplicationName = _ApplicationName
    End Sub


    Public Function getDirectory() As [String]
        Return Directory
    End Function
    Public Sub setDirectory(_Directory As [String])
        Me.Directory = _Directory
    End Sub



    Public Function getApplicationInfo() As [String]
        Return ApplicationInfo
    End Function
    Public Sub setApplicationInfo(_ApplicationInfo As [String])
        Me.ApplicationInfo = _ApplicationInfo
    End Sub

    Public Function getUID() As [String]
        Return UID
    End Function
    Public Sub setUID(_UID As [String])
        Me.UID = _UID
    End Sub

    Public Function getDeviceUUID() As [String]
        Return DeviceUUID
    End Function
    Public Sub setDeviceUUID(_DeviceUUID As [String])
        Me.DeviceUUID = _DeviceUUID
    End Sub


    Public Function getOrgID() As [String]
        Return OrgID
    End Function
    Public Sub setOrgID(_OrgID As [String])
        Me.OrgID = _OrgID
    End Sub


    Public Function getDeviceIMEI() As [String]
        Return DeviceIMEI
    End Function
    Public Sub setDeviceIMEI(_DeviceIMEI As [String])
        Me.DeviceIMEI = _DeviceIMEI
    End Sub


End Class

