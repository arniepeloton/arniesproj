Public Class PIMProductFamily
    Public Property MaxReturnRow As Integer = 50

    Public Property ID As String
    Public Property ProductFamilyID As String

    Public Property ProductFamilyLabelID As String
    Public Property IsActive As String
    Public Property UserID As String

    Function CheckUniqueProductFamily() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "pfam.ID"
                End With
                With .Add("ProductFamilyVocLabelID", SqlDbType.UniqueIdentifier)
                    .Expression = "pfam.ProductFamilyVocLabelID"
                End With
                With .Add("Label", SqlDbType.VarChar)
                    .Expression = "lbl.Label"
                End With
                With .Add("Language", SqlDbType.VarChar)
                    .Expression = "lng.Language"
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "ctry.[Name]"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPIMProductFamily pfam " &
                    " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (pfam.ProductFamilyVocLabelID = lbl.ID) " &
                    " INNER JOIN MobizLibraryCountriesLanguage lng ON (lbl.LanguageID = lng.ID) " &
                    " INNER JOIN MobizLibraryCountries ctry ON (lng.CountryID = ctry.ID) "

            .SqlWhere = "WHERE pfam.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND pfam.ProductFamilyVocLabelID " & IIf(ProductFamilyLabelID.Trim().Length = 0, " IS NOT NULL", " = " & ReadyString(ProductFamilyLabelID))

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getProductFamilyList(ByVal strKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "pfam.ID"
                End With
                With .Add("ProductFamilyVocLabelID", SqlDbType.UniqueIdentifier)
                    .Expression = "pfam.ProductFamilyVocLabelID"
                End With
                With .Add("Label", SqlDbType.VarChar)
                    .Expression = "lbl.Label"
                End With
                With .Add("Language", SqlDbType.VarChar)
                    .Expression = "lng.Language"
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "ctry.[Name]"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPIMProductFamily pfam " &
                    " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (pfam.ProductFamilyVocLabelID = lbl.ID) " &
                    " INNER JOIN MobizLibraryCountriesLanguage lng ON (lbl.LanguageID = lng.ID) " &
                    " INNER JOIN MobizLibraryCountries ctry ON (lng.CountryID = ctry.ID) "

            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY lbl.Label ASC"
            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getProductFamilyInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "pfam.ID"
                End With
                With .Add("ProductFamilyVocLabelID", SqlDbType.UniqueIdentifier)
                    .Expression = "pfam.ProductFamilyVocLabelID"
                End With
                With .Add("Label", SqlDbType.VarChar)
                    .Expression = "lbl.Label"
                End With
                With .Add("Language", SqlDbType.VarChar)
                    .Expression = "lng.Language"
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "ctry.[Name]"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPIMProductFamily pfam " &
                    " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (pfam.ProductFamilyVocLabelID = lbl.ID) " &
                    " INNER JOIN MobizLibraryCountriesLanguage lng ON (lbl.LanguageID = lng.ID) " &
                    " INNER JOIN MobizLibraryCountries ctry ON (lng.CountryID = ctry.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE pfam.ID IS NULL", " WHERE pfam.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveProductFamilyInfo() As String
        Dim p As New InsertUpdate("MobizLibraryPIMProductFamily")
        Dim pSql As String
        Dim strIDReturn As String = ""

        p("ProductFamilyVocLabelID") = ReadyString(ProductFamilyLabelID)
        p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "0", WebSession.Instance().PeopleUserID)
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

            ProductFamilyID = strIDReturn

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function


End Class
