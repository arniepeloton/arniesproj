Public Class PIMFeature
    Public Property MaxReturnRow As Integer = 50

    Public Property ID As String
    Public Property FeatureID As String

    Public Property FeatureLabelID As String
    Public Property FeatureNameID As String
    Public Property FeatureMeasureID As String

    Public Property FeatureType As String
    Public Property DisplayFormat As String

    Public Property FeatureOptionList As Object()


    Function CheckUniqueFeature() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ftr.ID"
                End With
                With .Add("FeatureLabelID", SqlDbType.UniqueIdentifier)
                    .Expression = "ftr.FeatureLabelID"
                End With
                With .Add("FeatureNameID", SqlDbType.UniqueIdentifier)
                    .Expression = "ftr.FeatureNameID"
                End With
                With .Add("FeatureMeasureID", SqlDbType.UniqueIdentifier)
                    .Expression = "ftr.FeatureMeasureID"
                End With
                With .Add("FeatureType", SqlDbType.VarChar)
                    .Expression = "ftr.FeatureType"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPIMFeature ftr"

            .SqlWhere = "WHERE ftr.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND ftr.FeatureLabelID " & IIf(FeatureLabelID.Trim().Length = 0, " IS NOT NULL", " = " & ReadyString(FeatureLabelID)) &
                        " AND ftr.FeatureNameID " & IIf(FeatureNameID.Trim().Length = 0, " IS NOT NULL", " = " & ReadyString(FeatureNameID)) &
                        " AND ftr.FeatureMeasureID " & IIf(FeatureMeasureID.Trim().Length = 0, " IS NOT NULL", " = " & ReadyString(FeatureMeasureID)) &
                        " AND ftr.FeatureType = " & IIf(FeatureType.Trim().Length > 0, ReadyString(FeatureType), "NULL")

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getFeatureList(ByVal strKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ftr.ID"
                    .Searchable = False
                End With
                With .Add("FeatureLabelID", SqlDbType.UniqueIdentifier)
                    .Expression = "ftr.FeatureLabelID"
                    .Searchable = False
                End With
                With .Add("FeatureLabel", SqlDbType.VarChar)
                    .Expression = "lbl.Label"
                End With
                With .Add("FeatureNameID", SqlDbType.UniqueIdentifier)
                    .Expression = "ftr.FeatureNameID"
                    .Searchable = False
                End With
                With .Add("Feature", SqlDbType.VarChar)
                    .Expression = "ftrname.Label"
                End With
                With .Add("FeatureMeasureID", SqlDbType.UniqueIdentifier)
                    .Expression = "ftr.FeatureMeasureID"
                    .Searchable = False
                End With
                With .Add("FeatureMeasure", SqlDbType.VarChar)
                    .Expression = "lblmsr.Label"
                End With
                With .Add("FeatureMeasureSign", SqlDbType.VarChar)
                    .Expression = "msrsgn.Sign"
                End With
                With .Add("FeatureType", SqlDbType.VarChar)
                    .Expression = "ftr.FeatureType"
                End With
                With .Add("DisplayFormat", SqlDbType.VarChar)
                    .Expression = "ftr.DisplayFormat"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMFeature ftr " &
                    " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (ftr.FeatureLabelID = lbl.ID) " &
                    " INNER JOIN MobizLibraryCountriesLanguageLabel ftrname ON (ftr.FeatureNameID = ftrname.ID) " &
                    " LEFT JOIN MobizLibraryPIMMeasure msr ON (ftr.FeatureMeasureID = msr.ID) " &
                    " LEFT JOIN MobizLibraryCountriesLanguageLabel lblmsr ON (msr.MeasureNameID = lblmsr.ID) " &
                    " LEFT JOIN MobizLibraryPIMMeasureSign msrsgn ON (msr.SignID = msrsgn.ID) "

            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY ftrname.Label ASC"
            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getFeatureOptionList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ftrstr.ID"
                    .Searchable = False
                End With
                With .Add("FeatureID", SqlDbType.UniqueIdentifier)
                    .Expression = "ftrstr.FeatureID"
                    .Searchable = False
                End With
                With .Add("StructuredValue", SqlDbType.VarChar)
                    .Expression = "ftrstr.StructuredValue"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPIMFeatureStructure ftrstr "
            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE ftrstr.FeatureID IS NULL", " WHERE ftrstr.FeatureID = " + ReadyString(strID))
            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getFeatureInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ftr.ID"
                    .Searchable = False
                End With
                With .Add("FeatureLabelID", SqlDbType.UniqueIdentifier)
                    .Expression = "ftr.FeatureLabelID"
                    .Searchable = False
                End With
                With .Add("FeatureLabel", SqlDbType.VarChar)
                    .Expression = "lbl.Label"
                End With
                With .Add("FeatureNameID", SqlDbType.UniqueIdentifier)
                    .Expression = "ftr.FeatureNameID"
                    .Searchable = False
                End With
                With .Add("Feature", SqlDbType.VarChar)
                    .Expression = "ftrname.Label"
                End With
                With .Add("FeatureMeasureID", SqlDbType.UniqueIdentifier)
                    .Expression = "ftr.FeatureMeasureID"
                    .Searchable = False
                End With
                With .Add("FeatureMeasure", SqlDbType.VarChar)
                    .Expression = "lblmsr.Label"
                End With
                With .Add("FeatureMeasureSign", SqlDbType.VarChar)
                    .Expression = "msrsgn.Sign"
                End With
                With .Add("FeatureType", SqlDbType.VarChar)
                    .Expression = "ftr.FeatureType"
                End With
                With .Add("DisplayFormat", SqlDbType.VarChar)
                    .Expression = "ftr.DisplayFormat"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMFeature ftr " &
                    " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (ftr.FeatureLabelID = lbl.ID) " &
                    " INNER JOIN MobizLibraryCountriesLanguageLabel ftrname ON (ftr.FeatureNameID = ftrname.ID) " &
                    " LEFT JOIN MobizLibraryPIMMeasure msr ON (ftr.FeatureMeasureID = msr.ID) " &
                    " LEFT JOIN MobizLibraryCountriesLanguageLabel lblmsr ON (msr.MeasureNameID = lblmsr.ID) " &
                    " LEFT JOIN MobizLibraryPIMMeasureSign msrsgn ON (msr.SignID = msrsgn.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE ftr.ID IS NULL", " WHERE ftr.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SavePIMFeatureInfo() As String
        Dim p As New InsertUpdate("MobizLibraryPIMFeature")
        Dim pSql As String
        Dim strIDReturn As String = ""
        Dim strIDRec As String

        p("FeatureLabelID") = ReadyString(FeatureLabelID)
        p("FeatureNameID") = ReadyString(FeatureNameID)
        p("FeatureMeasureID") = IIf(FeatureMeasureID.Trim().Length > 0, ReadyString(FeatureMeasureID), "NULL")
        p("FeatureType") = ReadyString(FeatureType)
        p("DisplayFormat") = ReadyString(DisplayFormat)
        p("IsActive") = "1"
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

            FeatureID = strIDReturn

            '''Save Feature Structure List
            If (FeatureOptionList.Length > 0) Then

                For Each objFeatureOption As Object In FeatureOptionList

                    If (objFeatureOption("RecState").ToString().Trim().Length > 0) Then

                        If (objFeatureOption("RecState").ToString().Trim() <> "Remove") Then
                            p = New InsertUpdate("MobizLibraryPIMFeatureStructure")

                            p("FeatureID") = ReadyString(strIDReturn)
                            p("StructuredValue") = ReadyString(objFeatureOption("FeatureOption").ToString())

                            If (objFeatureOption("ID").ToString().Trim().Length <= 0) Then
                                strIDRec = Guid.NewGuid.ToString()
                                p("ID") = ReadyString(strIDRec)
                                pSql = p.GetInsert()
                            Else
                                strIDRec = objFeatureOption("ID").ToString()
                                pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                            End If

                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                        Else

                            pSql = "DELETE " &
                                   " FROM dbo.MobizLibraryPIMFeatureStructure " &
                                   " WHERE ID " & IIf(objFeatureOption("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objFeatureOption("ID").ToString().Trim()))

                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                        End If

                    End If

                Next objFeatureOption
            Else
                pSql = "DELETE " &
                       " FROM dbo.MobizLibraryPIMFeatureStructure " &
                       " WHERE FeatureID " & IIf(strIDReturn.Trim().Length = 0, "IS NULL", " = " & ReadyString(strIDReturn))

                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function


End Class
