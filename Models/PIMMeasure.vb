Public Class PIMMeasure
    Public Property MaxReturnRow As Integer = 50

    Public Property ID As String
    Public Property MeasureID As String

    Public Property MeasureNameID As String
    Public Property MeasureSignID As String

    Function CheckLanguageUniqueMeasure() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "msr.ID"
                End With
                With .Add("MeasureNameID", SqlDbType.UniqueIdentifier)
                    .Expression = "msr.MeasureNameID"
                End With
                With .Add("SignID", SqlDbType.UniqueIdentifier)
                    .Expression = "msr.SignID"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPIMMeasure msr"

            .SqlWhere = "WHERE msr.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND msr.MeasureNameID " & IIf(MeasureNameID.Trim().Length = 0, " IS NOT NULL", " = " & ReadyString(MeasureNameID)) &
                        " AND msr.SignID = " & IIf(MeasureSignID.Trim().Length > 0, ReadyString(MeasureSignID), "NULL")

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getMeasureAKA(ByVal strMeasurement As String, ByVal blnIsWild As Boolean) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "msr.ID"
                    .Searchable = False
                End With
                With .Add("MeasureNameID", SqlDbType.UniqueIdentifier)
                    .Expression = "msr.MeasureNameID"
                    .Searchable = False
                End With
                With .Add("Measurement", SqlDbType.VarChar)
                    .Expression = "lbl.Label"
                End With
                With .Add("SignID", SqlDbType.UniqueIdentifier)
                    .Expression = "msr.SignID"
                    .Searchable = False
                End With
                With .Add("Sign", SqlDbType.VarChar)
                    .Expression = "ISNULL(sgn.Sign,'')"
                End With
                With .Add("MeasurementAKA", SqlDbType.VarChar)
                    .Expression = "msr.MeasurementAKA"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMMeasure msr " &
                    " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (msr.MeasureNameID = lbl.ID) " &
                    " LEFT JOIN MobizLibraryPIMMeasureSign sgn ON (msr.SignID = sgn.ID)"

            If (Not blnIsWild) Then
                .SqlWhere = " WHERE lbl.Label = " & ReadyString(strMeasurement) &
                    " OR ISNULL(sgn.Sign,'') = " & ReadyString(strMeasurement)
            Else
                .SqlWhere = " WHERE msr.MeasurementAKA LIKE " & ReadyStringWild(strMeasurement)
            End If

            .SqlOrder = "ORDER BY lbl.Label ASC"
            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getMeasureList(ByVal strKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "msr.ID"
                End With
                With .Add("MeasureNameID", SqlDbType.UniqueIdentifier)
                    .Expression = "msr.MeasureNameID"
                End With
                With .Add("Measurement", SqlDbType.VarChar)
                    .Expression = "lbl.Label"
                End With
                With .Add("SignID", SqlDbType.UniqueIdentifier)
                    .Expression = "msr.SignID"
                End With
                With .Add("Sign", SqlDbType.VarChar)
                    .Expression = "ISNULL(sgn.Sign,'')"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMMeasure msr " &
                    " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (msr.MeasureNameID = lbl.ID) " &
                    " LEFT JOIN MobizLibraryPIMMeasureSign sgn ON (msr.SignID = sgn.ID)"

            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY lbl.Label ASC"
            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getMeasureInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "msr.ID"
                End With
                With .Add("MeasureNameID", SqlDbType.UniqueIdentifier)
                    .Expression = "msr.MeasureNameID"
                End With
                With .Add("Measurement", SqlDbType.VarChar)
                    .Expression = "lbl.Label"
                End With
                With .Add("SignID", SqlDbType.UniqueIdentifier)
                    .Expression = "msr.SignID"
                End With
                With .Add("Sign", SqlDbType.VarChar)
                    .Expression = "ISNULL(sgn.Sign,'')"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMMeasure msr " &
                    " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (msr.MeasureNameID = lbl.ID) " &
                    " LEFT JOIN MobizLibraryPIMMeasureSign sgn ON (msr.SignID = sgn.ID)"

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE msr.ID IS NULL", " WHERE msr.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveMeasureInfo() As String
        Dim p As New InsertUpdate("MobizLibraryPIMMeasure")
        Dim pSql As String
        Dim strIDReturn As String = ""

        p("MeasureNameID") = ReadyString(MeasureNameID)
        p("SignID") = IIf(MeasureSignID.Trim().Length > 0, ReadyString(MeasureSignID), "NULL")
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

            MeasureID = strIDReturn

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function


End Class
