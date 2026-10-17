<Serializable()> _
Public Class GPSLocationList

    Public dateCreated As [String]
    Public imei As [String] = ""
    Public uuid As [String] = ""
    Public locationID As [String] = ""
    Public longitude As Double = 0
    Public sortOrder As Integer = 0
    Public latitude As Double = 0
    Public orgid As [String] = ""
    Public personid As [String] = ""
    Public deviceid As [String] = ""

    Public Sub New()

    End Sub

    Public Function getlocationID() As [String]
        Return locationID
    End Function

    Public Sub setlocation(_locationID As [String])
        Me.locationID = _locationID
    End Sub




    Public Function getlongitude() As Double
        Return longitude
    End Function

    Public Sub setlongitude(_longitude As Double)
        Me.longitude = _longitude
    End Sub




    Public Function getlatitude() As Double
        Return latitude
    End Function

    Public Sub setlatitude(_latitude As Double)
        Me.latitude = _latitude
    End Sub





    Public Function getsortOrder() As Integer
        Return sortOrder
    End Function

    Public Sub setsortOrder(_sortOrder As Integer)
        Me.sortOrder = _sortOrder
    End Sub





    Public Function getdateCreated() As [String]
        Return dateCreated
    End Function

    Public Sub setdateCreated(_dateCreated As [String])
        Me.dateCreated = _dateCreated
    End Sub



    Public Function getuuid() As [String]
        Return uuid
    End Function

    Public Sub setuuid(_uuid As [String])
        Me.uuid = _uuid
    End Sub




    Public Function getimei() As [String]
        Return imei
    End Function

    Public Sub setimei(_imei As [String])
        Me.imei = _imei
    End Sub


    Public Function getorgid() As [String]
        Return orgid
    End Function

    Public Sub setorgid(_orgid As [String])
        Me.orgid = _orgid
    End Sub



    Public Function getpersonid() As [String]
        Return personid
    End Function

    Public Sub setpersonid(_personid As [String])
        Me.personid = _personid
    End Sub



    Public Function getdeviceid() As [String]
        Return deviceid
    End Function

    Public Sub setdeviceid(_deviceid As [String])
        Me.deviceid = _deviceid
    End Sub
End Class

