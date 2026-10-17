Public Class BrowserController
    Inherits System.Web.Mvc.Controller

    Dim mFinder As Finder

    Function Index() As ActionResult
        Return View()
    End Function

    <HttpPost()>
    Public Function LibraryOrganizationRepresentative() As String

        Dim pOrganizationID As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        pOrganizationID = WebRequest.Instance.OrgID

        If (WebRequest.Instance.Data("OrganizationID") <> Nothing) Then
            If (WebRequest.Instance.Data("OrganizationID").Trim().Count > 0) Then
                pOrganizationID = WebRequest.Instance.Data("OrganizationID").Trim()
            End If
        End If

        'todo: add security
        With CreateFinder()
            .MaxRecords = If(CDec2(WebRequest.Instance.Data("Top1")) > 0, 1, 0)
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "orgrep.ID"
                    .Searchable = False
                End With
                With .Add("RepresentativeName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(orgrep.Title)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(orgrep.LastName)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(orgrep.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(orgrep.MiddleName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(orgrep.GenerationalSuffix)),', '), '') "
                End With
                With .Add("TelNo", SqlDbType.VarChar)
                    .Expression = "dbo.GetOrganizationRepTel(orgrep.ID)"
                End With
                With .Add("MobileNo", SqlDbType.VarChar)
                    .Expression = "dbo.GetOrganizationRepMob(orgrep.ID)"
                End With
                With .Add("Email", SqlDbType.VarChar)
                    .Expression = "dbo.GetOrganizationRepEmail(orgrep.ID)"
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "orgrep.IsActive"
                End With
            End With
            .SqlFrom = "FROM dbo.MobizLibraryOrganizationRepresentatives orgrep " &
                    " INNER JOIN dbo.MobizLibraryOrganization org ON (orgrep.OrganizationID = org.ID)"
            .SqlWhere = " WHERE " & IIf(pOrganizationID.Trim().Length = 0, " orgrep.OrganizationID IS NULL ", " orgrep.OrganizationID = " & ReadyString(pOrganizationID))
            .SqlOrder = "ORDER BY orgrep.LastName ASC, " &
                " orgrep.FirstName ASC," &
                " orgrep.MiddleName ASC"
        End With

        Return SubmitFinder()

    End Function
    <HttpPost()>
    Public Function LibraryOrganizationEmployee() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        With CreateFinder()
            .MaxRecords = If(CDec2(WebRequest.Instance.Data("Top1")) > 0, 1, 0)
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "MobizLibraryPeople.ID"
                    .Searchable = False
                End With
                With .Add("PeopleName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeoplePreNominalSuffix.Code)) + ' ', ' '), '') + " &
                        "ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.Surname)) + ', ',', '), '') + " &
                        "ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.FirstName)) + ' ',' '), '') + " &
                        "ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.MiddleName)) + ' ',' '), '') + " &
                        "ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryPeople.PersonalSuffixName)),', '), '') + " &
                        "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(MobizLibraryPeople.ID))),' '), '')"
                End With
            End With
            .SqlFrom = "FROM LibraryOrganizationEmployee" &
                    " JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = LibraryOrganizationEmployee.PeopleID" & vbCrLf
            .SqlFrom &= " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf

            .SqlOrder = "ORDER BY MobizLibraryPeople.Surname ASC, " &
                " MobizLibraryPeople.FirstName ASC," &
                " MobizLibraryPeople.MiddleName ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryOrganizationEmployeeFOWST() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        With CreateFinder()
            .MaxRecords = If(CDec2(WebRequest.Instance.Data("Top1")) > 0, 1, 0)
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "MobizLibraryPeople.ID"
                    .Searchable = False
                End With
                With .Add("PeopleName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeoplePreNominalSuffix.Code)) + ' ', ' '), '') + " &
                        "ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.Surname)) + ', ',', '), '') + " &
                        "ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.FirstName)) + ' ',' '), '') + " &
                        "ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPeople.MiddleName)) + ' ',' '), '') + " &
                        "ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryPeople.PersonalSuffixName)),', '), '') + " &
                        "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(MobizLibraryPeople.ID))),' '), '')"
                End With
            End With
            .SqlFrom = "FROM LibraryOrganizationEmployeeFOWST" &
                    " JOIN MobizLibraryPeople ON MobizLibraryPeople.ID = LibraryOrganizationEmployeeFOWST.PeopleID" & vbCrLf
            .SqlFrom &= " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ON MobizLibraryPeople.TitleID = MobizLibraryPeoplePreNominalSuffix.ID" & vbCrLf

            .SqlWhere = " WHERE LibraryOrganizationEmployeeFOWST.FowstID = " & ReadyString(WebRequest.Instance.FowstID) & vbCrLf
            .SqlOrder = "ORDER BY MobizLibraryPeople.Surname ASC, " &
                " MobizLibraryPeople.FirstName ASC," &
                " MobizLibraryPeople.MiddleName ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryFowst() As String
        Dim strOrgID As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowst.ID"
                    .Searchable = False
                End With
                With .Add("FowstName", SqlDbType.UniqueIdentifier)
                    .Expression = "fowst.Name"
                End With
                With .Add("Address", SqlDbType.VarChar)
                    .Expression = "dbo.SplitJoinComma(ISNULL(fowst.StreetNumber, '') " &
                                    "+ ', ' + ISNULL(fowst.Street, '') " &
                                    "+ ', ' + ISNULL(cty.Name, '') + ', ' + ISNULL(ctry.Name, ''))"
                End With
                With .Add("IsFOWSTInsideAMall", SqlDbType.Bit)
                    .Expression = "fowst.IsFOWSTInsideAMall"
                End With
                With .Add("FOWSTMallName", SqlDbType.VarChar)
                    .Expression = "fowst.FOWSTMallName"
                End With
            End With

            If CInt(WebRequest.Instance.Data("IsThirdPartyOrganization")) = 1 Then
                strOrgID = WebRequest.Instance.Data("ThirdPartyOrganizationID")
            Else
                strOrgID = WebRequest.Instance.OrgID
                If (WebRequest.Instance.Data("OrganizationID") <> Nothing) Then
                    If (WebRequest.Instance.Data("OrganizationID").Trim().Count > 0) Then
                        strOrgID = WebRequest.Instance.Data("OrganizationID").Trim()
                    End If
                End If
            End If
            

            If CInt(WebRequest.Instance.Data("FilterFowst")) = 1 Then
                .SqlFrom = "FROM dbo.LibraryOrganizationEmployee INNER JOIN " &
                      "dbo.LibraryOrganizationEmployeeFOWST ON dbo.LibraryOrganizationEmployee.PeopleID = dbo.LibraryOrganizationEmployeeFOWST.PeopleID INNER JOIN " &
                      "dbo.MobizLibraryOrganizationFOWST AS fowst ON dbo.LibraryOrganizationEmployeeFOWST.FowstID = fowst.ID LEFT OUTER JOIN " &
                      "dbo.MobizLibraryCities AS cty ON fowst.FowstCityID = cty.ID LEFT OUTER JOIN " &
                      "dbo.MobizLibraryCountries AS ctry ON fowst.FowstCountryID = ctry.ID"
                .SqlWhere = " WHERE fowst.OrganizationID = " & ReadyString(strOrgID) & " AND dbo.LibraryOrganizationEmployee.PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID) & " AND dbo.LibraryOrganizationEmployee.Deactivated = 0"
            Else
                .SqlFrom = "FROM dbo.MobizLibraryOrganizationFOWST fowst " &
                " LEFT OUTER JOIN dbo.MobizLibraryCities cty ON (fowst.FowstCityID = cty.ID) " &
                " LEFT OUTER JOIN dbo.MobizLibraryCountries ctry ON (fowst.FowstCountryID = ctry.ID)"

                .SqlWhere = " WHERE fowst.OrganizationID = " & ReadyString(strOrgID)
            End If

            .SqlOrder = "ORDER BY Address"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryFowstAll() As String
        Dim strOrgID As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        With CreateFinder()
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowst.ID"
                    .Searchable = False
                End With
                With .Add("FowstOrgName", SqlDbType.VarChar)
                    .Expression = " (CASE org.IsOrganization " &
                                  "     WHEN 1 THEN  org.CompanyName " &
                                  "     ELSE  " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(orgprenominal.Code)) + ' ', ' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonSurname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonFirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonMiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(org.PersonSuffixName)),', '), '') " &
                                  " END)"
                End With
                With .Add("FowstName", SqlDbType.UniqueIdentifier)
                    .Expression = "fowst.Name"
                End With
                With .Add("Address", SqlDbType.VarChar)
                    .Expression = "dbo.SplitJoinComma(ISNULL(fowst.StreetNumber, '') " &
                                    "+ ', ' + ISNULL(fowst.Street, '') " &
                                    "+ ', ' + ISNULL(cty.Name, '') + ', ' + ISNULL(ctry.Name, ''))"
                End With
                With .Add("IsFOWSTInsideAMall", SqlDbType.Bit)
                    .Expression = "fowst.IsFOWSTInsideAMall"
                End With
                With .Add("FOWSTMallName", SqlDbType.VarChar)
                    .Expression = "fowst.FOWSTMallName"
                End With
            End With

            .SqlFrom = "FROM dbo.MobizLibraryOrganizationFOWST fowst " &
            " INNER JOIN MobizLibraryOrganization org ON (fowst.OrganizationID = org.ID) " &
            " LEFT JOIN MobizLibraryPeoplePreNominalSuffix orgprenominal ON (org.PersonTitleID = orgprenominal.ID) " &
            " LEFT OUTER JOIN dbo.MobizLibraryCities cty ON (fowst.FowstCityID = cty.ID) " &
            " LEFT OUTER JOIN dbo.MobizLibraryCountries ctry ON (fowst.FowstCountryID = ctry.ID) "

            If CInt(WebRequest.Instance.Data("IsSubscriber")) = 1 Then
                .SqlFrom &= " INNER JOIN dbo.MobizLibrarySubscription sub ON (org.ID = sub.OrganizationID) "
            End If


            .SqlOrder = "ORDER BY Address"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryFowstPrinter() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "fwostprt.ID"
                    .Searchable = False
                End With
                With .Add("PrinterIPAddress", SqlDbType.VarChar)
                    .Expression = "fwostprt.PrinterIPAddress"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryOrganizationFOWSTPrinter fwostprt "

            .SqlWhere = " WHERE fwostprt.FowstID = " & ReadyString(WebRequest.Instance.FowstID)

            .SqlOrder = "ORDER BY fwostprt.PrinterIPAddress ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryBanks() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                .Add("BankCode", SqlDbType.VarChar)
                .Add("BankName", SqlDbType.VarChar)
            End With
            .SqlFrom = " FROM MobizLibraryBanks "
            .SqlWhere = " WHERE IsActive = 1 "
            .SqlOrder = " ORDER BY BankName"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryBankBranches() As String

        Dim pBankID As String

        pBankID = Common.WebRequest.Data("BankID")

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "bnkbr.ID"
                    .Searchable = False
                End With
                With .Add("BranchName", SqlDbType.VarChar)
                    .Expression = "bnkbr.BranchName"
                End With
                With .Add("Address", SqlDbType.VarChar)
                    .Expression = "bnkbr.Address"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryBankBranch bnkbr "
            .SqlWhere = " WHERE " & IIf(pBankID.Trim().Length = 0, " bnkbr.BankID IS NULL ", " bnkbr.BankID = " & ReadyString(pBankID))
            .SqlOrder = "ORDER BY bnkbr.BranchName ASC "

        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryCountries() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                .Add("Code", SqlDbType.VarChar)
                .Add("Name", SqlDbType.VarChar)
                .Add("CallingCode", SqlDbType.VarChar)
                With .Add("NoOfDigits", SqlDbType.VarChar)
                    .Searchable = False
                End With
            End With
            .SqlFrom = "FROM MobizLibraryCountries"
            .SqlWhere = "WHERE IsActive = 1"
            .SqlOrder = "ORDER BY Code"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryCountryLanguage() As String

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ctrylng.ID"
                    .Searchable = False
                End With
                With .Add("CountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "ctrylng.CountryID"
                    .Searchable = False
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "ctry.[Name]"
                End With
                With .Add("Language", SqlDbType.VarChar)
                    .Expression = "ctrylng.Language"
                End With
            End With
            .SqlFrom = "FROM MobizLibraryCountriesLanguage ctrylng " &
                    " INNER JOIN MobizLibraryCountries ctry ON (ctrylng.CountryID = ctry.ID)"
            .SqlOrder = "ORDER BY ctrylng.Language ASC," &
                    " ctry.[Name] ASC"
        End With

        Return SubmitFinder()

    End Function

    Public Function LibraryCountryLanguageVocabularyLabel() As String

        With CreateFinder()
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

            .SqlOrder = "ORDER BY voc.Label ASC"

        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryPIMPriceLevel() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prclvl.ID"
                    .Searchable = False
                End With
                With .Add("PriceLevel", SqlDbType.VarChar)
                    .Expression = "prclvl.PriceLevel"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryPIMPriceLevel prclvl "

            .SqlOrder = "ORDER BY prclvl.PriceLevel ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryPIMFeature() As String

        'todo: add security
        With CreateFinder()
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
                    .Expression = "msrname.Label"
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
                    " LEFT JOIN MobizLibraryCountriesLanguageLabel msrname ON (msr.MeasureNameID = msrname.ID) " &
                    " LEFT JOIN MobizLibraryPIMMeasureSign msrsgn ON (msr.SignID = msrsgn.ID) "

            .SqlOrder = "ORDER BY ftrname.Label ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryPIMProductFamily() As String

        'todo: add security
        With CreateFinder()
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

            .SqlOrder = "ORDER BY lbl.Label ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryUNSPSCCommodity() As String

        'todo: add security
        With CreateFinder()
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

            .SqlOrder = "ORDER BY uncom.Commodity ASC, " &
                    "uncls.Class ASC," &
                    "unfmly.Family ASC," &
                    "unseg.Segment ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryPIMMeasureSign() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "msrsgn.ID"
                End With
                With .Add("Sign", SqlDbType.VarChar)
                    .Expression = "msrsgn.Sign"
                End With
            End With
            .SqlFrom = "FROM MobizLibraryPIMMeasureSign msrsgn "
            .SqlOrder = "ORDER BY msrsgn.Sign ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryPIMMeasure() As String

        'todo: add security
        With CreateFinder()
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
            .SqlOrder = "ORDER BY lbl.Label ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryPIMMeasureWeight() As String

        'todo: add security
        With CreateFinder()
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

            .SqlWhere = " WHERE msr.IsStandardWeight = 1"

            .SqlOrder = "ORDER BY lbl.Label ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryPIMMeasureLength() As String

        'todo: add security
        With CreateFinder()
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

            .SqlWhere = " WHERE msr.IsStandardLength = 1"

            .SqlOrder = "ORDER BY lbl.Label ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryPIMProductMeasure() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdmea.ID"
                End With
                With .Add("UnitMeasureID", SqlDbType.UniqueIdentifier)
                    .Expression = "prdmea.UnitMeasureID"
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
                With .Add("Quantity", SqlDbType.Decimal)
                    .Expression = "prdmea.Quantity"
                End With
            End With

            .SqlFrom = " FROM MobizLibraryPIMProductMeasurement prdmea " &
                    " INNER JOIN MobizLibraryPIMMeasure msr ON (prdmea.UnitMeasureID = msr.ID) " &
                    " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (msr.MeasureNameID = lbl.ID) " &
                    " LEFT JOIN MobizLibraryPIMMeasureSign sgn ON (msr.SignID = sgn.ID)"

            .SqlWhere = " WHERE prdmea.ProductID = " & ReadyString(WebRequest.Instance.Data("ProductID"))

            .SqlOrder = " ORDER BY prdmea.Quantity ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryAreaCodes() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                .Add("Area", SqlDbType.VarChar)
                .Add("CallingCode", SqlDbType.VarChar)
            End With
            .SqlFrom = "FROM MobizLibraryAreaCodes"
            .SqlOrder = "ORDER BY Area"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryTelcoPrefixes() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prf.ID"
                    .Searchable = False
                End With
                With .Add("Prefix", SqlDbType.VarChar)
                    .Expression = "prf.Prefix"
                End With
                With .Add("TelCo", SqlDbType.VarChar)
                    .Expression = "telco.[Name]"
                End With
            End With
            .SqlFrom = "FROM MobizLibraryTelcoPrefixes prf " &
                    " INNER JOIN MobizLibraryTelcos telco ON (prf.TelcoId = telco.Id)"
            .SqlOrder = "ORDER BY prf.Prefix"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryLocation() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                With .Add("Location", SqlDbType.VarChar)
                    .Expression = "LocationName"
                End With
            End With
            .SqlFrom = "FROM MobizLibraryLocations "
            .SqlWhere = "WHERE IsActive = 1"
            .SqlOrder = "ORDER BY LocationName "
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibrarySecurityRole() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                With .Add("Code", SqlDbType.VarChar)
                    .Expression = "Code"
                End With
                With .Add("Role", SqlDbType.VarChar)
                    .Expression = "Role"
                End With
                With .Add("Description", SqlDbType.VarChar)
                    .Expression = "Description"
                End With
            End With
            .SqlFrom = "FROM LibrarySecurityRole "
            .SqlWhere = "WHERE IsActive = 1"
            .SqlOrder = "ORDER BY Role "
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryCities() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                .Add("Name", SqlDbType.VarChar)
            End With
            .SqlFrom = "FROM MobizLibraryCities"
            .SqlWhere = "WHERE IsActive = 1"
            .SqlOrder = "ORDER BY Name"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryCitizenship() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                .Add("Citizenship", SqlDbType.VarChar)
            End With
            .SqlFrom = "FROM MobizLibraryCitizenship"
            .SqlWhere = "WHERE IsActive = 1"
            .SqlOrder = "ORDER BY Citizenship"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryOrganization() As String

        Dim pFilterCustomers As Boolean
        Dim pFilterTypePerson As Boolean
        Dim pFilterFranchisor As Boolean

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        pFilterCustomers = CDec2(Common.WebRequest.Data("FilterCustomers"))
        If pFilterCustomers Then
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        End If
        pFilterTypePerson = CDec2(Common.WebRequest.Data("FilterTypePerson"))
        pFilterFranchisor = CDec2(Common.WebRequest.Data("FilterFranchisor"))

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "org.ID"
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = " (CASE org.IsOrganization " &
                                  "     WHEN 1 THEN  org.CompanyName " &
                                  "     ELSE  " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(orgprenominal.Code)) + ' ', ' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonSurname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonFirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonMiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(org.PersonSuffixName)),', '), '') " &
                                  " END)"
                End With
                With .Add("TinNumber", SqlDbType.VarChar)
                    .Expression = "org.TinNumber"
                End With
                With .Add("PersonSeniorCitizensIDNo", SqlDbType.VarChar)
                    .Expression = "org.PersonSeniorCitizensIDNo"
                End With
                With .Add("PersonPWDIDNo", SqlDbType.VarChar)
                    .Expression = "org.PersonPWDIDNo"
                End With
                With .Add("PersonPassportNo", SqlDbType.VarChar)
                    .Expression = "org.PersonPassportNo"
                End With
                With .Add("PersonDriversLicenseNo", SqlDbType.VarChar)
                    .Expression = "org.PersonDriversLicenseNo"
                End With
                With .Add("PersonSSSNo", SqlDbType.VarChar)
                    .Expression = "org.PersonSSSNo"
                End With
                With .Add("PersonGSISIDNo", SqlDbType.VarChar)
                    .Expression = "org.PersonGSISIDNo"
                End With
                With .Add("PersonPRCIDNo", SqlDbType.VarChar)
                    .Expression = "org.PersonPRCIDNo"
                End With
                With .Add("PersonPostalIDNo", SqlDbType.VarChar)
                    .Expression = "org.PersonPostalIDNo"
                End With
                With .Add("PersonVotersIDNo", SqlDbType.VarChar)
                    .Expression = "org.PersonVotersIDNo"
                End With
                With .Add("PersonOtherIDNo", SqlDbType.VarChar)
                    .Expression = "org.PersonOtherIDNo"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryOrganization org " &
                    " LEFT JOIN MobizLibraryPeoplePreNominalSuffix orgprenominal ON (org.PersonTitleID = orgprenominal.ID)"
            If pFilterCustomers Then
                .SqlFrom &= " JOIN LibraryOrganizationCustomer ON LibraryOrganizationCustomer.OrganizationID = org.ID" & vbCrLf
            End If
            If CDec2(Common.WebRequest.Data("FilterPeople")) <> 0 Then
                .SqlFrom &= " JOIN MobizLibraryPeopleOrganization ON MobizLibraryPeopleOrganization.OrganizationID = org.ID" & vbCrLf &
                    " AND MobizLibraryPeopleOrganization.PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID) & vbCrLf &
                    " AND MobizLibraryPeopleOrganization.IsConfirmed = 1" & vbCrLf &
                    " AND MobizLibraryPeopleOrganization.IsTerminated = 0" & vbCrLf

            End If
            'additional by janine sept 12 2014 for franchisor
            If pFilterFranchisor Then
                .SqlFrom &= " JOIN MobizLibrarySubscription ON MobizLibrarySubscription.OrganizationID = org.ID "

            End If
            'additional by janine july 26 for senior citizen customers
            If pFilterTypePerson Then
                .SqlWhere &= " WHERE org.IsOrganization = 0 " & vbCrLf
            End If
            'additional by janine sept 12 2014 for franchisor
            If pFilterFranchisor Then
                .SqlWhere &= " WHERE org.IsFranchisor = 1 " & vbCrLf
            End If
            .SqlOrder = "ORDER BY (CASE org.IsOrganization " &
                        "     WHEN 1 THEN  org.CompanyName " &
                        "     ELSE  " &
                        "         ISNULL(NULLIF(LTRIM(RTRIM(orgprenominal.Code)) + ' ', ' '), '') + " &
                        "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonSurname)) + ', ',', '), '') + " &
                        "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonFirstName)) + ' ',' '), '') + " &
                        "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonMiddleName)) + ' ',' '), '') + " &
                        "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(org.PersonSuffixName)),', '), '') " &
                        " END)"
        End With

        Dim pDT As DataTable
        Dim pOut As JsonDictionary
        Dim pDefaultCustomerOrganizationID As String

        pOut = New JsonDictionary
        pDT = mFinder.Requery
        pOut.Add("Results", pDT)
        If pFilterCustomers Then
            pDefaultCustomerOrganizationID = WebRequest.Instance.Business.SystemDefaults.CustomerOrganizationID
            If pDefaultCustomerOrganizationID Is Nothing Then
                pOut.Add("OfferAsDefault", 1)
            End If
        End If
        Return pOut.Serialize

    End Function

    <HttpPost()>
    Public Function LibraryOrganizationBanks() As String

        Dim pFilterCustomers As Boolean

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        pFilterCustomers = CDec2(Common.WebRequest.Data("FilterCustomers"))
        If pFilterCustomers Then
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        End If

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "org.ID"
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = " (CASE org.IsOrganization " &
                                  "     WHEN 1 THEN  org.CompanyName " &
                                  "     ELSE  " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(orgprenominal.Code)) + ' ', ' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonSurname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonFirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonMiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(org.PersonSuffixName)),', '), '') " &
                                  " END)"
                End With
            End With

            .SqlFrom = "FROM MobizLibraryOrganization org " &
                    " LEFT JOIN MobizLibraryPeoplePreNominalSuffix orgprenominal ON (org.PersonTitleID = orgprenominal.ID)"

            .SqlWhere = "WHERE IsABank = 1"

            .SqlOrder = "ORDER BY (CASE org.IsOrganization " &
                        "     WHEN 1 THEN  org.CompanyName " &
                        "     ELSE  " &
                        "         ISNULL(NULLIF(LTRIM(RTRIM(orgprenominal.Code)) + ' ', ' '), '') + " &
                        "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonSurname)) + ', ',', '), '') + " &
                        "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonFirstName)) + ' ',' '), '') + " &
                        "         ISNULL(NULLIF(LTRIM(RTRIM(org.PersonMiddleName)) + ' ',' '), '') + " &
                        "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(org.PersonSuffixName)),', '), '') " &
                        " END)"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryOrganizationTradeName() As String
        Dim pOrganizationID As String

        pOrganizationID = Common.WebRequest.Data("OrganizationID")

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                .Add("BusinessTradeName", SqlDbType.VarChar)
                .Add("BusinessTradeNameInitial", SqlDbType.VarChar)
            End With
            .SqlFrom = "FROM MobizLibraryOrganizationTradeName "
            .SqlWhere = " WHERE " & IIf(pOrganizationID.Trim().Length = 0, "OrganizationID IS NULL ", "OrganizationID = " & ReadyString(pOrganizationID))
            .SqlOrder = "ORDER BY BusinessTradeName ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryOrganizationCategory() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "cattyp.ID"
                    .Searchable = False
                End With
                With .Add("CategoryID", SqlDbType.UniqueIdentifier)
                    .Expression = "cattyp.CategoryID"
                    .Searchable = False
                End With
                With .Add("CategoryType", SqlDbType.VarChar)
                    .Expression = "cattyp.CategoryType"
                End With
                With .Add("Category", SqlDbType.VarChar)
                    .Expression = "cat.Category"
                End With
            End With

            .SqlFrom = " FROM MobizLibraryOrgType cattyp " &
                    " INNER JOIN MobizLibraryOrgCategory cat ON (cattyp.CategoryID = cat.ID)"

            .SqlOrder = "ORDER BY cat.Category ASC, cattyp.CategoryType ASC"

        End With

        Return SubmitFinder()

    End Function

    Public Function LibraryOrganizationReceiptSettings() As String
        Dim pOrganizationID As String

        pOrganizationID = Common.WebRequest.Data("OrganizationID")

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ret.ID"
                    .Searchable = False
                End With
                With .Add("OrgTradeNameID", SqlDbType.UniqueIdentifier)
                    .Expression = "ret.OrgTradeNameID"
                    .Searchable = False
                End With
                With .Add("BusinessTradeName", SqlDbType.VarChar)
                    .Expression = "trade.BusinessTradeName"
                End With
            End With

            .SqlFrom = " FROM LibraryReceiptSettings ret " &
                " INNER JOIN MobizLibraryOrganizationTradeName trade ON (ret.OrgTradeNameID = trade.ID)"

            .SqlWhere = " WHERE " & IIf(pOrganizationID.Trim().Length = 0, " ret.OrganizationID IS NULL ", " ret.OrganizationID = " & ReadyString(pOrganizationID))
            .SqlOrder = "ORDER BY trade.BusinessTradeName ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryJobOccupation() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                .Add("Code", SqlDbType.VarChar)
                .Add("JobOccupation", SqlDbType.VarChar)
                .Add("Tags", SqlDbType.VarChar)
            End With
            .SqlFrom = "FROM MobizLibraryJobOccupation"
            .SqlWhere = "WHERE IsActive = 1"
            .SqlOrder = "ORDER BY JobOccupation"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryPeople() As String

        Dim pEmployeesOnly As Boolean

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        pEmployeesOnly = CDec2(Common.WebRequest.Data("EmployeesOnly"))
        If pEmployeesOnly Then
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        End If

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppol.ID"
                    .Searchable = False
                End With
                With .Add("PeopleName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(ppol.PersonalSuffixName)),', '), '') + " &
                                  "ISNULL(NULLIF(' ' + LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppol.ID))),' '), '')"
                End With

                .Add("DateOfBirth", SqlDbType.Date)

                With .Add("MotherMaidenName", SqlDbType.UniqueIdentifier)
                    .Expression = "ppol.MotherMaidenSurname + ', ' + ppol.MotherMaidenFirstName"
                    .Searchable = False
                End With

            End With
            .SqlFrom = "FROM MobizLibraryPeople ppol " & vbCrLf
            If pEmployeesOnly Then
                .SqlFrom &= " JOIN LibraryOrganizationEmployee ON LibraryOrganizationEmployee.PeopleID = ppol.ID" & vbCrLf
            End If
            .SqlFrom &= " LEFT JOIN MobizLibraryPeoplePreNominalSuffix ppolprenominal ON (ppol.TitleID = ppolprenominal.ID)" & vbCrLf

            .SqlOrder = "ORDER BY (ISNULL(ppol.Surname,'') + ', ' + ISNULL(ppol.FirstName,'') + ' ' + ISNULL(ppol.MiddleName,'')) ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryPeoplePreSuffix() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolpresuffix.ID"
                    .Searchable = False
                End With
                With .Add("Code", SqlDbType.VarChar)
                    .Expression = "ppolpresuffix.Code"
                End With
                With .Add("Description", SqlDbType.VarChar)
                    .Expression = "ppolpresuffix.Description"
                End With
                With .Add("Category", SqlDbType.VarChar)
                    .Expression = "ppolpresuffix.Category"
                End With
            End With
            .SqlFrom = "FROM MobizLibraryPeoplePreNominalSuffix ppolpresuffix "
            .SqlOrder = "ORDER BY ppolpresuffix.SortOrder ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryPeoplePostSuffix() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolpostsuffix.ID"
                    .Searchable = False
                End With
                With .Add("Code", SqlDbType.VarChar)
                    .Expression = "ppolpostsuffix.Code"
                End With
                With .Add("Description", SqlDbType.VarChar)
                    .Expression = "ppolpostsuffix.Description"
                End With
                With .Add("Type", SqlDbType.VarChar)
                    .Expression = "ppolpostsuffix.Type"
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "ppolpostsuffix.Country"
                End With
            End With
            .SqlFrom = "FROM MobizLibraryPeoplePostNominalSuffix ppolpostsuffix "
            .SqlOrder = "ORDER BY ppolpostsuffix.Code ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryEducation() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "educ.ID"
                    .Searchable = False
                End With
                With .Add("CourseCode", SqlDbType.VarChar)
                    .Expression = "educ.CourseCode"
                End With
                With .Add("CourseTitle", SqlDbType.VarChar)
                    .Expression = "educ.CourseTitle"
                End With
            End With
            .SqlFrom = "FROM MobizLibraryEducationCourse educ "
            .SqlOrder = "ORDER BY educ.CourseTitle ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryFOWSTType() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                .Add("Name", SqlDbType.VarChar)
            End With
            .SqlFrom = "FROM MobizLibraryFOWSTType"
            .SqlWhere = "WHERE IsActive = 1"
            .SqlOrder = " ORDER BY SortOrder ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryFOWSTWarehouseType() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                .Add("Name", SqlDbType.VarChar)
            End With
            .SqlFrom = "FROM MobizLibraryFOWSTWarehouseType"
            .SqlWhere = "WHERE IsActive = 1"
            .SqlOrder = " ORDER BY Name ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryFOWSTRetailStoreType() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                .Add("Name", SqlDbType.VarChar)
            End With
            .SqlFrom = "FROM MobizLibraryFOWSTRetailStoreType"
            .SqlWhere = "WHERE IsActive = 1"
            .SqlOrder = " ORDER BY Name ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryIndustryClassification() As String

        'todo: add security
        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                With .Add("Code", SqlDbType.VarChar)
                    .Expression = "indstryclass.Code"
                End With
                With .Add("Class", SqlDbType.VarChar)
                    .Expression = "indstryclass.Class"
                End With
                With .Add("Description", SqlDbType.VarChar)
                    .Expression = "indstryclass.Description"
                End With
            End With
            .SqlFrom = "FROM MobizLibraryIndustryClass indstryclass"
            .SqlOrder = "ORDER BY indstryclass.Class ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryDevice() As String
        Dim strKeyWord As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        strKeyWord = Common.WebRequest.Data("KeyWord")

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "dev.ID"
                    .Searchable = False
                End With
                With .Add("PhoneName", SqlDbType.VarChar)
                    .Expression = "dev.PhoneName"
                End With
                With .Add("PhoneModel", SqlDbType.VarChar)
                    .Expression = "dev.PhoneModel"
                End With
                With .Add("IMEI", SqlDbType.VarChar)
                    .Expression = "dev.IMEI"
                End With
                With .Add("UUID", SqlDbType.VarChar)
                    .Expression = "dev.UUID"
                End With
                With .Add("OS", SqlDbType.VarChar)
                    .Expression = "dev.OS"
                End With
                With .Add("DeviceNo", SqlDbType.Int)
                    .Expression = "dev.DeviceNo"
                End With
                With .Add("IsActive", SqlDbType.Int)
                    .Expression = "dev.IsActive"
                End With
                With .Add("LastUpdated", SqlDbType.Int)
                    .Expression = "dev.LastUpdated"
                End With
                With .Add("ModifiedBy", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Firstname)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) + '', ''), '') "
                End With
            End With
            .SqlFrom = "FROM LibraryDevice dev " &
                        " LEFT JOIN dbo.MobizLibraryPeople ppol ON (dev.PeopleID = ppol.ID)"
            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY dev.PhoneName ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryDevicePrinter() As String
        Dim strKeyWord As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        strKeyWord = Common.WebRequest.Data("KeyWord")

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "prntr.ID"
                    .Searchable = False
                End With
                With .Add("PrinterName", SqlDbType.VarChar)
                    .Expression = "prntr.PrinterName"
                End With
                With .Add("PrinterType", SqlDbType.VarChar)
                    .Expression = "prntr.PrinterType"
                End With
                With .Add("FowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "prntr.FowstID"
                    .Searchable = False
                End With
                With .Add("Fowst", SqlDbType.VarChar)
                    .Expression = "fowst.Name"
                End With
                With .Add("IPAddress", SqlDbType.VarChar)
                    .Expression = "prntr.IPAddress"
                End With
                With .Add("IsActive", SqlDbType.VarChar)
                    .Expression = "prntr.IsActive"
                    .Searchable = False
                End With
            End With

            .SqlFrom = "FROM LibraryDevicePrinter prntr " &
                " INNER JOIN MobizLibraryOrganizationFOWST fowst ON (fowst.ID = prntr.FowstID) "

            .SearchString = strKeyWord

            .SqlOrder = "ORDER BY prntr.PrinterName ASC"

        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryDeviceFowst() As String
        Dim strKeyWord As String
        Dim pFowstID As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        strKeyWord = Common.WebRequest.Data("KeyWord")

        pFowstID = Common.WebRequest.Data("FOWSTID")

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "devfowst.ID"
                    .Searchable = False
                End With
                With .Add("DeviceID", SqlDbType.UniqueIdentifier)
                    .Expression = "devfowst.DeviceID"
                End With
                With .Add("PhoneName", SqlDbType.VarChar)
                    .Expression = "dev.PhoneName"
                End With
                With .Add("FowstID", SqlDbType.UniqueIdentifier)
                    .Expression = "devfowst.FowstID"
                End With
                With .Add("FOWST", SqlDbType.VarChar)
                    .Expression = "fowst.Name"
                End With
                With .Add("POSRegistrationNo", SqlDbType.VarChar)
                    .Expression = "devfowst.POSRegistrationNo"
                End With
                With .Add("TerminalNo", SqlDbType.VarChar)
                    .Expression = "devfowst.TerminalNo"
                End With
            End With
            .SqlFrom = "FROM LibraryDeviceFowst devfowst " &
                " INNER JOIN LibraryDevice dev ON (devfowst.DeviceID = dev.ID) " &
                " INNER JOIN MobizLibraryOrganizationFOWST fowst ON (devfowst.FowstID = fowst.ID) "

            .SqlWhere = " Where devfowst.FowstID = " & ReadyString(pFowstID)
            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY dev.PhoneName ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryDevicePolicyProfile() As String
        Dim strKeyWord As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        strKeyWord = Common.WebRequest.Data("KeyWord")

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dev.DevicePolicyProfileID"
                End With
                With .Add("DevicePolicyProfileName", SqlDbType.VarChar)
                    .Expression = "dev.DevicePolicyProfileName"
                End With
                With .Add("DevicePolicyDescription", SqlDbType.VarChar)
                    .Expression = "dev.DevicePolicyDescription"
                End With
            End With
            .SqlFrom = "FROM LibraryDevicePolicyProfile dev"
            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY dev.DevicePolicyProfileName ASC"
        End With

        Return SubmitFinder()

    End Function



    <HttpPost()>
    Public Function LibraryDeviceApproval() As String
        Dim strKeyWord As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        strKeyWord = Common.WebRequest.Data("KeyWord")

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                With .Add("PhoneName", SqlDbType.VarChar)
                    .Expression = "dev.PhoneName"
                End With
                With .Add("PhoneModel", SqlDbType.VarChar)
                    .Expression = "dev.PhoneModel"
                End With
                With .Add("IMEI", SqlDbType.VarChar)
                    .Expression = "dev.IMEI"
                End With
                With .Add("UUID", SqlDbType.VarChar)
                    .Expression = "dev.UUID"
                End With
                With .Add("OS", SqlDbType.VarChar)
                    .Expression = "dev.OS"
                End With
                With .Add("DeviceNo", SqlDbType.Int)
                    .Expression = "dev.DeviceNo"
                End With
            End With
            .SqlFrom = "FROM LibraryDevice dev"
            .SqlWhere = "Where IsActive=0 "
            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY dev.PhoneName ASC"
        End With

        Return SubmitFinder()

    End Function



    <HttpPost()>
    Public Function LibraryDevicePolicyAppoint() As String
        Dim strKeyWord As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        strKeyWord = Common.WebRequest.Data("KeyWord")

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                With .Add("PhoneName", SqlDbType.VarChar)
                    .Expression = "dev.PhoneName"
                End With
                With .Add("PhoneModel", SqlDbType.VarChar)
                    .Expression = "dev.PhoneModel"
                End With
                With .Add("IMEI", SqlDbType.VarChar)
                    .Expression = "dev.IMEI"
                End With
                With .Add("UUID", SqlDbType.VarChar)
                    .Expression = "dev.UUID"
                End With
                With .Add("OS", SqlDbType.VarChar)
                    .Expression = "dev.OS"
                End With
                With .Add("DeviceNo", SqlDbType.Int)
                    .Expression = "dev.DeviceNo"
                End With
            End With
            .SqlFrom = "FROM LibraryDevice dev"
            .SqlWhere = "Where IsActive=1 "
            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY dev.PhoneName ASC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function TransLastestOrderedStockByCustomer() As String

        Dim pCustomerID As String
        Dim pDateRangeType As String
        Dim pWhere As String
        Dim pDateRange As DateRange
        Dim pDT As DataTable
        Dim pJson As JsonDictionary

        pCustomerID = Common.WebRequest.Data("CustomerID")
        If IsGuid(pCustomerID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid customer ID")
        End If

        pDateRangeType = Common.WebRequest.Data("DateRangeType")
        Select Case pDateRangeType
            Case "Within last week"
                Client.GetWithinLastWeek(pDateRange)

            Case "Within the last 2 weeks"
                Client.GetWithinLast2Weeks(pDateRange)

            Case "Within the last 4 weeks"
                Client.GetWithinLast4Weeks(pDateRange)

            Case "Within the last 3 months"
                Client.GetWithinLast3Months(pDateRange)

            Case "Within the last 6 months"
                Client.GetWithinLast6Months(pDateRange)

            Case "Within last year"
                Client.GetWithinLastYear(pDateRange)

            Case "Week to date"
                Client.GetWtdDates(pDateRange, Nothing)

            Case "Month to date"
                Client.GetMtdDates(pDateRange, Nothing)

            Case "Quarter to date"
                Client.GetQtdDates(pDateRange, Nothing)

            Case "Year to date"
                Client.GetYtdDates(pDateRange, Nothing)

            Case Else
                Return JsonDictionary.Serialize("Error", "Invalid DateRangeType: " & pDateRangeType)

        End Select
        pWhere = " AND DrpCacheStockSalesDailyByCustomer.TransDate BETWEEN " & ReadyString(pDateRange.DateFrom) & " AND " & ReadyString(pDateRange.DateTo)

        With CreateFinder()
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "LibraryStockInformation.ID"
                End With
                With .Add("StockNumber", SqlDbType.VarChar)
                    .Expression = "LibraryStockInformation.StockNumber"
                End With
                With .Add("StockDescription", SqlDbType.VarChar)
                    .Expression = "ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)), '') +" & vbCrLf & _
                        " 	ISNULL(', ' + LTRIM(RTRIM(LibraryProducts.ProductName)), ' ,') +" & vbCrLf & _
                        " 	ISNULL(', ' + LTRIM(RTRIM(LibraryStockInformation.Make)), ' ,') +" & vbCrLf & _
                        " 	ISNULL(', ' + LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ' ,') +" & vbCrLf & _
                        " 	ISNULL(', ' + LTRIM(RTRIM(LibraryStockInformation.Content)), ', ')" & vbCrLf
                End With
                With .Add("TransDate", SqlDbType.DateTime2)
                    .Expression = "MAX(DrpCacheStockSalesDailyByCustomer.TransDate)"
                    .Searchable = False
                End With
                With .Add("Subtotal", SqlDbType.Money)
                    .Expression = "SUM(DrpCacheStockSalesDailyByCustomer.Subtotal)"
                End With
            End With
            .SqlFrom = "FROM DrpCacheStockSalesDailyByCustomer" & vbCrLf & _
                " JOIN LibraryStockInformation ON LibraryStockInformation.ID = DrpCacheStockSalesDailyByCustomer.StockID" & vbCrLf & _
                " JOIN LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID" & vbCrLf & _
                " JOIN LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID" & vbCrLf
            .SqlGroup = "GROUP BY " & .Fields("ID").Expression & vbCrLf &
                "," & .Fields("StockNumber").Expression & vbCrLf &
                "," & .Fields("StockDescription").Expression
            .SqlWhere = "WHERE DrpCacheStockSalesDailyByCustomer.CustomerID = " & ReadyString(pCustomerID) & vbCrLf &
                pWhere
            .SqlOrder = "ORDER BY " & .Fields("TransDate").Expression & " DESC" & vbCrLf &
                "," & .Fields("StockDescription").Expression
        End With
        pDT = mFinder.Requery

        pJson = New JsonDictionary
        pJson.Add("Results", pDT)
        pJson.Add("DateRange", DisplayableDateRange(pDateRange))

        Return pJson.Serialize

    End Function

    <HttpPost()>
    Public Function LibraryPIMProduct() As String

        Dim pSql As String
        Dim pWhere As String
        Dim pSearchMore As Boolean

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        pSearchMore = CInt2(WebRequest.Instance.Data("IsSearchMore")) <> 0

        With CreateFinder()
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "pimpro.ID"
                End With
                With .Add("ProductGroupNo", SqlDbType.VarChar)
                    .Expression = "pimpro.ProductGroupNo"
                End With
                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "pimpro.ProductNo"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "pimpro.ProductName"
                End With
                With .Add("ProductSearchString", SqlDbType.VarChar)
                    .Expression = " ISNULL(pimpro.ProductSearchString,'') "
                End With
                With .Add("PublishedBy", SqlDbType.VarChar)
                    .Searchable = False
                    .Expression = "pub.CompanyName"
                End With
                With .Add("UploadedBy", SqlDbType.VarChar)
                    .Searchable = False
                    .Expression = "upl.CompanyName"
                End With
                With .Add("IsBrandOwner", SqlDbType.Bit)
                    .Searchable = False
                    .Expression = "pimpro.IsBrandOwner"
                End With
                If pSearchMore Then
                    With .Add("AllowDecimal", SqlDbType.Bit)
                        .Searchable = False
                        .Expression = "0"
                    End With
                Else
                    With .Add("AllowDecimal", SqlDbType.Bit)
                        .Searchable = False
                        .Expression = "ISNULL(orgpim.AllowDecimal, 0)"
                    End With
                End If

            End With

            If pSearchMore Then
                .SqlFrom = " FROM MobizLibraryPIMProduct pimpro " &
                            " LEFT JOIN MobizLibraryOrganization pub ON (pimpro.PublishedByID = pub.ID) " &
                            " LEFT JOIN MobizLibraryOrganization upl ON (pimpro.UploadedByID = upl.ID) "
            Else
                .SqlFrom = " FROM MobizLibraryPIMProduct pimpro " &
                            " INNER JOIN LibraryOrganizationPIM orgpim ON (pimpro.ID = orgpim.ProductID) " &
                            " LEFT JOIN MobizLibraryOrganization pub ON (pimpro.PublishedByID = pub.ID) " &
                            " LEFT JOIN MobizLibraryOrganization upl ON (pimpro.UploadedByID = upl.ID) "
            End If

            If (CBool2(WebRequest.Instance.Data("WithContentsOnly"))) Then
                .SqlFrom &= " JOIN MobizLibraryPIMProductContents ON MobizLibraryPIMProductContents.ParentProductID = pimpro.ID " &
                " AND MobizLibraryPIMProductContents.OrganizationID = " & ReadyString(Common.WebRequest.Data("OrgID"))
            End If

            .SqlOrder = " ORDER BY pimpro.IsBrandOwner DESC, pimpro.ProductName ASC "

            pWhere = " WHERE pimpro.IsActive = 1 " & vbCrLf &
                " AND ISNULL(pimpro.ProductName, '') <> ''" & vbCrLf
            If CStr2(mFinder.SearchString).Trim <> "" Then
                pWhere &= " AND (" & .CreateWhere & " OR " & vbCrLf &
                    " pimpro.ID IN" & vbCrLf &
                    "   (SELECT MobizLibraryPIMProductMeasurement.ProductID" & vbCrLf & _
                    "   FROM MobizLibraryPIMProductMeasurementBarcode" & vbCrLf & _
                    "   JOIN MobizLibraryPIMProductMeasurement ON MobizLibraryPIMProductMeasurement.ID = MobizLibraryPIMProductMeasurementBarcode.StockMeasurementsID" & vbCrLf & _
                    "   WHERE MobizLibraryPIMProductMeasurementBarcode.ReadyBarcode = " & ReadyString(ReadyBarcode(mFinder.SearchString)) & vbCrLf & _
                    "))"
            End If
            pSql = .CreateSql(pWhere)
        End With

        Return SubmitFinder(pSql)

    End Function

    <HttpPost()>
    Public Function LibraryPIMProductPublic() As String

        Dim pSql As String
        Dim pWhere As String
        Dim pSearchMore As Boolean

        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        pSearchMore = CInt2(WebRequest.Instance.Data("IsSearchMore")) <> 0

        With CreateFinder()
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "pimpro.ID"
                End With
                With .Add("ProductGroupNo", SqlDbType.VarChar)
                    .Expression = "pimpro.ProductGroupNo"
                End With
                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "pimpro.ProductNo"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "pimpro.ProductName"
                End With
                With .Add("ProductSearchString", SqlDbType.VarChar)
                    .Expression = " ISNULL(pimpro.ProductSearchString,'') "
                End With
                With .Add("PublishedBy", SqlDbType.VarChar)
                    .Searchable = False
                    .Expression = "pub.CompanyName"
                End With
                With .Add("UploadedBy", SqlDbType.VarChar)
                    .Searchable = False
                    .Expression = "upl.CompanyName"
                End With
                With .Add("IsBrandOwner", SqlDbType.Bit)
                    .Searchable = False
                    .Expression = "pimpro.IsBrandOwner"
                End With
                With .Add("AllowDecimal", SqlDbType.Bit)
                    .Searchable = False
                    .Expression = "0"
                End With

            End With

            .SqlFrom = " FROM MobizLibraryPIMProduct pimpro " &
            " LEFT JOIN MobizLibraryOrganization pub ON (pimpro.PublishedByID = pub.ID) " &
            " LEFT JOIN MobizLibraryOrganization upl ON (pimpro.UploadedByID = upl.ID) "

            pWhere = " WHERE pimpro.IsActive = 1 " & vbCrLf &
                " AND pimpro.ID NOT IN (SELECT ProductID FROM LibraryOrganizationPIM) " & vbCrLf &
                " AND ISNULL(pimpro.ProductName, '') <> ''" & vbCrLf

            If CStr2(mFinder.SearchString).Trim <> "" Then
                pWhere &= " AND (" & .CreateWhere & " OR " & vbCrLf &
                    " pimpro.ID IN" & vbCrLf &
                    "   (SELECT MobizLibraryPIMProductMeasurement.ProductID" & vbCrLf & _
                    "   FROM MobizLibraryPIMProductMeasurementBarcode" & vbCrLf & _
                    "   JOIN MobizLibraryPIMProductMeasurement ON MobizLibraryPIMProductMeasurement.ID = MobizLibraryPIMProductMeasurementBarcode.StockMeasurementsID" & vbCrLf & _
                    "   WHERE MobizLibraryPIMProductMeasurementBarcode.ReadyBarcode = " & ReadyString(ReadyBarcode(mFinder.SearchString)) & vbCrLf & _
                    "))"
            End If

            .SqlOrder = " ORDER BY pimpro.IsBrandOwner DESC, pimpro.ProductName ASC "

            pSql = .CreateSql(pWhere)
        End With

        Return SubmitFinder(pSql)

    End Function

    <HttpPost()>
    Public Function LibraryFowstBin() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            If CInt(WebRequest.Instance.Data("NoFilter")) Then
                mFinder.MaxRecords = 0
            End If
            With .Fields
                With .Add("ID", SqlDbType.Int)
                    .Searchable = False
                End With
                .Add("Name", SqlDbType.VarChar)
                .Add("Description", SqlDbType.VarChar)
            End With
            .SqlFrom = "FROM LibraryFowstBin"
            .SqlWhere = "WHERE Name<>'Default Bin' AND FowstID = " & ReadyString(IIf(Common.WebRequest.Data("FowstID") = vbNullString, WebSession.Instance.FowstID, Common.WebRequest.Data("FowstID")))
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryFowstBinProduct() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.Int)
                    .Searchable = False
                    .Expression = "bin.ID"
                End With

                With .Add("Name", SqlDbType.VarChar)
                    .Expression = "bin.Name"
                End With

                With .Add("Description", SqlDbType.VarChar)
                    .Expression = "bin.Description"
                End With

                With .Add("MinPerProduct", SqlDbType.VarChar)
                    .Expression = "binpro.MinPerProduct"
                End With

                With .Add("MaxPerProduct", SqlDbType.VarChar)
                    .Expression = "binpro.MaxPerProduct"
                End With
            End With
            .SqlFrom = "FROM dbo.LibraryFowstBin bin " &
                " INNER JOIN dbo.LibraryFowstBinProduct binpro ON (bin.ID = binpro.BinID)"

            .SqlWhere = "WHERE binpro.FowstID = " & ReadyString(Common.WebRequest.Data("FowstID")) &
                        " AND binpro.BinID = " & ReadyString(Common.WebRequest.Data("BinID")) &
                        " AND binpro.ProductID = " & ReadyString(Common.WebRequest.Data("ProductID"))
        End With

        Return SubmitFinder()

    End Function

























    <HttpPost()>
    Public Function TransSalesOrder() As String

        Dim pCustomerID As String

        pCustomerID = Common.WebRequest.Data("CustomerID")
        If IsGuid(pCustomerID) = False Then
            Return JsonDictionary.Serialize("Error", "Invalid Customer ID")
        End If

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "TransSalesOrder.ID"
                End With
                With .Add("OrderNo", SqlDbType.VarChar)
                    .Expression = "TransSalesOrder.OrderNo"
                End With
                With .Add("LastUpdated", SqlDbType.DateTime)
                    .Expression = "TransSalesOrder.LastUpdated"
                End With
                With .Add("Total", SqlDbType.Money)
                    .Expression = "tblItems.Total"
                End With
                With .Add("SkuCount", SqlDbType.Int)
                    .Expression = "tblItems.SkuCount"
                End With
            End With
            .SqlFrom = " FROM TransSalesOrder" & vbCrLf
            .SqlFrom &= " OUTER APPLY (" & vbCrLf & _
                " 	SELECT SUM(TransSalesOrderItems.SubTotal) AS Total" & vbCrLf & _
                "   ,COUNT(DISTINCT TransSalesOrderItems.StockID) AS SkuCount" & vbCrLf & _
                " 	FROM TransSalesOrderItems" & vbCrLf & _
                "   WHERE TransSalesOrderItems.SalesOrderID = TransSalesOrder.ID" & vbCrLf & _
                "   AND TransSalesOrderItems.Quantity > 0" & vbCrLf &
                " ) tblItems    " & vbCrLf
            .SqlWhere = " WHERE TransSalesOrder.CustomerID = " & ReadyString(pCustomerID) & vbCrLf & _
                " AND TransSalesOrder.IsHold = 1" & vbCrLf & _
                " AND TransSalesOrder.IsCancelled = 0" & vbCrLf
            .SqlOrder = "ORDER BY TransSalesOrder.LastUpdated DESC" & vbCrLf
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function TransInvoices() As String

        'If WebSession.Instance.UserAccess Is Nothing _
        '    OrElse (WebSession.Instance.UserAccess.SalesOrder_SalesOrder.IsEdit _
        '            OrElse WebSession.Instance.UserAccess.SalesOrder_SalesOrder.IsEnable _
        '            OrElse WebSession.Instance.UserAccess.PointOfSales_PointOfSales.IsEnable _
        '    ) = False Then

        '    Return JsonDictionary.Serialize("Error", "Access denied")
        'End If

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        Dim pCustomerID As String
        Dim pNonZero As Boolean

        pCustomerID = Common.WebRequest.Data("CustomerID")
        pNonZero = CDec2(Common.WebRequest.Data("NonZero"))

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInvoice.ID"
                    .Searchable = True
                End With
                With .Add("CustomerID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInvoice.CustomerID"
                    .Searchable = True
                End With
                With .Add("InvoiceNum", SqlDbType.VarChar)
                    .Expression = "RIGHT('0000000' + CONVERT(varchar, TransInvoice.InvoiceNumber), 7)"
                End With
                With .Add("SalesOrderNoRemoveDash", SqlDbType.VarChar) 'additional by janine search sales order no remove dash
                    .Expression = "CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + CONVERT(varchar, LibraryDevice.DeviceNo) + CONVERT(varchar, TransInvoice.SalesOrderNo)"
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = " (CASE MobizLibraryOrganization.IsOrganization " &
                                  "     WHEN 1 THEN  MobizLibraryOrganization.CompanyName " &
                                  "     ELSE  " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonSurname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonFirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonMiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryOrganization.PersonSuffixName)),', '), '') " &
                                  " END)"
                End With
                With .Add("InvoiceDate", SqlDbType.DateTime)
                    .Expression = "TransInvoice.InvoiceDate"
                End With
                With .Add("Total", SqlDbType.Money)
                    .Expression = "tblItems.Total"
                    .Searchable = False
                End With
                With .Add("SkuCount", SqlDbType.Int)
                    .Expression = "tblItems.SkuCount"
                    .Searchable = False
                End With
                With .Add("TotalOrder", SqlDbType.Money)
                    .Expression = "tblItemsOrder.TotalOrder"
                    .Searchable = False
                End With
                With .Add("SkuCountOrder", SqlDbType.Int)
                    .Expression = "tblItemsOrder.SkuCountOrder"
                    .Searchable = False
                End With
                With .Add("SalesOrderNo", SqlDbType.Int)
                    .Expression = "ISNULL(SalesOrderNo,0)"
                    .Searchable = False
                End With
                With .Add("FOWSTName", SqlDbType.VarChar)
                    .Expression = "MobizLibraryOrganizationFOWST.Name"
                End With
                With .Add("DeviceName", SqlDbType.VarChar)
                    .Expression = "LibraryDevice.PhoneName"
                End With
                With .Add("UserID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInvoice.UserID"
                    .Searchable = False
                End With
                With .Add("EmployeeFullName", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPeople.Surname + ', ' + MobizLibraryPeople.FirstName"
                    .Searchable = False
                End With
                With .Add("CurrentUserID", SqlDbType.UniqueIdentifier)
                    .Expression = ReadyString(WebSession.Instance.PeopleUserID)
                    .Searchable = False
                End With
                With .Add("MIN", SqlDbType.VarChar)
                    .Expression = "CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo)"
                End With
                With .Add("TableNo", SqlDbType.VarChar)
                    .Expression = "TransInvoice.CustomerTableNo"
                End With
                With .Add("SearchSO", SqlDbType.VarChar)
                    .Expression = "CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo) + '-' +  CONVERT(varchar, ISNULL(SalesOrderNo,0)) "
                End With
                With .Add("DeliveryMethod", SqlDbType.VarChar)
                    .Expression = "TransInvoice.DeliveryMethod"
                End With
                With .Add("OrderMethod", SqlDbType.VarChar)
                    .Expression = "TransInvoice.OrderMethod"
                End With
                With .Add("CustomerDemographicGender", SqlDbType.VarChar)
                    .Expression = "TransInvoice.CustomerDemographicGender"
                End With
                With .Add("CustomerDemographicAge", SqlDbType.VarChar)
                    .Expression = "TransInvoice.CustomerDemographicAge"
                End With
            End With
            '.SqlFrom = " FROM TransInvoice" & vbCrLf & _
            '    " JOIN MobizLibraryOrganization ON TransInvoice.CustomerID = MobizLibraryOrganization.ID" & vbCrLf
            .SqlFrom = " FROM TransInvoice INNER JOIN" & vbCrLf & _
                      " MobizLibraryPeople ON TransInvoice.UserID = MobizLibraryPeople.ID INNER JOIN" & vbCrLf & _
                      " LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID INNER JOIN" & vbCrLf & _
                      " MobizLibraryOrganizationFOWST ON TransInvoice.FowstID = MobizLibraryOrganizationFOWST.ID" & vbCrLf & _
                      " JOIN MobizLibraryOrganization ON TransInvoice.CustomerID = MobizLibraryOrganization.ID"
            .SqlFrom &= " OUTER APPLY (" & vbCrLf & _
                " 	SELECT SUM(TransInvoiceItems.SubTotal) AS Total" & vbCrLf & _
                "   ,COUNT(DISTINCT TransInvoiceItems.ProductID) AS SkuCount" & vbCrLf & _
                " 	FROM TransInvoiceItems" & vbCrLf & _
                "   WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf & _
                "   AND TransInvoiceItems.Quantity > 0" & vbCrLf &
                " ) tblItems    " & vbCrLf
            'additional by janine sept 5 2014 for sales order taker
            .SqlFrom &= " OUTER APPLY (" & vbCrLf & _
               " 	SELECT SUM(TransInvoiceItems.SubTotal) AS TotalOrder" & vbCrLf & _
               "   ,COUNT(DISTINCT TransInvoiceItems.ProductID) AS SkuCountOrder" & vbCrLf & _
               " 	FROM TransInvoiceItems" & vbCrLf & _
               "   WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf & _
               "   AND TransInvoiceItems.OrderedQuantity > 0" & vbCrLf &
               " ) tblItemsOrder    " & vbCrLf
            .SqlWhere = " WHERE TransInvoice.IsCancelled = 0" & vbCrLf &
                " AND TransInvoice.InvoiceNumber " & If(pNonZero, "<>", "=") & " 0" & vbCrLf &
                " AND EXISTS(SELECT TransInvoiceItems.ID" & vbCrLf & _
                "   FROM TransInvoiceItems" & vbCrLf & _
                "   WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID)" & vbCrLf
            If IsGuid(pCustomerID) Then
                .SqlWhere &= " AND TransInvoice.CustomerID = " & ReadyString(ReadyGuid(pCustomerID)) & vbCrLf
            End If
            .SqlOrder = " ORDER BY TransInvoice.InvoiceDate DESC" & vbCrLf &
                ",TransInvoice.LastUpdated DESC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function TransInvoicesForReprint() As String
        Dim strFOWSTID As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        strFOWSTID = Common.WebRequest.Data("FOWSTID")

        With CreateFinder()
            .MaxRecords = 5
            .SearchString = ""
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInvoice.ID"
                    .Searchable = True
                End With
                With .Add("CustomerID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInvoice.CustomerID"
                    .Searchable = True
                End With
                With .Add("InvoiceNum", SqlDbType.VarChar)
                    .Expression = "RIGHT('0000000' + CONVERT(varchar, TransInvoice.InvoiceNumber), 7)"
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = " (CASE MobizLibraryOrganization.IsOrganization " &
                                  "     WHEN 1 THEN  MobizLibraryOrganization.CompanyName " &
                                  "     ELSE  " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonSurname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonFirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonMiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryOrganization.PersonSuffixName)),', '), '') " &
                                  " END)"
                End With
                With .Add("InvoiceDate", SqlDbType.DateTime)
                    .Expression = "TransInvoice.InvoiceDate"
                End With
                With .Add("Total", SqlDbType.Money)
                    .Expression = "(SELECT SUM(SubTotal) FROM TransInvoiceItems WHERE InvoiceID = TransInvoice.ID)"
                    .Searchable = False
                End With
                With .Add("SkuCount", SqlDbType.Int)
                    .Expression = "(SELECT COUNT(ID) FROM TransInvoiceItems WHERE InvoiceID = TransInvoice.ID)"
                    .Searchable = False
                End With
                With .Add("SalesOrderNo", SqlDbType.Int)
                    .Expression = "ISNULL(SalesOrderNo,0)"
                    .Searchable = False
                End With
                With .Add("FOWSTName", SqlDbType.VarChar)
                    .Expression = "MobizLibraryOrganizationFOWST.Name"
                End With
                With .Add("DeviceName", SqlDbType.VarChar)
                    .Expression = "LibraryDevice.PhoneName"
                End With
                With .Add("UserID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInvoice.UserID"
                    .Searchable = False
                End With
                With .Add("EmployeeFullName", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPeople.Surname + ', ' + MobizLibraryPeople.FirstName"
                    .Searchable = False
                End With
                With .Add("CurrentUserID", SqlDbType.UniqueIdentifier)
                    .Expression = ReadyString(WebSession.Instance.PeopleUserID)
                    .Searchable = False
                End With
                With .Add("MIN", SqlDbType.VarChar)
                    .Expression = "CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo) + '-' + CONVERT(varchar, LibraryDevice.DeviceNo)"
                    .Searchable = False
                End With
                With .Add("TableNo", SqlDbType.VarChar)
                    .Expression = "TransInvoice.CustomerTableNo"
                End With
            End With
            .SqlFrom = " FROM TransInvoice INNER JOIN" & vbCrLf & _
                      " MobizLibraryPeople ON TransInvoice.UserID = MobizLibraryPeople.ID INNER JOIN" & vbCrLf & _
                      " LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID INNER JOIN" & vbCrLf & _
                      " MobizLibraryOrganizationFOWST ON TransInvoice.FowstID = MobizLibraryOrganizationFOWST.ID" & vbCrLf & _
                      " JOIN MobizLibraryOrganization ON TransInvoice.CustomerID = MobizLibraryOrganization.ID "
            .SqlWhere = " WHERE TransInvoice.IsCancelled = 0 " & vbCrLf &
                " AND TransInvoice.InvoiceNumber > 0 " & vbCrLf &
                " AND TransInvoice.UserID = " & ReadyString(WebSession.Instance.PeopleUserID) & vbCrLf &
                " AND TransInvoice.FowstID = " & ReadyString(strFOWSTID)

            .SqlOrder = " ORDER BY TransInvoice.InvoiceDate DESC" & vbCrLf &
                ",TransInvoice.LastUpdated DESC"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function TransInvoicesReturnables(Optional ByVal vIsAllDeviceInFowst As Boolean = False) As String
        Dim pDT As DataTable

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        Dim pCustomerID As String
        Dim pExcludeMemoID As String
        Dim pWhereMemo As String

        pCustomerID = Common.WebRequest.Data("CustomerID")
        pExcludeMemoID = Common.WebRequest.Data("MemoID")
        If IsGuid(pExcludeMemoID) Then
            pExcludeMemoID = ReadyGuid(pExcludeMemoID)
            pWhereMemo = " AND TransMemo.ID <> " & ReadyString(pExcludeMemoID) & vbCrLf
        End If


        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "TransInvoice.ID"
                    .Searchable = False
                End With
                With .Add("InvoiceNum", SqlDbType.VarChar)
                    ' 12 chars max kay 12 chars max pud sa POS print
                    .Expression = "RIGHT('000000000000' + CONVERT(varchar, TransInvoice.InvoiceNumber), 12)"
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = " (CASE MobizLibraryOrganization.IsOrganization " &
                                  "     WHEN 1 THEN  MobizLibraryOrganization.CompanyName " &
                                  "     ELSE  " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonSurname)) + ', ',', '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonFirstName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonMiddleName)) + ' ',' '), '') + " &
                                  "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryOrganization.PersonSuffixName)),', '), '') " &
                                  " END)"
                End With
                With .Add("InvoiceDate", SqlDbType.DateTime)
                    .Expression = "TransInvoice.InvoiceDate"
                End With
                With .Add("Total", SqlDbType.Money)
                    .Expression = "SUM(tblItems.Total)"
                    .Searchable = False
                End With
                With .Add("SkuCount", SqlDbType.Int)
                    .Expression = "SUM(tblItems.SkuCount)"
                    .Searchable = False
                End With
                With .Add("Returnables", SqlDbType.Int)
                    .Expression = "SUM(tblItems.Returnables)"
                    .Searchable = False
                End With
                With .Add("FOWSTName", SqlDbType.VarChar)
                    .Expression = "MobizLibraryOrganizationFOWST.Name"
                End With
                With .Add("DeviceName", SqlDbType.VarChar)
                    .Expression = "LibraryDevice.PhoneName"
                End With
                With .Add("OrderByFowst", SqlDbType.Int)
                    .Expression = "	CASE TransInvoice.FOWSTID " &
                        " WHEN " & ReadyString(WebRequest.Instance.FowstID) & " THEN 0 " &
                        " ELSE " &
                        "   1 " &
                        " END "
                End With
                'commented by janine nov 24 2014 . do not check device.
                'With .Add("OrderByDevice", SqlDbType.Int)
                '    .Expression = "	CASE TransInvoice.DeviceID " &
                '        " WHEN " & ReadyString(WebRequest.Instance.DeviceID) & " THEN 0 " &
                '        " ELSE " &
                '        "   1 " &
                '        " END "
                'End With
            End With
            .SqlFrom = " FROM TransInvoice" & vbCrLf & _
                " INNER JOIN MobizLibraryOrganizationFOWST ON TransInvoice.FowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
                " INNER JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID " & vbCrLf &
                " JOIN MobizLibraryOrganization ON TransInvoice.CustomerID = MobizLibraryOrganization.ID" & vbCrLf &
                " " & vbCrLf & _
                " CROSS APPLY" & vbCrLf & _
                " 	(SELECT SUM(TransInvoiceItems.SubTotal) AS Total" & vbCrLf & _
                " 	,COUNT(DISTINCT TransInvoiceItems.ProductID) AS SkuCount" & vbCrLf & _
                "   ,SUM(TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity) - ISNULL(tblReturns.QuantityReturned, 0) AS Returnables" & vbCrLf & _
                "   FROM TransInvoiceItems" & vbCrLf & _
                "   " & vbCrLf & _
                " 	OUTER APPLY " & vbCrLf & _
                "     	(SELECT SUM(TransMemoSalesReturn.QuantityReturned * TransMemoSalesReturn.ActualQuantity) AS QuantityReturned" & vbCrLf & _
                "     	FROM TransMemoSalesReturn" & vbCrLf & _
                "       JOIN TransMemo ON TransMemo.ID = TransMemoSalesReturn.MemoID" & vbCrLf & _
                " 		WHERE TransMemo.IsCancelled = 0" & vbCrLf & _
                "       AND TransMemoSalesReturn.ProductID = TransInvoiceItems.ProductID" & vbCrLf & _
                "       AND TransMemoSalesReturn.InvoiceID = TransInvoiceItems.InvoiceID" & vbCrLf & _
                "       AND TransMemo.ApprovedBy IS NOT NULL" & vbCrLf & _
                pWhereMemo &
                " 	) AS tblReturns" & vbCrLf & _
                "   " & vbCrLf & _
                " 	WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf & _
                "   AND TransInvoiceItems.Quantity > 0" & vbCrLf & _
                " 	GROUP BY tblReturns.QuantityReturned" & vbCrLf & _
                "   HAVING SUM(TransInvoiceItems.Quantity * TransInvoiceItems.ActualQuantity) > ISNULL(tblReturns.QuantityReturned, 0)" & vbCrLf & _
                " ) AS tblItems" & vbCrLf & _
                " " & vbCrLf

            .SqlWhere = " WHERE TransInvoice.IsCancelled = 0" & vbCrLf & _
                " AND TransInvoice.InvoiceNumber <> 0 " & vbCrLf & _
                " AND tblItems.Returnables > 0" & vbCrLf & _
                " AND TransInvoice.FOWSTID = " & ReadyString(WebRequest.Instance.FowstID)
            'commented by janine nov 24 2014 . do not check device.
            ''   IIf(Not vIsAllDeviceInFowst, " AND TransInvoice.DeviceID = " & ReadyString(WebRequest.Instance.DeviceID), "")

            'If (CStr2(Common.WebRequest.Data("keywords")) <> "") Then
            '    .SqlWhere &= " AND TransInvoice.InvoiceNumber = " & ReadyString(Common.WebRequest.Data("keywords"))
            'End If

            If IsGuid(pCustomerID) Then
                .SqlWhere &= " AND TransInvoice.CustomerID = " & ReadyString(ReadyGuid(pCustomerID)) & vbCrLf
            End If
            .SqlGroup = " GROUP BY TransInvoice.ID " & vbCrLf & _
                " ," & .Fields("InvoiceNum").Expression & vbCrLf & _
                " , (CASE MobizLibraryOrganization.IsOrganization " &
                "     WHEN 1 THEN  MobizLibraryOrganization.CompanyName " &
                "     ELSE  " &
                "         ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonSurname)) + ', ',', '), '') + " &
                "         ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonFirstName)) + ' ',' '), '') + " &
                "         ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryOrganization.PersonMiddleName)) + ' ',' '), '') + " &
                "         ISNULL(NULLIF(', ' + LTRIM(RTRIM(MobizLibraryOrganization.PersonSuffixName)),', '), '') " &
                " END)" & vbCrLf & _
                " , TransInvoice.FowstID" & vbCrLf & _
                " , TransInvoice.DeviceID" & vbCrLf & _
                " , TransInvoice.InvoiceDate" & vbCrLf & _
                " , MobizLibraryOrganizationFOWST.Name" & vbCrLf & _
                " , LibraryDevice.PhoneName" & vbCrLf & _
                " ,TransInvoice.LastUpdated " & vbCrLf

            .SqlOrder = " ORDER BY " & vbCrLf & _
                " (CASE TransInvoice.FOWSTID " &
                " WHEN " & ReadyString(WebRequest.Instance.FowstID) & " THEN 0 " &
                " ELSE " &
                "   1 " &
                " END) ASC, " & vbCrLf & _
                " TransInvoice.InvoiceDate DESC" & vbCrLf & _
                " ,TransInvoice.LastUpdated DESC" & vbCrLf
            'commented by janine nov 24 2014 . do not check device.
            '" (CASE TransInvoice.DeviceID " &
            '   " WHEN " & ReadyString(WebRequest.Instance.DeviceID) & " THEN 0 " &
            '   " ELSE " &
            '   "   1 " &
            '   " END) ASC, " & vbCrLf & _
        End With

        Return SubmitFinder()

        ' '' '''Change by Francis Adan
        ' '' ''' At first query, it is looking only on the current device, if no result then call again the function but this time all device of the fowst
        ' ''pDT = mFinder.Requery
        ' ''If (pDT.Rows.Count > 0) Then
        ' ''    Return JsonDictionary.Serialize("Results", pDT)
        ' ''Else
        ' ''    If (Not vIsAllDeviceInFowst) Then
        ' ''        TransInvoicesReturnables(True)
        ' ''    Else
        ' ''        Return JsonDictionary.Serialize("Results", pDT)
        ' ''    End If
        ' ''End If


    End Function

    <HttpPost()>
    Public Function MainSecurityLevel() As String

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.Int)
                    .Searchable = False
                End With
                With .Add("LevelType", SqlDbType.VarChar)
                    .Expression = "MainSecurityLevel.LevelType"
                End With
            End With
            .SqlFrom = "FROM MainSecurityLevel"
            .SqlWhere = "WHERE IsCancelled = 0"
            .SqlOrder = "ORDER BY MainSecurityLevel.LevelType"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Public Function LibraryBranch() As String

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.Int)
                    .Searchable = False
                End With
                With .Add("BranchName", SqlDbType.VarChar)
                    .Expression = "tblBranch.Address"
                End With
            End With
            .SqlFrom = "FROM DavaoUnifiedPayroll.dbo.LibraryBranch tblBranch"
            .SqlOrder = "ORDER BY BranchName"
        End With

        Return SubmitFinder()

    End Function

    <HttpPost()>
    Function LibraryDefaultPIMFeature() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        With CreateFinder()
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.Int)
                    .Expression = "dbo.MobizLibraryCountriesLanguageLabel.ID"
                    .Searchable = False
                End With
                With .Add("Label", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryCountriesLanguageLabel.Label"
                End With
            End With
            .SqlFrom = "FROM dbo.MobizLibraryCountriesLanguageLabel INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryPIMFeature ON dbo.MobizLibraryCountriesLanguageLabel.ID = dbo.MobizLibraryPIMFeature.FeatureLabelID INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryPIMProductFeature INNER JOIN " & vbCrLf &
                      "dbo.MobizLibraryUNSPSCCommodityFeature ON dbo.MobizLibraryPIMProductFeature.CommodityFeatureID = dbo.MobizLibraryUNSPSCCommodityFeature.ID ON  " & vbCrLf &
                      "dbo.MobizLibraryPIMFeature.ID = dbo.MobizLibraryUNSPSCCommodityFeature.FeatureID INNER JOIN " & vbCrLf &
                      "dbo.LibraryOrganizationPIM ON dbo.MobizLibraryPIMProductFeature.ProductID = dbo.LibraryOrganizationPIM.ProductID "
            .SqlWhere = "WHERE MobizLibraryPIMFeature.IsDefaultFeature = 1"
        End With

        Return SubmitFinder()
    End Function

    <HttpPost()>
    Public Function LibraryCustomerFowst() As String
        Dim strOrgID As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

        With CreateFinder()
            .Distinct = True
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowst.ID"
                    .Searchable = False
                End With
                With .Add("FowstName", SqlDbType.UniqueIdentifier)
                    .Expression = "fowst.Name"
                    .Searchable = False
                End With
                With .Add("Address", SqlDbType.VarChar)
                    .Expression = "dbo.SplitJoinComma(ISNULL(fowst.StreetNumber, '') " &
                                    "+ ', ' + ISNULL(fowst.Street, '') " &
                                    "+ ', ' + ISNULL(cty.Name, '') + ', ' + ISNULL(ctry.Name, ''))"
                End With
            End With

            strOrgID = WebRequest.Instance.OrgID
            If (WebRequest.Instance.Data("OrganizationID") <> Nothing) Then
                If (WebRequest.Instance.Data("OrganizationID").Trim().Count > 0) Then
                    strOrgID = WebRequest.Instance.Data("OrganizationID").Trim()
                End If
            End If

            If CInt(WebRequest.Instance.Data("FilterFowst")) = 1 Then
                .SqlFrom = "FROM dbo.LibraryOrganizationEmployee INNER JOIN " &
                      "dbo.LibraryOrganizationEmployeeFOWST ON dbo.LibraryOrganizationEmployee.PeopleID = dbo.LibraryOrganizationEmployeeFOWST.PeopleID INNER JOIN " &
                      "dbo.MobizLibraryOrganizationFOWST AS fowst ON dbo.LibraryOrganizationEmployeeFOWST.FowstID = fowst.ID LEFT OUTER JOIN " &
                      "dbo.MobizLibraryCities AS cty ON fowst.FowstCityID = cty.ID LEFT OUTER JOIN " &
                      "dbo.MobizLibraryCountries AS ctry ON fowst.FowstCountryID = ctry.ID"
                .SqlWhere = " WHERE fowst.OrganizationID = " & ReadyString(strOrgID)
            Else
                .SqlFrom = "FROM dbo.MobizLibraryOrganizationFOWST fowst " &
                " LEFT OUTER JOIN dbo.MobizLibraryCities cty ON (fowst.FowstCityID = cty.ID) " &
                " LEFT OUTER JOIN dbo.MobizLibraryCountries ctry ON (fowst.FowstCountryID = ctry.ID)"

                .SqlWhere = " WHERE fowst.OrganizationID = " & ReadyString(strOrgID)
            End If

            .SqlOrder = "ORDER BY Address"
        End With

        Return SubmitFinder()
    End Function

    <HttpPost()>
    Public Function LibraryCustomerFowstBin() As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            If CInt(WebRequest.Instance.Data("NoFilter")) Then
                mFinder.MaxRecords = 0
            End If
            With .Fields
                With .Add("ID", SqlDbType.Int)
                    .Searchable = False
                End With
                .Add("Name", SqlDbType.VarChar)
                .Add("Description", SqlDbType.VarChar)
            End With
            .SqlFrom = "FROM LibraryOrganizationCustomerFowstBin"
            .SqlWhere = "WHERE FowstID = " & ReadyString(IIf(Common.WebRequest.Data("CustomerID") = vbNullString, WebSession.Instance.FowstID, Common.WebRequest.Data("CustomerID")))
        End With

        Return SubmitFinder()
    End Function

    <HttpPost()>
    Public Function LibraryWorkSchedule() As String
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.Int)
                    .Searchable = False
                End With
                .Add("WorkScheduleName", SqlDbType.VarChar)
                .Add("Description", SqlDbType.VarChar)
            End With
            .SqlFrom = "FROM LibraryOrganizationWorkSchedule"
        End With

        Return SubmitFinder()
    End Function

    <HttpPost()>
    Public Function LibraryFranchisee() As String
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        With CreateFinder()
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "fowst.ID"
                    .Searchable = False
                End With

                With .Add("FowstName", SqlDbType.Int)
                    .Expression = "fowst.Name"
                End With

                With .Add("Address", SqlDbType.Int)
                    .Expression = "dbo.SplitJoinComma(ISNULL(fowst.StreetNumber, '') " &
                                     "+ ', ' + ISNULL(fowst.Street, '') " &
                                     "+ ', ' + ISNULL(cty.Name, '') + ', ' + ISNULL(ctry.Name, ''))"
                End With

            End With
            .SqlFrom = "FROM MobizLibraryOrganizationFranchisee " & vbCrLf &
                        "inner join MobizLibraryOrganizationFOWST fowst on fowst.ID = MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID " & vbCrLf &
                        "LEFT OUTER JOIN dbo.MobizLibraryCities AS cty ON fowst.FowstCityID = cty.ID LEFT OUTER JOIN " & vbCrLf &
                        "dbo.MobizLibraryCountries AS ctry ON fowst.FowstCountryID = ctry.ID "

            .SqlWhere = "where MobizLibraryOrganizationFranchisee.IsActive = 1 AND FranchisorOrgID = " & ReadyString(Common.WebRequest.Data("OrgID"))
        End With

        Return SubmitFinder()
    End Function

    <HttpPost()>
    Public Function LibraryOrganizationProductPublisher() As String
        Dim strKeyWord As String

        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
        strKeyWord = Common.WebRequest.Data("KeyWord")

        With CreateFinder()
            .Distinct = True

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "org.ID"
                    .Searchable = False
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = "org.CompanyName"
                End With
                With .Add("NoOfProducts", SqlDbType.BigInt)
                    .Expression = "(SELECT COUNT(ID) FROM MobizLibraryPIMProduct WHERE PublishedByID = org.ID)"
                End With
                With .Add("LastUpdated", SqlDbType.DateTime)
                    .Expression = "(SELECT TOP 1 LastUpdated FROM MobizLibraryPIMProduct WHERE PublishedByID = org.ID ORDER BY LastUpdated DESC)"
                End With
            End With
            .SqlFrom = "FROM MobizLibraryPIMProduct pim INNER JOIN MobizLibraryOrganization org ON pim.PublishedByID = org.ID "
            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY org.CompanyName ASC"
        End With

        Return SubmitFinder()

    End Function


#Region " common "
    Private Function CreateFinder() As Finder

        'todo: add common security

        mFinder = New Finder
        mFinder.MaxRecords = 50
        mFinder.SearchString = Common.WebRequest.Data("keywords")
        mFinder.ProData = Common.WebRequest.ProData

        Return mFinder

    End Function

    ' think of another name
    Private Function SubmitFinder(Optional ByVal vSql As String = "") As String

        Dim pDT As DataTable

        If CStr2(vSql) > "" Then
            pDT = mFinder.ProData.Execute(vSql)
        Else
            pDT = mFinder.Requery
        End If


        Return JsonDictionary.Serialize("Results", pDT)

    End Function
#End Region
End Class
