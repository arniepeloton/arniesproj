Public Class LibraryTelcoPrefixes
    ReadOnly mProData As ProData
    ReadOnly mParent As Business

    Private Const mUnknownTelcoName As String = "[unknown]"

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function GetOrCreateByMobileNumber(vMobileNumber As String) As DataRow

        Dim pPrefix = ParsePrefix(vMobileNumber)
        Dim pDR = GetOrCreateByPrefix(pPrefix)

        Return pDR

    End Function

    Public Shared Function ParsePrefix(vMobileNumber As String) As String

        Dim pOut As String
        Dim pStartIndex As Integer

        If CStr2(vMobileNumber).Length < 10 Then
            Throw New ApplicationException("vMobileNumber must be at least 10 numbers. ref: vbBSMPTDYVYG!")
        End If

        pStartIndex = vMobileNumber.Length - 10
        pOut = vMobileNumber.Substring(pStartIndex, 3)

        Return pOut

    End Function

    Public Shared Function ParseNumber(vMobileNumber As String) As String

        Dim pOut As String
        Dim pStartIndex As Integer

        If CStr2(vMobileNumber).Length < 10 Then
            Throw New ApplicationException("vMobileNumber must be at least 10 numbers. ref: vb3YHPBHEHXX!")
        End If

        pStartIndex = vMobileNumber.Length - 7
        pOut = vMobileNumber.Substring(pStartIndex, 7)

        Return pOut

    End Function

    Private Function GetOrCreateByPrefix(vPrefix As String) As DataRow

        Dim pDR = GetByPrefix(vPrefix)
        If pDR Is Nothing Then
            CreatePrefix(vPrefix)
            pDR = GetByPrefix(vPrefix)
        End If

        Return pDR

    End Function

    Private Function GetByPrefix(vPrefix As String) As DataRow

        Dim pSql = "SELECT ID" & vbCrLf &
            " FROM MobizLibraryTelcoPrefixes" & vbCrLf &
            " WHERE Prefix = " & ReadyString(vPrefix)
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Private Function CreatePrefix(vPrefix As String) As String

        Dim pIU As New InsertUpdate("MobizLibraryTelcoPrefixes")
        Dim pID = Guid.NewGuid.ToString()
        pIU("ID") = ReadyString(pID)
        pIU("Prefix") = ReadyString(vPrefix)
        Dim pDrTelco = GetOrCreateTelcoByName(mUnknownTelcoName)
        pIU("TelcoId") = ReadyString(pDrTelco("ID"))
        pIU("UserID") = ReadyString(modClient.EmployeeWebGuidID)
        pIU("LastUpdated") = "GETDATE()"
        Dim pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        Return pID

    End Function

    Private Function GetOrCreateTelcoByName(vName As String) As DataRow

        Dim pDR = GetTelcoByName(vName)
        If pDR Is Nothing Then
            CreateTelco(vName)
            pDR = GetTelcoByName(vName)
        End If

        Return pDR

    End Function

    Private Function GetTelcoByName(vName As String) As DataRow

        Dim pSql = "SELECT ID" & vbCrLf &
            " FROM MobizLibraryTelcos" & vbCrLf &
            " WHERE Name = " & ReadyString(vName)
        Dim pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Private Function CreateTelco(vName As String) As String

        Dim pID = Guid.NewGuid.ToString()
        Dim pIU As New InsertUpdate("MobizLibraryTelcos")
        pIU("ID") = ReadyString(pID)
        pIU("Name") = ReadyString(vName)
        pIU("CountryID") = ReadyString(GetTopTelcoCountry())
        pIU("UserID") = ReadyString(modClient.EmployeeWebGuidID)
        pIU("LastUpdated") = "GETDATE()"
        Dim pSql = pIU.GetInsert()
        mProData.ExecuteSQL(psql)

        Return pID

    End Function

    Private Function GetTopTelcoCountry() As String

        Dim pSql = " SELECT TOP 1 MobizLibraryTelcos.CountryID" & vbCrLf &
            " FROM MobizLibraryTelcos" & vbCrLf &
            " GROUP BY MobizLibraryTelcos.CountryID" & vbCrLf &
            " ORDER BY COUNT(*) DESC"
        Dim pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Throw New ApplicationException("MobizLibraryTelcos has no record. ref: vbHHQ26BZH4Z!")
        End If

        Return CStr2(pDR("CountryID"))

    End Function
End Class
