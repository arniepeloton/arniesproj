Public Class Citizenship

    Function getCitizenshipList(ByVal strKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ctzenshp.ID"
                    .Searchable = False
                End With
                With .Add("Citizenship", SqlDbType.VarChar)
                    .Expression = "ctzenshp.Citizenship"
                End With
            End With

            .SqlFrom = " FROM  dbo.LibraryCitizenship ctzenshp "
            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY ctzenshp.Citizenship ASC"
            pDT = .Requery

        End With

        Return pDT

    End Function

End Class
