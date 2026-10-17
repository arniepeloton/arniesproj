Public Class Company

    Function getCompanyList(ByVal strKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "comp.ID"
                    .Searchable = False
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = "comp.CompanyName"
                End With
            End With

            .SqlFrom = " FROM  dbo.LibraryCompany comp "
            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY comp.CompanyName ASC"
            pDT = .Requery

        End With

        Return pDT

    End Function

End Class
