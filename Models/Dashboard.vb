Public Class Dashboard
    Public Property PeopleID As String
    Public Property Product As String
    Public Property Category As String
    Public Property Title As String
    Public Property Message As String

    Function GetPeopleName() As DataTable

        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        Try
            With pFinder
                .ProData = Common.WebRequest.ProData

                With .Fields
                    With .Add("ID", SqlDbType.UniqueIdentifier)
                        .Searchable = False
                        .Expression = "ID"
                    End With

                    With .Add("FirstName", SqlDbType.VarChar)
                        .Expression = "FirstName"
                    End With

                    With .Add("MiddleName", SqlDbType.VarChar)
                        .Expression = "MiddleName"
                    End With

                    With .Add("Surname", SqlDbType.VarChar)
                        .Expression = "Surname"
                    End With

                End With

                .SqlFrom = "FROM MobizLibraryPeople where ID=" & ReadyString(PeopleID)

                pDT = .Requery
            End With

        Catch ex As Exception

        End Try

        Return pDT
    End Function

    Function SaveUpdateInfo() As String
        Dim p As New InsertUpdate("MobizLibraryUpdateLog")
        Dim pSql As String
        Dim strIDReturn As String = ""

        Try
            p("Product") = ReadyString(Product)
            p("Category") = ReadyString(Category)
            p("Title") = ReadyString(Title)
            p("Message") = ReadyString(Message)
            p("UserID") = ReadyString(PeopleID)
            p("LastUpdated") = "getdate()"

            strIDReturn = Guid.NewGuid.ToString()
            p("ID") = ReadyString(strIDReturn)
            pSql = p.GetInsert()

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Return "Error"
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function

    <HttpPost()>
    Function GetListOfBusiness() As DataTable
        Common.WebRequest.ProData.ExecuteSQL("Use MobizMobiz")
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 50

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryRegisterForUpdates.ID"
                End With

                With .Add("FirstName", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryRegisterForUpdates.FirstName"
                End With

                With .Add("LastName", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryRegisterForUpdates.LastName"
                End With

                With .Add("MiddleName", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryRegisterForUpdates.MiddleName"
                End With

                With .Add("BusinessName", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryRegisterForUpdates.BusinessName"
                End With

                With .Add("BusinessIndustry", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryIndustryClass.Class"
                End With

                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "c.Name"
                End With

                With .Add("City", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryCities.Name"
                End With

                With .Add("TelephoneNo", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryRegisterForUpdates.TelephoneNo"
                End With

                With .Add("CallingCode", SqlDbType.VarChar)
                    .Expression = "a.CallingCode"
                End With

                With .Add("AreaCode", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryAreaCodes.CallingCode"
                End With

                With .Add("MobileNo", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryRegisterForUpdates.MobileNo"
                End With

                With .Add("CountryCode", SqlDbType.VarChar)
                    .Expression = "b.CallingCode"
                End With

                With .Add("Prefix", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryTelcoPrefixes.Prefix"
                End With

                With .Add("EmailAddress", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryRegisterForUpdates.EmailAddress"
                End With

                With .Add("Product", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryRegisterForUpdates.MobizProduct"
                End With

                With .Add("LastUpdated", SqlDbType.Date)
                    .Expression = "dbo.LibraryRegisterForUpdates.LastUpdated"
                End With
            End With

            .SqlFrom = "FROM dbo.LibraryRegisterForUpdates LEFT JOIN " &
                      "dbo.MobizLibraryCities ON dbo.LibraryRegisterForUpdates.CityID = dbo.MobizLibraryCities.ID LEFT JOIN " &
                      "dbo.MobizLibraryCountries a ON dbo.LibraryRegisterForUpdates.CallingCodeTelID = a.ID LEFT JOIN " &
                      "dbo.MobizLibraryCountries b ON dbo.LibraryRegisterForUpdates.CallingCodeMobileID = b.ID LEFT JOIN " &
                      "dbo.MobizLibraryCountries c ON dbo.LibraryRegisterForUpdates.CountryID = c.ID LEFT JOIN " &
                      "dbo.MobizLibraryAreaCodes ON dbo.LibraryRegisterForUpdates.AreaCodeID = dbo.MobizLibraryAreaCodes.ID LEFT JOIN " &
                      "dbo.MobizLibraryTelcoPrefixes ON dbo.LibraryRegisterForUpdates.TelcoPrefixID = dbo.MobizLibraryTelcoPrefixes.ID LEFT JOIN " &
                      "dbo.MobizLibraryIndustryClass ON dbo.LibraryRegisterForUpdates.IndustryClassID = dbo.MobizLibraryIndustryClass.ID"


            .SqlOrder = "ORDER BY LibraryRegisterForUpdates.LastUpdated desc"

            pDT = .Requery
        End With

        Return pDT
    End Function

    <HttpPost()>
    Function GetDetails() As DataTable
        Common.WebRequest.ProData.ExecuteSQL("Use MobizMobiz")
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryRegisterForUpdates.ID"
                End With

                With .Add("FirstName", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryRegisterForUpdates.FirstName"
                End With

                With .Add("LastName", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryRegisterForUpdates.LastName"
                End With

                With .Add("MiddleName", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryRegisterForUpdates.MiddleName"
                End With

                With .Add("BusinessName", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryRegisterForUpdates.BusinessName"
                End With

                With .Add("BusinessIndustry", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryIndustryClass.Class"
                End With

                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "c.Name"
                End With

                With .Add("City", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryCities.Name"
                End With

                With .Add("TelephoneNo", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryRegisterForUpdates.TelephoneNo"
                End With

                With .Add("CallingCode", SqlDbType.VarChar)
                    .Expression = "a.CallingCode"
                End With

                With .Add("AreaCode", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryAreaCodes.CallingCode"
                End With

                With .Add("MobileNo", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryRegisterForUpdates.MobileNo"
                End With

                With .Add("CountryCode", SqlDbType.VarChar)
                    .Expression = "b.CallingCode"
                End With

                With .Add("Prefix", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryTelcoPrefixes.Prefix"
                End With

                With .Add("EmailAddress", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryRegisterForUpdates.EmailAddress"
                End With

                With .Add("Product", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryRegisterForUpdates.MobizProduct"
                End With

                With .Add("LastUpdated", SqlDbType.Date)
                    .Expression = "dbo.LibraryRegisterForUpdates.LastUpdated"
                End With
            End With

            .SqlFrom = "FROM dbo.LibraryRegisterForUpdates LEFT JOIN " &
                      "dbo.MobizLibraryCities ON dbo.LibraryRegisterForUpdates.CityID = dbo.MobizLibraryCities.ID LEFT JOIN " &
                      "dbo.MobizLibraryCountries a ON dbo.LibraryRegisterForUpdates.CallingCodeTelID = a.ID LEFT JOIN " &
                      "dbo.MobizLibraryCountries b ON dbo.LibraryRegisterForUpdates.CallingCodeMobileID = b.ID LEFT JOIN " &
                      "dbo.MobizLibraryCountries c ON dbo.LibraryRegisterForUpdates.CountryID = c.ID LEFT JOIN " &
                      "dbo.MobizLibraryAreaCodes ON dbo.LibraryRegisterForUpdates.AreaCodeID = dbo.MobizLibraryAreaCodes.ID LEFT JOIN " &
                      "dbo.MobizLibraryTelcoPrefixes ON dbo.LibraryRegisterForUpdates.TelcoPrefixID = dbo.MobizLibraryTelcoPrefixes.ID LEFT JOIN " &
                      "dbo.MobizLibraryIndustryClass ON dbo.LibraryRegisterForUpdates.IndustryClassID = dbo.MobizLibraryIndustryClass.ID "

            .SqlWhere = "WHERE dbo.LibraryRegisterForUpdates.ID = " & ReadyString(WebRequest.Instance.Data("ID"))

            pDT = .Requery
        End With

        Return pDT
    End Function

    <HttpPost()>
    Function GetSearchResult() As DataTable
        Common.WebRequest.ProData.ExecuteSQL("Use MobizMobiz")
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = 50

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "dbo.LibraryRegisterForUpdates.ID"
                End With

                With .Add("FirstName", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryRegisterForUpdates.FirstName"
                End With

                With .Add("LastName", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryRegisterForUpdates.LastName"
                End With

                With .Add("MiddleName", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryRegisterForUpdates.MiddleName"
                End With

                With .Add("BusinessName", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryRegisterForUpdates.BusinessName"
                End With

                With .Add("BusinessIndustry", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryIndustryClass.Class"
                End With

                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "c.Name"
                End With

                With .Add("City", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryCities.Name"
                End With

                With .Add("TelephoneNo", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryRegisterForUpdates.TelephoneNo"
                End With

                With .Add("CallingCode", SqlDbType.VarChar)
                    .Expression = "a.CallingCode"
                End With

                With .Add("AreaCode", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryAreaCodes.CallingCode"
                End With

                With .Add("MobileNo", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryRegisterForUpdates.MobileNo"
                End With

                With .Add("CountryCode", SqlDbType.VarChar)
                    .Expression = "b.CallingCode"
                End With

                With .Add("Prefix", SqlDbType.VarChar)
                    .Expression = "dbo.MobizLibraryTelcoPrefixes.Prefix"
                End With

                With .Add("EmailAddress", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryRegisterForUpdates.EmailAddress"
                End With

                With .Add("Product", SqlDbType.VarChar)
                    .Expression = "dbo.LibraryRegisterForUpdates.MobizProduct"
                End With

                With .Add("LastUpdated", SqlDbType.Date)
                    .Expression = "dbo.LibraryRegisterForUpdates.LastUpdated"
                End With
            End With

            .SqlFrom = "FROM dbo.LibraryRegisterForUpdates LEFT JOIN " &
                      "dbo.MobizLibraryCities ON dbo.LibraryRegisterForUpdates.CityID = dbo.MobizLibraryCities.ID LEFT JOIN " &
                      "dbo.MobizLibraryCountries a ON dbo.LibraryRegisterForUpdates.CallingCodeTelID = a.ID LEFT JOIN " &
                      "dbo.MobizLibraryCountries b ON dbo.LibraryRegisterForUpdates.CallingCodeMobileID = b.ID LEFT JOIN " &
                      "dbo.MobizLibraryCountries c ON dbo.LibraryRegisterForUpdates.CountryID = c.ID LEFT JOIN " &
                      "dbo.MobizLibraryAreaCodes ON dbo.LibraryRegisterForUpdates.AreaCodeID = dbo.MobizLibraryAreaCodes.ID LEFT JOIN " &
                      "dbo.MobizLibraryTelcoPrefixes ON dbo.LibraryRegisterForUpdates.TelcoPrefixID = dbo.MobizLibraryTelcoPrefixes.ID LEFT JOIN " &
                      "dbo.MobizLibraryIndustryClass ON dbo.LibraryRegisterForUpdates.IndustryClassID = dbo.MobizLibraryIndustryClass.ID"

            .SearchString = WebRequest.Instance.Data("Keywords")
            .SqlOrder = "ORDER BY LibraryRegisterForUpdates.LastUpdated desc"

            pDT = .Requery
        End With

        Return pDT
    End Function
End Class
