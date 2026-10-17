Public Class JobOccupation

    Function getJobOccupationList(ByVal strKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "occ.ID"
                    .Searchable = False
                End With
                With .Add("Code", SqlDbType.VarChar)
                    .Expression = "occ.Code"
                End With
                With .Add("JobOccupation", SqlDbType.VarChar)
                    .Expression = "occ.JobOccupation"
                End With
            End With

            .SqlFrom = " FROM  dbo.LibraryJobOccupation occ "
            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY occ.JobOccupation ASC"
            pDT = .Requery

        End With

        Return pDT

    End Function

End Class
