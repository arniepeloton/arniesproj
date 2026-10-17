Public Class TagSkuController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        Return View()

    End Function

    <HttpPost()>
    Public Function TagData() As String

        Dim pOut As JsonDictionary
        Dim pArr As String()
        Dim pList As List(Of String)
        Dim pSql As String
        Dim pUpdatedCount As Integer

        pOut = New JsonDictionary
        pArr = Regex.Split(Common.WebRequest.Data("StockNumbers"), "\W")
        If pArr.Length > 0 Then
            pList = UniqueItems(pArr)
            For Each pStr As String In pList
                pSql = "UPDATE LibraryStockInformation" &
                    " SET ID = ID" &
                    " WHERE LTRIM(RTRIM(StockNumber)) = " & ReadyString(pStr.Trim)
                If Common.WebRequest.ProData.ExecuteSQL(pSql) > 0 Then
                    pUpdatedCount += 1
                End If
            Next pStr
            pOut.Add("UpdatedCount", pUpdatedCount)
        Else
            pOut.Add("Error", "No specified stock numbers")
        End If

        Return pOut.Serialize

    End Function
End Class
