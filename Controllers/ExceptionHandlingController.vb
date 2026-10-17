#If DEBUG Then
Public Class ExceptionHandlingController
    Inherits System.Web.Mvc.Controller

    Public Function Index() As String

    End Function

    Public Function Scenario01NoHandler() As String

        SqlError()

    End Function

    Public Function Scenario02GenericErrorHandler() As String

        Try
            SqlError()
        Catch ex As Exception
            Return "An error was caught: " & ex.Message
        End Try

    End Function

    Public Function Scenario03FriendlyErrorHandler() As String

        Try
            SqlError()
        Catch ex As Exception
            Return "Something's not right. Please try again later."
        End Try

    End Function

    Public Function Scenario04CustomizedError() As String

        Try
            SqlWithCustomizedError()
        Catch ex As Exception
            Return ex.Message
        End Try

    End Function

    Public Function Scenario05CustomizedErrorWithFinally() As String

        Try
            SqlWithCustomizedErrorWithFinally()
        Catch ex As Exception
            Return ex.Message
        End Try

    End Function

    Public Function Scenario06SilentErrorDontDoThis() As String

        SetFileTextSilentErrorDontDoThis()

        Return "ok"

    End Function

    Public Function Scenario07BranchedCatch() As String

        Try
            SqlError()
            SetFileTextNoHandler()
        Catch ex As System.Data.SqlClient.SqlException
            ' catch sql errors
            Return "error in sql"
        Catch ex As System.IO.DirectoryNotFoundException
            ' catch directory errors
            Return "error in file creation"
        Catch ex As Exception
            ' catch the rest
            Return ex.Message
        End Try

    End Function

    Public Function Scenario08ranchedCatch2() As String

        Try
            SetFileTextNoHandler()
            SqlError()
        Catch ex As System.Data.SqlClient.SqlException
            ' catch sql errors
            Return "error in sql"
        Catch ex As System.IO.DirectoryNotFoundException
            ' catch directory errors
            Return "error in file creation"
        Catch ex As Exception
            ' catch the rest
            Return ex.Message
        End Try

    End Function

    Private Function SqlError() As DataTable

        Dim pDT As DataTable

        pDT = Common.WebRequest.ProData.Execute("SELECT * FROM NonExistingTable")
        Common.WebRequest.ProData.Disconnect() ' this will never be reached

        Return pDT

    End Function

    Private Function SqlWithCustomizedError() As DataTable

        Dim pDT As DataTable

        Try
            pDT = Common.WebRequest.ProData.Execute("SELECT * FROM NonExistingTable")
            Common.WebRequest.ProData.Disconnect() ' this will never be reached
        Catch ex As Exception
            Common.WebRequest.ProData.Disconnect() ' redundant code!
            Throw New Exception("Something's not right with the query.", ex)
        End Try

        Return pDT

    End Function

    Private Function SqlWithCustomizedErrorWithFinally() As DataTable

        Dim pDT As DataTable

        Try
            ' step 1
            pDT = Common.WebRequest.ProData.Execute("SELECT * FROM NonExistingTable")
        Catch ex As Exception
            ' step 2
            ' execute code here

            ' step 3
            Throw New Exception("Something's not right with the query.")
        Finally
            ' step 4
            Common.WebRequest.ProData.Disconnect() ' this will never be reached
        End Try

        Return pDT

    End Function

    Private Sub SetFileTextNoHandler()

        Dim pBytes() As Byte
        Dim pStr As String
        Dim pFilename As String
        Const pText As String = "This is the file content"

        pFilename = Common.ServerPath & "temp\NonExistingFolder\NonExistingFile.txt"
        Using pFileStream As IO.FileStream = New IO.FileStream( _
            pFilename, IO.FileMode.Create, _
            IO.FileAccess.Write, IO.FileShare.None)

            pBytes = New System.Text.UTF8Encoding().GetBytes(pText & "")
            pFileStream.Write(pBytes, 0, pBytes.Length)
        End Using

    End Sub

    Private Sub SetFileTextSilentErrorDontDoThis()

        Dim pBytes() As Byte
        Dim pStr As String
        Dim pFilename As String
        Const pText As String = "This is the file content"

        Try
            pFilename = Common.ServerPath & "temp\NonExistingFolder\NonExistingFile.txt"
            Using pFileStream As IO.FileStream = New IO.FileStream( _
                pFilename, IO.FileMode.Create, _
                IO.FileAccess.Write, IO.FileShare.None)

                pBytes = New System.Text.UTF8Encoding().GetBytes(pText & "")
                pFileStream.Write(pBytes, 0, pBytes.Length)
            End Using
        Catch ex As Exception
            Debug.WriteLine("The calling function will not know about this error.")
        End Try

    End Sub

    'Private Sub SetFileTextRethrowError()

    '    Dim pBytes() As Byte
    '    Dim pStr As String
    '    Dim pFilename As String
    '    Const pText As String = "This is the file content"

    '    Try
    '        pFilename = Common.ServerPath & "temp\NonExistingFolder\NonExistingFile.txt"
    '        Using pFileStream As IO.FileStream = New IO.FileStream( _
    '            pFilename, IO.FileMode.Create, _
    '            IO.FileAccess.Write, IO.FileShare.None)

    '            pBytes = New System.Text.UTF8Encoding().GetBytes(pText & "")
    '            pFileStream.Write(pBytes, 0, pBytes.Length)
    '        End Using
    '    Catch ex As Exception
    '        Throw New Exception("Unable to set file text", ex)
    '    End Try

    'End Sub
End Class

#End If
