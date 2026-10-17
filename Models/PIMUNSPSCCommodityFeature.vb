Public Class PIMUNSPSCCommodityFeature
    Public Property MaxReturnRow As Integer = 50

    Public Property ID As String
    Public Property UNSPSCCommodityID As String
    Public Property UNSPSCCommFeatureList As Object()

    Function getUNSPSCCommodityList(ByVal strKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "uncom.ID"
                End With
                With .Add("CommodityCode", SqlDbType.VarChar)
                    .Expression = "uncom.CommodityCode"
                End With
                With .Add("Commodity", SqlDbType.VarChar)
                    .Expression = "uncom.Commodity"
                End With
                With .Add("ClassID", SqlDbType.UniqueIdentifier)
                    .Expression = "uncom.ClassID"
                End With
                With .Add("ClassCode", SqlDbType.VarChar)
                    .Expression = "uncls.ClassCode"
                End With
                With .Add("Class", SqlDbType.VarChar)
                    .Expression = "uncls.Class"
                End With
                With .Add("FamilyID", SqlDbType.UniqueIdentifier)
                    .Expression = "uncls.FamilyID"
                End With
                With .Add("FamilyCode", SqlDbType.VarChar)
                    .Expression = "unfmly.FamilyCode"
                End With
                With .Add("Family", SqlDbType.VarChar)
                    .Expression = "unfmly.Family"
                End With
                With .Add("SegmentID", SqlDbType.UniqueIdentifier)
                    .Expression = "unfmly.SegmentID"
                End With
                With .Add("SegmentCode", SqlDbType.VarChar)
                    .Expression = "unseg.SegmentCode"
                End With
                With .Add("Segment", SqlDbType.VarChar)
                    .Expression = "unseg.Segment"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryUNSPSCCommodity uncom " &
                    " INNER JOIN MobizLibraryUNSPSCClass uncls ON (uncom.ClassID = uncls.ID) " &
                    " INNER JOIN MobizLibraryUNSPSCFamily unfmly ON (uncls.FamilyID = unfmly.ID) " &
                    " INNER JOIN MobizLibraryUNSPSCSegment unseg ON (unfmly.SegmentID = unseg.ID)"


            .SearchString = strKeyWord

            .SqlOrder = "ORDER BY uncom.Commodity ASC, " &
                "uncls.Class ASC," &
                "unfmly.Family ASC," &
                "unseg.Segment ASC"

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getUNSPSCCommFeatureList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "commftr.ID"
                    .Searchable = False
                End With
                With .Add("CommodityID", SqlDbType.UniqueIdentifier)
                    .Expression = "commftr.CommodityID"
                    .Searchable = False
                End With
                With .Add("FeatureID", SqlDbType.UniqueIdentifier)
                    .Expression = "commftr.FeatureID"
                    .Searchable = False
                End With
                With .Add("FeatureNameID", SqlDbType.UniqueIdentifier)
                    .Expression = "ftr.FeatureNameID"
                    .Searchable = False
                End With
                With .Add("Feature", SqlDbType.VarChar)
                    .Expression = "ftrlbl.Label"
                End With
                With .Add("FeatureType", SqlDbType.VarChar)
                    .Expression = "ftr.FeatureType"
                End With
                With .Add("DisplayFormat", SqlDbType.VarChar)
                    .Expression = "ftr.DisplayFormat"
                End With
                With .Add("IsKeyFeature", SqlDbType.Int)
                    .Expression = "commftr.IsKeyFeature"
                End With
                With .Add("IsSearchable", SqlDbType.Bit)
                    .Expression = "commftr.IsSearchable"
                End With
                With .Add("SortOrder", SqlDbType.Int)
                    .Expression = "commftr.SortOrder"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryUNSPSCCommodityFeature commftr " &
                    " INNER JOIN MobizLibraryPIMFeature ftr ON (commftr.FeatureID = ftr.ID) " &
                    " INNER JOIN MobizLibraryCountriesLanguageLabel ftrlbl ON (ftr.FeatureNameID = ftrlbl.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, " WHERE commftr.CommodityID IS NULL ", " WHERE commftr.CommodityID = " + ReadyString(strID))

            .SqlOrder = "ORDER BY commftr.SortOrder ASC"
            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getUNSPSCCommodityInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "uncom.ID"
                End With
                With .Add("CommodityCode", SqlDbType.VarChar)
                    .Expression = "uncom.CommodityCode"
                End With
                With .Add("Commodity", SqlDbType.VarChar)
                    .Expression = "uncom.Commodity"
                End With
                With .Add("ClassID", SqlDbType.UniqueIdentifier)
                    .Expression = "uncom.ClassID"
                End With
                With .Add("ClassCode", SqlDbType.VarChar)
                    .Expression = "uncls.ClassCode"
                End With
                With .Add("Class", SqlDbType.VarChar)
                    .Expression = "uncls.Class"
                End With
                With .Add("FamilyID", SqlDbType.UniqueIdentifier)
                    .Expression = "uncls.FamilyID"
                End With
                With .Add("FamilyCode", SqlDbType.VarChar)
                    .Expression = "unfmly.FamilyCode"
                End With
                With .Add("Family", SqlDbType.VarChar)
                    .Expression = "unfmly.Family"
                End With
                With .Add("SegmentID", SqlDbType.UniqueIdentifier)
                    .Expression = "unfmly.SegmentID"
                End With
                With .Add("SegmentCode", SqlDbType.VarChar)
                    .Expression = "unseg.SegmentCode"
                End With
                With .Add("Segment", SqlDbType.VarChar)
                    .Expression = "unseg.Segment"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryUNSPSCCommodity uncom " &
                    " INNER JOIN MobizLibraryUNSPSCClass uncls ON (uncom.ClassID = uncls.ID) " &
                    " INNER JOIN MobizLibraryUNSPSCFamily unfmly ON (uncls.FamilyID = unfmly.ID) " &
                    " INNER JOIN MobizLibraryUNSPSCSegment unseg ON (unfmly.SegmentID = unseg.ID)"

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE uncom.ID IS NULL", " WHERE uncom.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveUNSPSCCommodityInfo() As String
        Dim p As New InsertUpdate("MobizLibraryUNSPSCCommodityFeature")
        Dim pSql As String
        Dim strIDReturn As String = ""
        Dim strIDRec As String

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

            UNSPSCCommodityID = strIDReturn

            '''Save Commodity Feature List
            If (UNSPSCCommFeatureList.Length > 0) Then

                For Each objCommFeatureOption As Object In UNSPSCCommFeatureList

                    If (objCommFeatureOption("RecState").ToString().Trim().Length > 0) Then

                        If (objCommFeatureOption("RecState").ToString().Trim() <> "Remove") Then
                            p = New InsertUpdate("MobizLibraryUNSPSCCommodityFeature")

                            p("CommodityID") = ReadyString(UNSPSCCommodityID)
                            p("FeatureID") = ReadyString(objCommFeatureOption("FeatureID").ToString())
                            p("SortOrder") = objCommFeatureOption("SortOrder").ToString()
                            p("IsKeyFeature") = objCommFeatureOption("IsKeyFeature").ToString()
                            p("IsSearchable") = objCommFeatureOption("IsSearchable").ToString()

                            If (objCommFeatureOption("ID").ToString().Trim().Length <= 0) Then
                                strIDRec = Guid.NewGuid.ToString()
                                p("ID") = ReadyString(strIDRec)
                                pSql = p.GetInsert()
                            Else
                                strIDRec = objCommFeatureOption("ID").ToString()
                                pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                            End If

                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                        Else

                            pSql = "DELETE " &
                                   " FROM dbo.MobizLibraryUNSPSCCommodityFeature " &
                                   " WHERE ID " & IIf(objCommFeatureOption("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objCommFeatureOption("ID").ToString().Trim()))

                            Common.WebRequest.ProData.ExecuteSQL(pSql)

                        End If

                    End If

                Next objCommFeatureOption
            Else
                pSql = "DELETE " &
                       " FROM dbo.MobizLibraryUNSPSCCommodityFeature " &
                       " WHERE CommodityID " & IIf(strIDReturn.Trim().Length = 0, "IS NULL", " = " & ReadyString(strIDReturn))

                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function


End Class
