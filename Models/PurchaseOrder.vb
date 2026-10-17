Public Class PurchaseOrder
    Public Property OrgID As String
    Public Property PurchaseOrderID As String
    Public Property IsRestrict As String


    Function GetPOData() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ID"
                End With

                With .Add("IsRestrict", SqlDbType.UniqueIdentifier)
                    .Expression = "IsRestrict"
                End With

                With .Add("LastUpdated", SqlDbType.UniqueIdentifier)
                    .Expression = "LastUpdated"
                End With
            End With

            .SqlFrom = " FROM LibraryPurchaseOrderSettings ORDER BY LastUpdated DESC "

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function SavePOData() As String
        Dim p As New InsertUpdate("LibraryPurchaseOrderSettings")
        Dim pSql As String
        Dim strIDReturn As String

        Try
            p("IsRestrict") = ReadyString(IsRestrict)
            p("LastUpdated") = "getdate()"
            p("UserID") = ReadyString(WebSession.Instance().PeopleUserID)

            If PurchaseOrderID = "" Then
                p("ID") = ReadyString(Guid.NewGuid.ToString())
                pSql = p.GetInsert()
            Else
                pSql = p.GetUpdate("ID = " & ReadyString(PurchaseOrderID))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryPurchaseOrderSettings'"
            Dim row As Integer = Common.WebRequest.ProData.ExecuteSQL(pSql)

            If row = 0 Then
                pSql = "INSERT INTO TablesLastUpdated(ID, TableName, FileName, LastUpdated) " & vbCrLf &
                        "VALUES(" & ReadyString(Guid.NewGuid.ToString()) & ", 'LibraryPurchaseOrderSettings', NULL, GetDate())"
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            Return "saved"
        Catch ex As Exception
            Return ex.Message
        End Try

    End Function
End Class
