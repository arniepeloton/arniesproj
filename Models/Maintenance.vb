Public Class Maintenance
    Dim pSql As String = vbNullString
    Dim pDT As DataTable

    Public Function getSystemDefaults() As String

        pSql = "SELECT Description, DefaultValue FROM SystemDefaults"

        With Common.WebRequest
            pDT = .ProData.Execute(pSql)
        End With

        Dim vDt As New List(Of Dictionary(Of String, String))

        vDt = JsonDictionary.TableToDictionary(pDT)
        Return JsonDictionary.Serialize(vDt)
    End Function

    Public Function getMBORT() As DataTable

        pSql = "SELECT Description, DefaultValue FROM SystemDefaults WHERE Description='MBORTResponseTime' OR Description='MBORTUnAnsweredBOA'"

        With Common.WebRequest
            pDT = .ProData.Execute(pSql)
        End With

        Return pDT
    End Function

    Public Function saveMBORT(ByVal sdt As Dictionary(Of String, Object)) As Boolean
        Dim sql As String

        sql = "Update SystemDefaults set DefaultValue = " & ReadyString(sdt("MBORTResponseTime")) & " where Description = 'MBORTResponseTime'"

        With Common.WebRequest
            .ProData.ExecuteSQL(sql)
        End With

        sql = "Update SystemDefaults set DefaultValue = " & ReadyString(sdt("MBORTUnAnsweredBOA")) & " where Description = 'MBORTUnAnsweredBOA'"

        With Common.WebRequest
            .ProData.ExecuteSQL(sql)
        End With

        Return True
    End Function
End Class
