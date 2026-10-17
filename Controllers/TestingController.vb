Imports System.Data.SqlClient

Public Class TestingController
    Inherits System.Web.Mvc.Controller

    Dim conn As New SqlConnection("data source=devserver; initial catalog=MobizKonstruksyon; user id=sa; password=ducmain;")

    Function GetPeople()
        Dim Sql As String = "SELECT * FROM LibraryPeople"
        Dim objCommand As New SqlCommand(Sql, conn)

        conn.Open()

        Dim myreader As SqlDataReader = objCommand.ExecuteReader()

        conn.Close()

        Return myreader
    End Function
End Class
