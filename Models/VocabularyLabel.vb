Public Class VocabularyLabel
    Public Property MaxReturnRow As Integer = 50

    Public Property ID As String
    Public Property VocabularyLabelID As String

    Public Property LanguageID As String
    Public Property Label As String
    Public Property IsActive As String
    Public Property UserID As String

    Function CheckLanguageUniqueLabel() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "voc.ID"
                End With
                With .Add("Label", SqlDbType.VarChar)
                    .Expression = "voc.Label"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryCountriesLanguageLabel voc"

            .SqlWhere = "WHERE voc.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND voc.Label = " & ReadyString(Label)

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getVocabularyLabelList(ByVal strKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "voc.ID"
                    .Searchable = False
                End With
                With .Add("CountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "ctrylng.CountryID"
                    .Searchable = False
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "ctry.[Name]"
                End With
                With .Add("LanguageID", SqlDbType.UniqueIdentifier)
                    .Expression = "voc.LanguageID"
                    .Searchable = False
                End With
                With .Add("Language", SqlDbType.VarChar)
                    .Expression = "ctrylng.Language"
                End With
                With .Add("Label", SqlDbType.VarChar)
                    .Expression = "voc.Label"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryCountriesLanguageLabel voc " &
                    " INNER JOIN MobizLibraryCountriesLanguage ctrylng ON (voc.LanguageID = ctrylng.ID) " &
                    " INNER JOIN MobizLibraryCountries ctry ON (ctrylng.CountryID = ctry.ID)"

            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY voc.Label ASC"
            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getVocabularyLabelInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "voc.ID"
                    .Searchable = False
                End With
                With .Add("LanguageID", SqlDbType.UniqueIdentifier)
                    .Expression = "voc.LanguageID"
                    .Searchable = False
                End With
                With .Add("Language", SqlDbType.VarChar)
                    .Expression = "ctrylng.Language"
                End With
                With .Add("Label", SqlDbType.VarChar)
                    .Expression = "voc.Label"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryCountriesLanguageLabel voc " &
                    " INNER JOIN MobizLibraryCountriesLanguage ctrylng ON (voc.LanguageID = ctrylng.ID)"

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE voc.ID IS NULL", " WHERE voc.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveVocabularyLabelInfo() As String
        Dim p As New InsertUpdate("MobizLibraryCountriesLanguageLabel")
        Dim pSql As String
        Dim strIDReturn As String = ""

        p("LanguageID") = ReadyString(LanguageID)
        p("Label") = ReadyString(Label)
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

            VocabularyLabelID = strIDReturn

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function


End Class
