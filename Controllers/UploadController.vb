Namespace WebProj
    Public Class UploadController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult

            Return View()

        End Function

        <HttpPost>
        Public Function StoreSqlite() As String

            Dim pFolder As String
            Dim pFile As HttpPostedFileBase

            pFolder = Common.ServerPath & "temp\StoreSqlite\"
            CreateFolder(pFolder)

            For Each pSourceFilename As String In Request.Files
                pFile = Request.Files(pSourceFilename)
                pFile.SaveAs(pFolder & pFile.FileName)
            Next pSourceFilename

            Return JsonDictionary.Serialize("TimeStamp", DateTimeStamp())

        End Function

    End Class
End Namespace