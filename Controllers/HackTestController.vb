Public Class HackTestController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        'selectTest() : Exit Function

        'Dim p As New InsertUpdate("librarybanks")
        'Dim pSql As String

        'p("ID") = "newid()"
        'p("bankname") = ReadyString("asdasdadasd")
        'p("userid") = 1

        ''pSql = p.GetInsert

        ''Try
        'Common.WebRequest.ProData.BeginTransaction()
        'Common.WebRequest.ProData.ExecuteSQL(pSql)
        'Common.WebRequest.ProData.CommitTransaction()
        ''Catch ex As Exception
        ''    Common.WebRequest.ProData.RollbackTransaction()
        ''End Try


        Return View()

    End Function

    <HttpPost()>
    Public Function Search() As String

        Dim pKeywords As String
        Dim pDT As DataTable

        pKeywords = Common.WebRequest.Data("keywords")
        pDT = selectTest(pKeywords)

        Dim pOut As New JsonDictionary

        pOut.Add("data", pDT)
        pOut.Add("another data", "mao ni ang value")

        Return pOut.Serialize

    End Function

    Private Function selectTest(vkeywords As String) As DataTable

        Dim pFinder As Finder
        Dim pSql As String
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .SearchString = vkeywords

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "LibraryBanks.ID"
                    .Searchable = False
                End With
                .Add("BankName", SqlDbType.VarChar)
                .Add("UserID", SqlDbType.Int)
                With .Add("EmpID", SqlDbType.Int)
                    .Expression = "t.ID"
                End With
            End With

            .SqlFrom = "FROM LibraryBanks" &
                " JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles t ON t.ID = LibraryBanks.UserID"
            .SqlWhere = "WHERE BankName LIKE " & ReadyStringWild("asia")
            .SqlOrder = "ORDER BY BankName"
            'pSql = .CreateSql()
            pDT = .Requery
        End With

        'pDT = Common.WebRequest.ProData.Execute(pSql)
        Return pDT

    End Function

    Public Function SqlUnprotected() As ActionResult
        ' http://www.sql-server-performance.com/2012/protecting-against-sql-injection/
        ' http://blogs.msdn.com/b/gertd/archive/2008/08/21/getting-to-the-crown-jewels.aspx

        Return View()

    End Function

    Public Function CrossSiteScriptingUnprotected() As ActionResult

        Return View()

    End Function

    Public Function CrossSiteScriptingUnprotectedPostComment() As String

        Dim pFilename As String

        pFilename = Common.ServerPath & "temp/CrossSiteScriptingPost"

        Return FileText(CrossSiteScriptingUnprotectedPostComment)

    End Function

    Public Function normal() As ActionResult

        Return View()

    End Function

    Public Function SalesOrder() As ActionResult

        Return View()

    End Function

    Public Function TAccount() As ActionResult

        Return View()

    End Function

    <HttpPost()>
    Public Function AjaxDictionaryArray() As String

        Dim p As Object

        p = Common.WebRequest.AllData("data")
        For Each pObj As Object In p
            Debug.Print("ID: " & pObj("ID"))
        Next pObj

    End Function
End Class
