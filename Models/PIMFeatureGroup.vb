Public Class PIMFeatureGroup
    Public Property MaxReturnRow As Integer = 50

    Public Property ID As String
    Public Property FeatureGroupID As String

    Public Property FeatureGroupLabelID As String
    Public Property IsActive As String
    Public Property UserID As String

    Function CheckUniqueFeatureGroup() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "fgrp.ID"
                End With
                With .Add("FeatureGroupVocLabelID", SqlDbType.UniqueIdentifier)
                    .Expression = "fgrp.FeatureGroupVocLabelID"
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

            .SqlFrom = " FROM dbo.MobizLibraryPIMFeatureGroup fgrp " &
                    " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (fgrp.FeatureGroupVocLabelID = lbl.ID) " &
                    " INNER JOIN MobizLibraryCountriesLanguage lng ON (lbl.LanguageID = lng.ID) " &
                    " INNER JOIN MobizLibraryCountries ctry ON (lng.CountryID = ctry.ID) "

            .SqlWhere = "WHERE fgrp.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND fgrp.FeatureGroupVocLabelID " & IIf(FeatureGroupLabelID.Trim().Length = 0, " IS NOT NULL", " = " & ReadyString(FeatureGroupLabelID))

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getFeatureGroupList(ByVal strKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "fgrp.ID"
                End With
                With .Add("FeatureGroupVocLabelID", SqlDbType.UniqueIdentifier)
                    .Expression = "fgrp.FeatureGroupVocLabelID"
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

            .SqlFrom = " FROM dbo.MobizLibraryPIMFeatureGroup fgrp " &
                    " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (fgrp.FeatureGroupVocLabelID = lbl.ID) " &
                    " INNER JOIN MobizLibraryCountriesLanguage lng ON (lbl.LanguageID = lng.ID) " &
                    " INNER JOIN MobizLibraryCountries ctry ON (lng.CountryID = ctry.ID) "

            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY lbl.Label ASC"
            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getFeatureGroupInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "fgrp.ID"
                End With
                With .Add("FeatureGroupVocLabelID", SqlDbType.UniqueIdentifier)
                    .Expression = "fgrp.FeatureGroupVocLabelID"
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

            .SqlFrom = " FROM dbo.MobizLibraryPIMFeatureGroup fgrp " &
                    " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (fgrp.FeatureGroupVocLabelID = lbl.ID) " &
                    " INNER JOIN MobizLibraryCountriesLanguage lng ON (lbl.LanguageID = lng.ID) " &
                    " INNER JOIN MobizLibraryCountries ctry ON (lng.CountryID = ctry.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE fgrp.ID IS NULL", " WHERE fgrp.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveFeatureGroupInfo() As String
        Dim p As New InsertUpdate("MobizLibraryPIMFeatureGroup")
        Dim pSql As String
        Dim strIDReturn As String = ""

        p("FeatureGroupVocLabelID") = ReadyString(FeatureGroupLabelID)
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

            FeatureGroupID = strIDReturn

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function


End Class
