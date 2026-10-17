Public Class Occupation

    Function getOccupationList(ByVal strKeyWord As String) As DataTable
        Dim strSQL As String
        Dim dtaTable As DataTable

        strSQL = "SELECT " &
            " occ.ID," &
            " occ.Code," &
            " occ.Occupation " &
         " FROM dbo.LibraryOccupation occ " &
         " WHERE (occ.Code + ' ' + occ.Occupation) " &
         " LIKE '%" & Replace(strKeyWord, "'", "''") & "%' " &
         " ORDER BY occ.Occupation ASC"

        With Common.WebRequest
            dtaTable = .ProData.Execute(strSQL)
        End With

        Return dtaTable

    End Function

End Class
