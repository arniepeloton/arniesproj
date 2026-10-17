Public Class City
    Public Property MaxReturnRow As Integer = 50

    Function getCityList(ByVal strKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "cty.ID"
                    .Searchable = False
                End With
                With .Add("City", SqlDbType.VarChar)
                    .Expression = "cty.[Name]"
                End With
                With .Add("Zip", SqlDbType.Int)
                    .Expression = "cty.Zip"
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "cty.IsActive"
                    .Searchable = False
                End With
            End With

            .SqlFrom = " FROM dbo.LibraryCities cty"
            .SqlWhere = " WHERE IsActive = 1 "
            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY cty.[Name] ASC"
            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getCityListAll() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "cty.ID"
                    .Searchable = False
                End With
                With .Add("City", SqlDbType.VarChar)
                    .Expression = "cty.[Name]"
                End With
                With .Add("Zip", SqlDbType.Int)
                    .Expression = "cty.Zip"
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "cty.IsActive"
                    .Searchable = False
                End With
            End With

            .SqlFrom = " FROM dbo.LibraryCities cty"
            .SqlWhere = " WHERE IsActive = 1 "
            .SqlOrder = "ORDER BY cty.[Name] ASC"
            pDT = .Requery

        End With

        Return pDT

    End Function

End Class
