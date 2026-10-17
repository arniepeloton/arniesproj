Public Class DataImportLog

    Public Enum eResultType
        Fail
        Success
        AlreadyImported
        SummaryFail
        SummarySuccess
        InvoiceToOrder
    End Enum

    Private Shared ReadOnly mDirectory As String
    Dim mLog As String = ""
    ReadOnly mFilename As String

    Shared Sub New()

        mDirectory = Common.ServerPath & "temp\data import logs\"

    End Sub

    Public Sub New()

        mFilename = DateTimeStamp()

    End Sub

    Public Sub AddLog(vText As String)

        If mLog <> "" Then mLog &= vbCrLf
        mLog &= "[" & Format(Now, "yyy/MM/dd HH:mm:ss") & "] " & vText

    End Sub

    Public Sub Save(vResultType As eResultType)

        Dim pFilename As String

        Try
            pFilename = mDirectory & [Enum].GetName(GetType(eResultType), vResultType) & "\" & mFilename & ".txt"
            FileText(pFilename) = mLog
        Catch ex As Exception
            Stop2()
        End Try

    End Sub
End Class
