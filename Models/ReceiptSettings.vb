Public Class ReceiptSettings

    Public Property ID As String
    Public Property ReceiptSettingsID As String
    Public Property MaxReturnRow As Integer = 50

    Public Property OrganizationID As String
    Public Property OrganizationTradeNameID As String
    Public Property PaperSize As String

    Public Property MessageList As Object()
    Public Property PlacesList As Object()

    Public Property ReceiptLogo As Object

    Function getReceiptSettingsList(ByVal strKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "rset.ID"
                    .Searchable = False
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = "org.CompanyName"
                End With
                With .Add("OrgTradeNameID", SqlDbType.UniqueIdentifier)
                    .Expression = "rset.OrgTradeNameID"
                    .Searchable = False
                End With
                With .Add("BusinessTradeName", SqlDbType.VarChar)
                    .Expression = "orgtrade.BusinessTradeName"
                End With
                With .Add("TIN", SqlDbType.VarChar)
                    .Expression = "org.TinNumber"
                End With
                With .Add("PaperSize", SqlDbType.VarChar)
                    .Expression = "rset.PaperSize"
                End With
                With .Add("LastUpdated", SqlDbType.Bit)
                    .Expression = "rset.LastUpdated"
                End With
                With .Add("ModifiedBy", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Firstname)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + '', ''), '') "
                End With
            End With

            .SqlFrom = "FROM dbo.LibraryReceiptSettings rset " &
                    " INNER JOIN dbo.MobizLibraryOrganizationTradeName orgtrade ON (rset.OrgTradeNameID = orgtrade.ID) " &
                    " INNER JOIN dbo.MobizLibraryOrganization org ON (orgtrade.OrganizationID = org.ID) " &
                    " LEFT JOIN dbo.MobizLibraryPeople ppol ON (rset.UserID = ppol.ID)"

            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY org.CompanyName ASC, orgtrade.BusinessTradeName ASC"
            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getMessageList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "msg.ID"
                    .Searchable = False
                End With
                With .Add("ReceiptSettingsID", SqlDbType.UniqueIdentifier)
                    .Expression = "msg.ReceiptSettingsID"
                End With
                With .Add("MessageFooter", SqlDbType.VarChar)
                    .Expression = "msg.MessageFooter"
                End With
                With .Add("SortOrder", SqlDbType.Int)
                    .Expression = "msg.SortOrder"
                End With
            End With

            .SqlFrom = " FROM dbo.LibraryReceiptSettingsMessage msg "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE msg.ReceiptSettingsID IS NULL", " WHERE msg.ReceiptSettingsID = " + ReadyString(strID))

            .SqlOrder = "ORDER BY msg.SortOrder ASC"

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getPlacesList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "rsetfowst.ID"
                    .Searchable = False
                End With
                With .Add("ReceiptSettingsID", SqlDbType.UniqueIdentifier)
                    .Expression = "rsetfowst.ReceiptSettingsID"
                    .Searchable = False
                End With
                With .Add("OrganizationBranchID", SqlDbType.UniqueIdentifier)
                    .Expression = "custfowst.ID"
                    .Searchable = False
                End With
                With .Add("BranchName", SqlDbType.VarChar)
                    .Expression = "custfowst.[Name]"
                End With
                With .Add("BranchAddress", SqlDbType.VarChar)
                    .Expression = "	ISNULL(NULLIF(LTRIM(RTRIM(custfowst.StreetNumber)), ''), '') + " &
                                  " ISNULL(NULLIF(', ' + LTRIM(RTRIM(custfowst.Street)),', '), '') + " &
                                  " ISNULL(NULLIF(', ' + LTRIM(RTRIM(custfowstcty.Name)),', '), '') + " &
                                  " ISNULL(NULLIF(', ' + LTRIM(RTRIM(custfowstctry.Name)),', '), '')"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryOrganizationFOWST custfowst " &
                " LEFT JOIN dbo.MobizLibraryCities custfowstcty ON (custfowst.FowstCityID = custfowstcty.ID) " &
                " LEFT JOIN dbo.MobizLibraryCountries custfowstctry ON (custfowst.FowstCountryID = custfowstctry.ID) " &
                " LEFT JOIN LibraryReceiptSettingsFowst rsetfowst ON (custfowst.ID = rsetfowst.FowstID AND " & IIf(strID.Trim().Length = 0, " rsetfowst.ReceiptSettingsID IS NULL", " rsetfowst.ReceiptSettingsID = " + ReadyString(strID)) & ") "

            .SqlWhere = " WHERE custfowst.OrganizationID " & IIf(WebRequest.Instance.OrgID.Trim().Length = 0, " IS NULL ", " = " & ReadyString(WebRequest.Instance.OrgID))

            .SqlOrder = "ORDER BY ISNULL(NULLIF(LTRIM(RTRIM(custfowst.StreetNumber)), ''), '') + " &
                            " ISNULL(NULLIF(', ' + LTRIM(RTRIM(custfowst.Street)),', '), '') + " &
                            " ISNULL(NULLIF(', ' + LTRIM(RTRIM(custfowstcty.Name)),', '), '') + " &
                            " ISNULL(NULLIF(', ' + LTRIM(RTRIM(custfowstctry.Name)),', '), '') ASC"

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getReceiptSettingsInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            If (strID.Trim().Length > 0) Then

                With .Fields
                    With .Add("ID", SqlDbType.UniqueIdentifier)
                        .Expression = "rset.ID"
                        .Searchable = False
                    End With
                    With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                        .Expression = "orgtrade.OrganizationID"
                        .Searchable = False
                    End With
                    With .Add("CompanyName", SqlDbType.VarChar)
                        .Expression = "org.CompanyName"
                    End With
                    With .Add("OrgTradeNameID", SqlDbType.UniqueIdentifier)
                        .Expression = "rset.OrgTradeNameID"
                        .Searchable = False
                    End With
                    With .Add("BusinessTradeName", SqlDbType.VarChar)
                        .Expression = "orgtrade.BusinessTradeName"
                    End With
                    With .Add("TIN", SqlDbType.VarChar)
                        .Expression = "org.TinNumber"
                    End With
                    With .Add("PaperSize", SqlDbType.VarChar)
                        .Expression = "rset.PaperSize"
                    End With
                End With

                .SqlFrom = "FROM dbo.LibraryReceiptSettings rset " &
                        " INNER JOIN dbo.MobizLibraryOrganizationTradeName orgtrade ON (rset.OrgTradeNameID = orgtrade.ID) " &
                        " INNER JOIN dbo.MobizLibraryOrganization org ON (orgtrade.OrganizationID = org.ID) "

                .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE rset.ID IS NULL ", " WHERE rset.ID = " + ReadyString(strID))

                .SqlOrder = "ORDER BY org.CompanyName ASC, orgtrade.BusinessTradeName ASC"
                pDT = .Requery

            Else

                With .Fields
                    With .Add("ID", SqlDbType.UniqueIdentifier)
                        .Expression = "NULL"
                        .Searchable = False
                    End With
                    With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                        .Expression = "org.ID"
                        .Searchable = False
                    End With
                    With .Add("CompanyName", SqlDbType.VarChar)
                        .Expression = "org.CompanyName"
                    End With
                    With .Add("OrgTradeNameID", SqlDbType.UniqueIdentifier)
                        .Expression = "NULL"
                        .Searchable = False
                    End With
                    With .Add("BusinessTradeName", SqlDbType.VarChar)
                        .Expression = "NULL"
                    End With
                    With .Add("TIN", SqlDbType.VarChar)
                        .Expression = "org.TinNumber"
                    End With
                    With .Add("PaperSize", SqlDbType.VarChar)
                        .Expression = "NULL"
                    End With
                End With

                .SqlFrom = "FROM dbo.MobizLibraryOrganization org "

                .SqlWhere = IIf(WebRequest.Instance.OrgID.Trim().Length = 0, "WHERE org.ID IS NULL ", " WHERE org.ID = " + ReadyString(WebRequest.Instance.OrgID))

                pDT = .Requery

            End If

        End With

        Return pDT

    End Function

    Function CheckReceiptSettings() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "recset.ID"
                    .Searchable = False
                End With
                With .Add("OrgTradeNameID", SqlDbType.UniqueIdentifier)
                    .Expression = "recset.OrgTradeNameID"
                    .Searchable = False
                End With
                With .Add("PaperSize", SqlDbType.UniqueIdentifier)
                    .Expression = "recset.PaperSize"
                    .Searchable = False
                End With
            End With
            .SqlFrom = "FROM LibraryReceiptSettings recset"
            .SqlWhere = " WHERE recset.OrgTradeNameID = " & ReadyString(OrganizationTradeNameID) &
                        " AND recset.PaperSize = " & ReadyString(PaperSize)

            pDT = .Requery

        End With

        Return pDT
    End Function

    Function SaveReceiptSettings() As String
        Dim p As New InsertUpdate("LibraryReceiptSettings")
        Dim pSql As String
        Dim strIDRec As String
        Dim strIDReturn As String

        Try
            p("OrgTradeNameID") = ReadyString(OrganizationTradeNameID)
            p("PaperSize") = ReadyString(PaperSize)
            p("LastUpdated") = "getdate()"
            If (WebSession.Instance().PeopleUserID = Nothing) Then
                p("UserID") = "NULL"
            Else
                p("UserID") = IIf(WebSession.Instance().PeopleUserID.Trim().Length <= 0, "NULL", ReadyString(WebSession.Instance().PeopleUserID))
            End If

            If (ID.Trim().Length <= 0) Then
                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = ID
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            '''Save Messages
            For Each objReceiptMessage As Object In MessageList

                If (objReceiptMessage("RecState").ToString().Trim().Length > 0) Then

                    If (objReceiptMessage("RecState").ToString().Trim() <> "Remove") Then
                        p = New InsertUpdate("LibraryReceiptSettingsMessage")

                        p("ReceiptSettingsID") = ReadyString(strIDReturn)
                        p("MessageFooter") = IIf(objReceiptMessage("MessageFooter").ToString().Trim().Length = 0, "NULL", ReadyString(objReceiptMessage("MessageFooter").ToString().Trim()))
                        p("SortOrder") = IIf(IsNumeric(objReceiptMessage("SortOrder").ToString()), objReceiptMessage("SortOrder").ToString(), 0)

                        If (objReceiptMessage("ID").ToString().Trim().Length <= 0) Then
                            strIDRec = Guid.NewGuid.ToString()
                            p("ID") = ReadyString(strIDRec)
                            pSql = p.GetInsert()
                        Else
                            strIDRec = objReceiptMessage("ID").ToString()
                            pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                        End If

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    Else

                        pSql = "DELETE " &
                               " FROM dbo.LibraryReceiptSettingsMessage " &
                               " WHERE ID " & IIf(objReceiptMessage("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objReceiptMessage("ID").ToString().Trim()))

                        Common.WebRequest.ProData.ExecuteSQL(pSql)

                    End If

                End If

            Next objReceiptMessage

            '''Save Places
            For Each objReceiptPlaces As Object In PlacesList

                Dim tempDict As Dictionary(Of String, Object)

                tempDict = objReceiptPlaces

                If (objReceiptPlaces("RecState").ToString().Trim() <> "Remove") Then
                    p = New InsertUpdate("LibraryReceiptSettingsFowst")

                    p("ReceiptSettingsID") = ReadyString(strIDReturn)
                    p("FowstID") = IIf(objReceiptPlaces("OrganizationBranchID").ToString().Trim().Length = 0, "NULL", ReadyString(objReceiptPlaces("OrganizationBranchID").ToString().Trim()))

                    If Not tempDict.ContainsKey("ID") Then
                        strIDRec = Guid.NewGuid.ToString()
                        p("ID") = ReadyString(strIDRec)
                        pSql = p.GetInsert()
                    Else
                        strIDRec = objReceiptPlaces("ID").ToString()
                        pSql = p.GetUpdate(" ID = " & ReadyString(strIDRec))
                    End If

                    Common.WebRequest.ProData.ExecuteSQL(pSql)

                Else

                    If tempDict.ContainsKey("ID") Then
                        pSql = "DELETE " &
                           " FROM dbo.LibraryReceiptSettingsFowst " &
                           " WHERE ReceiptSettingsID " & IIf(strIDReturn.Trim().Length = 0, "IS NULL", " = " & ReadyString(strIDReturn.Trim())) &
                           " AND ID " & IIf(objReceiptPlaces("ID").ToString().Trim().Length = 0, "IS NULL", " = " & ReadyString(objReceiptPlaces("ID").ToString().Trim()))
                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    End If
                End If

            Next objReceiptPlaces

            pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryReceiptSettings'"
            Common.WebRequest.ProData.ExecuteSQL(pSql)


            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function

End Class
