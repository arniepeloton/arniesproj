Public Class PIMMeasureSign
    Public Property MaxReturnRow As Integer = 50

    Public Property ID As String
    Public Property MeasureSignID As String

    Public Property Sign As String
    Public Property IsActive As String
    Public Property UserID As String

    Function CheckUniquePIMMeasureSign() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "msrsgn.ID"
                End With
                With .Add("Sign", SqlDbType.VarChar)
                    .Expression = "msrsgn.Sign"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPIMMeasureSign msrsgn"

            .SqlWhere = "WHERE msrsgn.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND msrsgn.Sign = " & ReadyString(Sign)

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getPIMMeasureSignList(ByVal strKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "msrsgn.ID"
                End With
                With .Add("Sign", SqlDbType.VarChar)
                    .Expression = "msrsgn.Sign"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMMeasureSign msrsgn "

            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY msrsgn.Sign ASC"
            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getPIMMeasureSignInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "msrsgn.ID"
                End With
                With .Add("Sign", SqlDbType.VarChar)
                    .Expression = "msrsgn.Sign"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMMeasureSign msrsgn "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE msrsgn.ID IS NULL", " WHERE msrsgn.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SavePIMMeasureSignInfo() As String
        Dim p As New InsertUpdate("MobizLibraryPIMMeasureSign")
        Dim pSql As String
        Dim strIDReturn As String = ""

        p("Sign") = ReadyString(Sign)
        p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
        p("LastUpdated") = "getdate()"

        Try

            Common.WebRequest.ProData.BeginTransaction()

            If (ID.Trim().Length <= 0) Then
                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = ID
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            MeasureSignID = strIDReturn

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn
    End Function


End Class
