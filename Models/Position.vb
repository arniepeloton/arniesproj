Public Class Position

    Function getPositionList(ByVal strKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "jpos.ID"
                    .Searchable = False
                End With
                With .Add("Position", SqlDbType.VarChar)
                    .Expression = "jpos.JobPosition"
                End With
            End With

            .SqlFrom = " FROM  dbo.LibraryPositions jpos "
            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY jpos.JobPosition ASC"
            pDT = .Requery

        End With

        Return pDT

    End Function

End Class
