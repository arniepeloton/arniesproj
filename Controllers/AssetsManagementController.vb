Imports Rebex.IO.Compression

Namespace WebProj
    Public Class AssetsManagementController
        Inherits System.Web.Mvc.Controller
        Dim pSql As String

        Function Index() As ActionResult
            Return View()
        End Function

        Public Function SearchExistingGuest()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim pDT As DataTable

            With New Finder
                .MaxRecords = 0
                .ProData = Common.WebRequest.ProData
                .SearchString = Common.WebRequest.Data("keywords")
                With .Fields
                    With .Add("DbID", SqlDbType.UniqueIdentifier)
                        .Expression = "cus.ID"
                        .Searchable = False
                    End With
                    With .Add("ID", SqlDbType.UniqueIdentifier)
                        .Expression = "org.ID"
                        .Searchable = False
                    End With
                    With .Add("CustomerName", SqlDbType.VarChar)
                        .Expression = "CASE WHEN org.CompanyName = '' THEN ISNULL(org.PersonFullName + ' ', '') + ISNULL(org.PersonSurname + ', ', '')  + ISNULL(org.PersonFirstName + ' ', '') + ISNULL(org.PersonMiddleName, '') ELSE org.CompanyName END"
                    End With
                    With .Add("IsOrganization", SqlDbType.VarChar)
                        .Expression = "org.IsOrganization"
                        .Searchable = False
                    End With

                    If Common.WebRequest.Data("FilterBy") = "Person" Then
                        .Add("PersonFullName", SqlDbType.VarChar)
                        .Add("PersonFirstName", SqlDbType.VarChar)
                        .Add("PersonMiddleName", SqlDbType.VarChar)
                        .Add("PersonSurname", SqlDbType.VarChar)
                        With .Add("PersonDateOfBirth", SqlDbType.Date)
                            .Expression = "org.PersonDateOfBirth"
                            .Searchable = False
                        End With
                    End If

                    With .Add("LastCheckedIn", SqlDbType.DateTime)
                        .Expression = "(SELECT TOP 1 ISNULL(ActualCheckedInUsedOn, CheckedInUsedOn) FROM TransRentableAssets WHERE ClientID = org.ID ORDER BY ISNULL(ActualCheckedInUsedOn, CheckedInUsedOn) DESC)"
                        .Searchable = False
                    End With
                End With
                .SqlFrom = "FROM LibraryOrganizationCustomer as cus " & vbCrLf &
                        " INNER JOIN MobizLibraryOrganization as org ON org.ID = cus.OrganizationID  " & vbCrLf

                .SqlWhere = " WHERE cus.Deactivated = 0 "

                If Common.WebRequest.Data("FilterBy") = "Org" Then
                    .SqlWhere += " AND org.IsOrganization = 1 "
                ElseIf Common.WebRequest.Data("FilterBy") = "Person" Then
                    .SqlWhere += " AND org.IsOrganization = 0 "
                End If

                .SqlOrder = "ORDER BY org.CompanyName, org.PersonFullName, org.PersonSurname ASC"

                pDT = .Requery
            End With

            Return JsonDictionary.Serialize("Results", pDT)
        End Function

        Function GetProspectDetailsPeople() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim org As New OrganizationShort
            Dim pOut As New JsonDictionary
            Dim peopleID As String = Common.WebRequest.Data("ID").ToString()

            pOut.Add("Address", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(org.GetProspectPeopleAddress(peopleID))))
            pOut.Add("Mobile", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(org.GetProspectPeopleMobile(peopleID))))
            pOut.Add("Email", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(org.GetProspectPeopleEmail(peopleID))))

            Return pOut.Serialize()
        End Function

        Function GetInvoicingMethod() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Common.WebRequest.ProData.BeginTransaction()

                Dim keyword As String = Common.WebRequest.Data("keywords")
                Dim status As String = Common.WebRequest.Data("status")

                pSql = "SELECT * FROM (SELECT met.*, ppol.FirstName + ' ' + ppol.Surname as ModifiedBy, " & vbCrLf &
                        " CASE MethodType " & vbCrLf &
                        "   WHEN 'uponcheckin' THEN 'Invoice upon check-in' " & vbCrLf &
                        "   WHEN 'uponcheckout' THEN 'Invoice upon check-out' " & vbCrLf &
                        "   WHEN 'bydays' THEN 'Invoice every ' + CONVERT(varchar, NoOfDays) + ' day(s) from check-in date' " & vbCrLf &
                        "   WHEN 'bydaysinterval' THEN 'Invoice every ' + CONVERT(varchar, NoOfDays) + ' day(s) from check-in date' " & vbCrLf &
                        "   ELSE 'Invoice every ' + CONVERT(varchar, NoOfDays) + 'th day of the month' " & vbCrLf &
                        " END as MethodName "

                If keyword <> "" Then
                    pSql += ", ISNULL((SELECT DISTINCT STUFF((SELECT DISTINCT ',' + cat.Category " & vbCrLf &
                            "       FROM LibraryInvoicingMethodCategory as cat " & vbCrLf &
                            "       WHERE cat.InvoicingMethodID = met.ID " & vbCrLf &
                            "   FOR XML PATH('') " & vbCrLf &
                            " ), 1, 1,'') FROM LibraryInvoicingMethodCategory), '') as Categories "
                End If

                pSql += "FROM LibraryInvoicingMethod as met " & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = met.UserID) as tb " & vbCrLf &
                        IIf(status = "All", " WHERE IsActive = 1 OR IsActive = 0 ", IIf(status = Nothing, " WHERE IsActive = 1", "WHERE IsActive = " & ReadyString(status)))

                If keyword <> "" Then
                    pSql += " AND MethodName + ' ' + Categories LIKE " & ReadyStringWild(keyword)
                End If

                Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)
                Dim pOut As New JsonDictionary
                Dim arrMethod As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDT))

                For i As Integer = 0 To arrMethod.length - 1
                    pSql = "SELECT *, 'edit' as State FROM LibraryInvoicingMethodCategory WHERE InvoicingMethodID = " & ReadyString(arrMethod(i)("ID"))
                    arrMethod(i).Add("Category", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))
                Next

                pOut.Add("Results", arrMethod)

                Return pOut.Serialize()
            Catch ex As Exception
                AppendToLog(ex.Message)
            End Try
        End Function

        Function SaveInvoicingMethod() As Object
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Common.WebRequest.ProData.BeginTransaction()

                Dim data As Object = Common.WebRequest.AllData("data")
                Dim p As New InsertUpdate("LibraryInvoicingMethod")
                Dim methodID As String = Guid.NewGuid.ToString()
                Dim exmethodID As String = "" ' existing method
                Dim finalmethodID As String = "" ' :)
                Dim arrExist As New List(Of Dictionary(Of String, Object))

                p("MethodType") = ReadyString(data("MethodType"))
                p("NoOfDays") = ReadyString(data("NoOfDays"))
                p("IsActive") = ReadyString(data("IsActive"))
                If data.ContainsKey("MonthlyBillType") Then
                    p("MonthlyBillType") = ReadyString(data("MonthlyBillType"))
                End If
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = "getdate()"

                'check if theres exact record of new inv method
                pSql = "SELECT ID FROM LibraryInvoicingMethod WHERE MethodType = " & ReadyString(data("MethodType")) & vbCrLf &
                        " AND NoOfDays = " & ReadyString(data("NoOfDays"))
                Dim row As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                If row.Count > 0 Then ' if exist, use the existing ID to avoid duplication
                    exmethodID = row(0)("ID").ToString
                End If

                If data("ID").ToString() = "" And exmethodID = "" Then

                    p("ID") = ReadyString(methodID)
                    pSql = p.GetInsert()
                Else
                    methodID = data("ID").ToString()
                    If exmethodID = "" Then
                        pSql = p.GetUpdate(" ID = " & ReadyString(methodID))
                    Else
                        pSql = p.GetUpdate(" ID = " & ReadyString(exmethodID))
                    End If
                End If

                Common.WebRequest.ProData.Execute(pSql)

                finalmethodID = IIf(exmethodID = "", methodID, exmethodID)

                For Each cat As Object In data("Category")
                    Dim pExist As New Dictionary(Of String, Object)

                    p = New InsertUpdate("LibraryInvoicingMethodCategory")

                    pSql = "SELECT cat.ID, met.MethodType, met.NoOfDays FROM LibraryInvoicingMethodCategory as cat" & vbCrLf &
                            " INNER JOIN LibraryInvoicingMethod as met ON met.ID = cat.InvoicingMethodID " & vbCrLf &
                            " WHERE Category = " & ReadyString(cat("Category")) & vbCrLf &
                            " AND InvoicingMethodID <> " & ReadyString(finalmethodID)
                    Dim ex As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                    'check if category is already belong to other invoicing method
                    If ex.Count = 0 Then
                        pSql = "SELECT * FROM LibraryInvoicingMethodCategory WHERE Category = " & ReadyString(cat("Category")) & vbCrLf &
                                " AND InvoicingMethodID = " & ReadyString(finalmethodID)

                        If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            p("InvoicingMethodID") = ReadyString(finalmethodID)
                            p("Category") = ReadyString(cat("Category"))

                            pSql = p.GetInsert()
                            Common.WebRequest.ProData.Execute(pSql)
                        Else
                            If cat("State") = "remove" Then
                                pSql = "DELETE FROM LibraryInvoicingMethodCategory WHERE ID = " & ReadyString(cat("ID"))
                                Common.WebRequest.ProData.Execute(pSql)
                            End If
                        End If
                    Else
                        If exmethodID <> ex(0)("ID").ToString() Then ' exclude the same method id
                            pExist.Add("ID", ex(0)("ID").ToString())
                            pExist.Add("Category", cat("Category"))
                            pExist.Add("MethodType", ex(0)("MethodType").ToString())
                            pExist.Add("NoOfDays", ex(0)("NoOfDays").ToString())
                            arrExist.Add(pExist)
                        End If
                    End If
                Next cat

                If arrExist.Count > 0 Then
                    Return Json(New With {Key .Results = arrExist})
                End If

                For Each transcat As Object In data("TransferCategory")
                    p = New InsertUpdate("LibraryInvoicingMethodCategory")

                    p("InvoicingMethodID") = ReadyString(finalmethodID)
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" Category = " & ReadyString(transcat("Category"))))

                Next transcat

                Common.WebRequest.ProData.CommitTransaction()

                Return Json(New With {Key .Results = "saved"})

            Catch ex As Exception
                AppendToLog(ex.Message)
                Return Json(New With {Key .Results = ex.Message})
            End Try
        End Function

        Function GetAssetsCategory() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = " SELECT cat.C0020 as Category " & vbCrLf &
                        " FROM LibraryRentableAssets as ass " & vbCrLf &
                        " INNER JOIN MobizLibraryPimProduct as pim ON pim.ID = ass.ProductID" & vbCrLf &
                        " INNER JOIN MobizLibraryPIMProductVariableFeatures001 as cat ON cat.ProductID = ass.ProductID " & vbCrLf &
                        " WHERE pim.PublishedByID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                        " AND IsAsset = 1 " & vbCrLf &
                        " GROUP BY cat.C0020 ORDER BY cat.C0020"

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function SearchClient() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            'pSql = "SELECT ISNULL(pubppol.FirstName, pricusppol.FirstName) + ' ' + ISNULL(pubppol.MiddleName, pricusppol.MiddleName) + ' ' + ISNULL(pubppol.Surname, pricusppol.Surname) as CustomerName " & vbCrLf &
            '        " , trans.ReservedFrom, ReservedUntil, CheckedInUsedOn, CheckedOutReturnedOn " & vbCrLf &
            '        " FROM TransRentableAssets as trans " & vbCrLf &
            '        " LEFT JOIN MobizLibraryPeople as pubppol ON pubppol.ID = trans.ClientID " & vbCrLf &
            '        " LEFT JOIN LibraryPeople as pricusppol ON pricusppol.ID = trans.ClientID " & vbCrLf &
            '        " WHERE IsDone = 0 " & vbCrLf &
            '        "  AND (ISNULL(pubppol.FirstName, pricusppol.FirstName) + ' ' + " & vbCrLf &
            '        "   ISNULL(pubppol.MiddleName, pricusppol.MiddleName) + ' ' + " & vbCrLf &
            '        "   ISNULL(pubppol.Surname, pricusppol.Surname) LIKE " & ReadyStringWild(Common.WebRequest.Data("Search")) & ") " & vbCrLf

            pSql = "SELECT DISTINCT " & vbCrLf &
                    " transcus.ClientID, " & vbCrLf &
                    " ISNULL(pubppol.PersonSurname, pricusppol.Surname) + ', ' + ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) + ' ' + ISNULL(pubppol.PersonMiddleName, pricusppol.MiddleName) as CustomerName " & vbCrLf &
                    " FROM TransRentableAssets as trans " & vbCrLf &
                    " INNER JOIN TransRentableAssetCustomer as transcus ON transcus.TransRentableAssetID = trans.ID " & vbCrLf &
                    " LEFT JOIN MobizLibraryOrganization as pubppol ON pubppol.ID = transcus.ClientID " & vbCrLf &
                    " LEFT JOIN LibraryPeople as pricusppol ON pricusppol.ID = transcus.ClientID " & vbCrLf &
                    " WHERE --trans.IsDone = 0 AND " & vbCrLf &
                    " ISNULL(pubppol.PersonSurname, pricusppol.Surname) + ', ' + ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) + ' ' + ISNULL(pubppol.PersonMiddleName, pricusppol.MiddleName) " & vbCrLf &
                    " LIKE " & ReadyStringWild(Common.WebRequest.Data("Search"))

            'pSql = "SELECT TransID, ClientID, CustomerName " & vbCrLf &
            '        " FROM ( " & vbCrLf &
            '        "       SELECT trans.ID as TransID, ISNULL(pubppol.ID, pricusppol.ID) as ClientID, " & vbCrLf &
            '        "       ISNULL(pubppol.PersonSurname, pricusppol.Surname) + ', ' + ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) + ' ' + ISNULL(pubppol.PersonMiddleName, pricusppol.MiddleName) as CustomerName " & vbCrLf &
            '        "       FROM TransRentableAssets as trans " & vbCrLf &
            '        "       LEFT JOIN MobizLibraryOrganization as pubppol ON pubppol.ID = trans.ClientID " & vbCrLf &
            '        "       LEFT JOIN LibraryPeople as pricusppol ON pricusppol.ID = trans.ClientID " & vbCrLf &
            '        "       WHERE IsDone = 0 " & vbCrLf &
            '        "   UNION ALL " & vbCrLf &
            '        "       SELECT " & vbCrLf &
            '        "       trans.ID as TransID, " & vbCrLf &
            '        "       (SELECT TOP 1 ClientID FROM TransRentableAssetCustomer WHERE TransRentableAssetID = trans.ID) as ClientID, " & vbCrLf &
            '        "       (SELECT DISTINCT STUFF((SELECT DISTINCT ', ' + " & vbCrLf &
            '        "           ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) + ' ' + ISNULL(pubppol.PersonMiddleName, pricusppol.MiddleName) + ' ' + ISNULL(pubppol.PersonSurname, pricusppol.Surname) " & vbCrLf &
            '        "           FROM TransRentableAssetCustomer as transcus " & vbCrLf &
            '        "           LEFT JOIN MobizLibraryOrganization as pubppol ON pubppol.ID = transcus.ClientID " & vbCrLf &
            '        "           LEFT JOIN LibraryPeople as pricusppol ON pricusppol.ID = transcus.ClientID " & vbCrLf &
            '        "           WHERE transcus.TransRentableAssetID = trans.ID " & vbCrLf &
            '        "           FOR XML PATH('') " & vbCrLf &
            '        "       ), 1, 1,'')) as CustomerName " & vbCrLf &
            '        "       FROM TransRentableAssets as trans " & vbCrLf &
            '        "       WHERE IsDone = 0 " & vbCrLf &
            '        ") as T WHERE CustomerName LIKE " & ReadyStringWild(Common.WebRequest.Data("Search")) & vbCrLf

            Dim arrAsset As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))
            Dim pOut As New JsonDictionary

            For i As Integer = 0 To arrAsset.Length - 1
                Dim selectedMonth As String = ReadyString(CInt2(Common.WebRequest.Data("Year")) & "" & Format(CInt2(Common.WebRequest.Data("Month")) + 1, "0#"))
                pSql = "SELECT " & vbCrLf &
                        " ISNULL(trans.IsDone, 'False') as IsDone " & vbCrLf &
                        " , ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) + ' ' + ISNULL(pubppol.PersonMiddleName, pricusppol.MiddleName) + ' ' + ISNULL(pubppol.PersonSurname, pricusppol.Surname) as CustomerName " & vbCrLf &
                        " , cusorg.ID as OrgID " & vbCrLf &
                        " , cusorg.CompanyName " & vbCrLf &
                        " , trans.ID " & vbCrLf &
                        " , trans.TransactionNumber " & vbCrLf &
                        " , trans.AssetID, resass.ProductID " & vbCrLf &
                        " , trans.OrgID " & vbCrLf &
                        " , ISNULL(convert(varchar(max), trans.ClientID), (SELECT TOP 1 ClientID FROM TransRentableAssetCustomer WHERE TransRentableAssetID = trans.ID))  as ClientID " & vbCrLf &
                        " , trans.TransRefID " & vbCrLf &
                        " , trans.IsCancelled " & vbCrLf &
                        " , trans.ID as TransID " & vbCrLf &
                        " , trans.NoOfAdults " & vbCrLf &
                        " , trans.NoOfChild " & vbCrLf &
                        " , trans.OnlineTravelAgenyName " & vbCrLf &
                        " , trans.OTAConfirmationNumber " & vbCrLf &
                        " , trans.CustomRates " & vbCrLf &
                        " , trans.PriceLevelID " & vbCrLf &
                        " , trans.OutOfOrderReason " & vbCrLf &
                        " , ISNULL(trans.IsDoneCleaning, 'False') as IsDoneCleaning " & vbCrLf &
                        " , ISNULL(trans.IsOutOfOrder, 'False') as IsOutOfOrder " & vbCrLf &
                        " , convert(varchar, trans.CreatedDate, 120) as CreatedDate " & vbCrLf &
                        " , convert(varchar, trans.LastUpdated, 120) as LastUpdated " & vbCrLf &
                        " , convert(varchar, trans.DateCancelled, 120) as DateCancelled " & vbCrLf &
                        " , convert(varchar, trans.TransferDate, 120) as TransferDate " & vbCrLf &
                        " , trans.TransferFromID " & vbCrLf &
                        " , convert(varchar, trans.ReservedFrom, 120) as ReservedFrom " & vbCrLf &
                        " , convert(varchar, trans.ReservedUntil, 120) as ReservedUntil " & vbCrLf &
                        " , convert(varchar, trans.CheckedOutReturnedOn, 120) as CheckedOutReturnedOn " & vbCrLf &
                        " , convert(varchar, trans.ActualCheckedOutReturnedOn, 120) as ActualCheckedOutReturnedOn " & vbCrLf &
                        " , convert(varchar, trans.OutOfOrderFrom, 120) as OutOfOrderFrom " & vbCrLf &
                        " , convert(varchar, trans.OutOfOrderTo, 120) as OutOfOrderTo " & vbCrLf &
                        " , convert(varchar, trans.CheckedInUsedOn, 120) as CheckedInUsedOn " & vbCrLf &
                        " , convert(varchar, ISNULL(trans.ActualCheckedInUsedOn, trans.CheckedInUsedOn), 120) as ActualCheckedInUsedOn " & vbCrLf &
                        " , (SELECT prdprc.Price FROM TransRentableAssets as transren " & vbCrLf &
                        "       INNER JOIN LibraryRentableAssets as ren ON transren.AssetID = ren.ID " & vbCrLf &
                        "       INNER JOIN MobizLibraryPIMProductPriceLevel as prdprc ON ren.ProductID = prdprc.ProductID " & vbCrLf &
                        "       INNER JOIN MobizLibraryPIMPriceLevel prc ON prdprc.PriceLevelID = prc.ID " & vbCrLf &
                        "       WHERE prdprc.PriceLevelID = trans.PriceLevelID AND prdprc.ProductID = ren.ProductID AND transren.ID = trans.ID " & vbCrLf &
                        "       AND prdprc.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & ") as Rate" & vbCrLf &
                        " , cusorg.OfficeAddressBuildingNo + ' ' + cusorg.OfficeAddressStreet + ', ' + cusorgcity.Name + ', ' + cusorgcountry.Name as CompanyAddress " & vbCrLf &
                        " , pimpro.ProductName as AssetName " & vbCrLf &
                        " , CASE WHEN ISNULL(trans.IsDone, 0) = 0 THEN CASE WHEN ISNULL(trans.CheckedInUsedOn, '') = '' THEN 'reserved' ELSE 'unavailable' END ELSE CASE WHEN ISNULL(trans.IsCancelled, 0) = 1 THEN 'cancelled' ELSE 'pastcheckin' END END as Status" & vbCrLf &
                        " , CASE WHEN ISNULL(trans.IsDone, 0) = 0 OR ISNULL(trans.IsCancelled, 0) = 1 THEN 'False' ELSE 'True' END as IsPastCheckins" & vbCrLf &
                        " , updatedby.FirstName + ' ' + updatedby.Surname as UpdatedBy" & vbCrLf &
                        " FROM TransRentableAssetCustomer as transcus " & vbCrLf &
                        " INNER JOIN TransRentableAssets as trans ON trans.ID = transcus.TransRentableAssetID " & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganization as cusorg ON cusorg.ID = trans.OrgID " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as updatedby ON updatedby.ID = trans.CreatedBy " & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganization as pubppol ON pubppol.ID = CASE WHEN ISNULL(convert(varchar(max), trans.ClientID), '') = '' THEN (SELECT TOP 1 ClientID FROM TransRentableAssetCustomer WHERE TransRentableAssetID = trans.ID) ELSE trans.ClientID END " & vbCrLf &
                        " LEFT JOIN LibraryPeople as pricusppol ON pricusppol.ID = CASE WHEN ISNULL(convert(varchar(max), trans.ClientID), '') = '' THEN (SELECT TOP 1 ClientID FROM TransRentableAssetCustomer WHERE TransRentableAssetID = trans.ID) ELSE trans.ClientID END " & vbCrLf &
                        " LEFT JOIN MobizLibraryCities as cusorgcity ON cusorgcity.ID = cusorg.OfficeAddressCityID " & vbCrLf &
                        " LEFT JOIN MobizLibraryCountries as cusorgcountry ON cusorgcountry.ID = cusorg.OfficeAddressCountryID " & vbCrLf &
                        " INNER JOIN LibraryRentableAssets as resass ON resass.ID = trans.AssetID " & vbCrLf &
                        " INNER JOIN MobizLibraryPIMProduct as pimpro ON pimpro.ID = resass.ProductID " & vbCrLf &
                        " INNER JOIN MobizLibraryPIMProductVariableFeatures001 as cat ON cat.ProductID = resass.ProductID " & vbCrLf &
                        " WHERE transcus.ClientID = " & ReadyString(arrAsset(i)("ClientID")) & vbCrLf &
                        " --AND trans.IsDone = 0 " & vbCrLf &
                        " ORDER BY trans.LastUpdated DESC "

                Dim row As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

                For Each trans As Object In row
                    pSql = " SELECT " & vbCrLf &
                            " transcus.ClientID, ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) + ' ' + ISNULL(pubppol.PersonMiddleName, pricusppol.MiddleName) + ' ' + ISNULL(pubppol.PersonSurname, pricusppol.Surname) as CustomerName " & vbCrLf &
                            " , ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) as FirstName " & vbCrLf &
                            " , ISNULL(pubppol.PersonMiddleName, pricusppol.MiddleName) as MiddleName " & vbCrLf &
                            " , ISNULL(pubppol.PersonSurname, pricusppol.Surname) as Surname " & vbCrLf &
                            " , convert(varchar, pubppol.PersonDateOfBirth, 107) as DateOfBirth " & vbCrLf &
                            " , ppolcity.Name as CityName " & vbCrLf &
                            " , ppolcountry.Name as CountryName " & vbCrLf &
                            " , ISNULL(CONVERT(varchar(MAX), ppoladd.CityID), '') as CityID " & vbCrLf &
                            " , ISNULL(CONVERT(varchar(MAX), ppoladd.CountryID), '') as CountryID " & vbCrLf &
                            " , ppoladd.HouseBuildingNo " & vbCrLf &
                            " , ppoladd.Street " & vbCrLf &
                            " , ISNULL(CONVERT(varchar(MAX), ppoladd.ID), '') as AddressID " & vbCrLf &
                            " , ISNULL(CONVERT(varchar(MAX), ppolemail.ID), '') as EmailID " & vbCrLf &
                            " , ppolemail.EmailAddress " & vbCrLf &
                            " , ISNULL(CONVERT(varchar(MAX), ppolmobile.ID), '') as MobileID " & vbCrLf &
                            " , MobizLibraryTelcoPrefixes.Prefix + '' + ppolmobile.ContactNo as ContactNo " & vbCrLf &
                            " , ppolmobile.CountryID as CountryCodeID " & vbCrLf &
                            " , ppolmobile.TelCoPrefixID " & vbCrLf &
                            " FROM TransRentableAssetCustomer as transcus " & vbCrLf &
                            " LEFT JOIN MobizLibraryOrganization as pubppol ON pubppol.ID = transcus.ClientID " & vbCrLf &
                            " LEFT JOIN LibraryPeople as pricusppol ON pricusppol.ID = transcus.ClientID " & vbCrLf &
                            " OUTER APPLY (SELECT TOP 1 * FROM MobizLibraryPeopleAddress WHERE transcus.ClientID = PeopleID ORDER BY LastUpdated DESC) as ppoladd " & vbCrLf &
                            " OUTER APPLY (SELECT TOP 1 * FROM LibraryPeopleContactEmail WHERE PeopleID = transcus.ClientID AND EmailAddress <> '' ORDER BY LastUpdated DESC) as ppolemail " & vbCrLf &
                            " LEFT JOIN LibraryPeopleContactMobile as ppolmobile ON ppolmobile.ID = (SELECT TOP 1 ID FROM LibraryPeopleContactMobile WHERE PeopleID = transcus.ClientID) " & vbCrLf &
                            " LEFT JOIN MobizLibraryTelcoPrefixes ON MobizLibraryTelcoPrefixes.ID = ppolmobile.TelCoPrefixID " & vbCrLf &
                            " LEFT JOIN MobizLibraryCities as ppolcity ON ppolcity.ID = ppoladd.CityID " & vbCrLf &
                            " LEFT JOIN MobizLibraryCountries as ppolcountry ON ppolcountry.ID = ppoladd.CountryID " & vbCrLf &
                            " WHERE transcus.TransRentableAssetID = " & ReadyString(trans("ID")) & vbCrLf &
                            " ORDER BY transcus.SortOrderNo ASC "

                    Dim rowCus As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

                    If rowCus.Length = 0 Then
                        pSql = " SELECT " & vbCrLf &
                            " trans.ClientID, ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) + ' ' + ISNULL(pubppol.PersonMiddleName, pricusppol.MiddleName) + ' ' + ISNULL(pubppol.PersonSurname, pricusppol.Surname) as CustomerName " & vbCrLf &
                            " , ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) as FirstName " & vbCrLf &
                            " , ISNULL(pubppol.PersonMiddleName, pricusppol.MiddleName) as MiddleName " & vbCrLf &
                            " , ISNULL(pubppol.PersonSurname, pricusppol.Surname) as Surname " & vbCrLf &
                            " , convert(varchar, pubppol.PersonDateOfBirth, 107) as DateOfBirth " & vbCrLf &
                            " , ppolcity.Name as CityName " & vbCrLf &
                            " , ppolcountry.Name as CountryName " & vbCrLf &
                            " , ISNULL(CONVERT(varchar(MAX), ppoladd.CityID), '') as CityID " & vbCrLf &
                            " , ISNULL(CONVERT(varchar(MAX), ppoladd.CountryID), '') as CountryID " & vbCrLf &
                            " , ppoladd.HouseBuildingNo " & vbCrLf &
                            " , ppoladd.Street " & vbCrLf &
                            " , ISNULL(CONVERT(varchar(MAX), ppoladd.ID), '') as AddressID " & vbCrLf &
                            " , ISNULL(CONVERT(varchar(MAX), ppolemail.ID), '') as EmailID " & vbCrLf &
                            " , ppolemail.EmailAddress " & vbCrLf &
                            " , ISNULL(CONVERT(varchar(MAX), ppolmobile.ID), '') as MobileID " & vbCrLf &
                            " , MobizLibraryTelcoPrefixes.Prefix + '' + ppolmobile.ContactNo as ContactNo " & vbCrLf &
                            " , ppolmobile.CountryID as CountryCodeID " & vbCrLf &
                            " , ppolmobile.TelCoPrefixID " & vbCrLf &
                            " FROM TransRentableAssets as trans " & vbCrLf &
                            " LEFT JOIN MobizLibraryOrganization as pubppol ON pubppol.ID = trans.ClientID " & vbCrLf &
                            " LEFT JOIN LibraryPeople as pricusppol ON pricusppol.ID = trans.ClientID " & vbCrLf &
                            " OUTER APPLY (SELECT TOP 1 * FROM MobizLibraryPeopleAddress WHERE trans.ClientID = PeopleID ORDER BY LastUpdated DESC) as ppoladd " & vbCrLf &
                            " OUTER APPLY (SELECT TOP 1 * FROM LibraryPeopleContactEmail WHERE PeopleID = trans.ClientID AND EmailAddress <> '' ORDER BY LastUpdated DESC) as ppolemail " & vbCrLf &
                            " LEFT JOIN LibraryPeopleContactMobile as ppolmobile ON ppolmobile.ID = (SELECT TOP 1 ID FROM LibraryPeopleContactMobile WHERE PeopleID = trans.ClientID) " & vbCrLf &
                            " LEFT JOIN MobizLibraryTelcoPrefixes ON MobizLibraryTelcoPrefixes.ID = ppolmobile.TelCoPrefixID " & vbCrLf &
                            " LEFT JOIN MobizLibraryCities as ppolcity ON ppolcity.ID = ppoladd.CityID " & vbCrLf &
                            " LEFT JOIN MobizLibraryCountries as ppolcountry ON ppolcountry.ID = ppoladd.CountryID " & vbCrLf &
                            " WHERE trans.ID = " & ReadyString(trans("ID").ToString())

                        rowCus = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))
                    End If

                    For Each cus As Object In rowCus
                        pSql = "SELECT req.*, 'edit' as State " & vbCrLf &
                            " , ppol.FirstName + ' ' + ppol.Surname as ModifiedBy" & vbCrLf &
                            " , convert(varchar, req.LastUpdated, 120) as ModifiedDate " & vbCrLf &
                            " FROM TransRentableAssetCustomerRequest as req" & vbCrLf &
                            " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = req.UserID " & vbCrLf &
                            " WHERE req.TransRentableAssetID = " & ReadyString(trans("ID")) &
                            " AND req.IsActive = 1 AND req.Action <> 'Deleted' ORDER BY req.LastUpdated DESC"
                        cus.Add("Request", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

                        If CBool2(trans("IsOutOfOrder")) = False Then
                            pSql = "SELECT req.*, 'edit' as State " & vbCrLf &
                                " , ppol.FirstName + ' ' + ppol.Surname as ModifiedBy" & vbCrLf &
                                " , convert(varchar, req.LastUpdated, 120) as ModifiedDate " & vbCrLf &
                                " FROM TransRentableAssetCustomerRequest as req" & vbCrLf &
                                " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = req.UserID " & vbCrLf &
                                " WHERE req.ClientID = " & ReadyString(cus("ClientID")) &
                                " AND req.IsDone = 1 AND req.IsActive = 1 ORDER BY req.LastUpdated DESC"
                            cus.Add("PreviousRequest", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))
                        End If
                    Next cus

                    trans.Add("Customer", rowCus)

                    pSql = "SELECT ID as TransOCId, Qty as RequestQty, OtherChargesID " & vbCrLf &
                            " FROM TransRentableAssetOtherCharges WHERE TransRentableAssetID = " & ReadyString(trans("ID"))
                    trans.Add("OtherCharges", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))
                Next trans
                arrAsset(i).Add("Trans", row)
            Next

            pOut.Add("Results", arrAsset)

            Return pOut.Serialize()

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function GetGridData()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim type As String = Common.WebRequest.Data("type")
            Dim datefrom As String = Common.WebRequest.Data("DateFrom")
            Dim dateto As String = Common.WebRequest.Data("DateTo")

            Try
                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(GetQueryDashboard(type, datefrom, dateto)))
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetDashboardData() As Object
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim datetoday As String = Common.WebRequest.Data("date")
            Dim datefrom As String = datetoday
            Dim dateto As String = datetoday

            Try
                pSql = GetQueryDashboard("NoOfCheckedIn", datefrom, dateto)

                Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)
                Dim pOut As New JsonDictionary

                pOut.Add("NoOfCheckedInToday", pDT)

                pSql = GetQueryDashboard("ScheduledCheckout", datefrom, dateto)
                pDT = Common.WebRequest.ProData.Execute(pSql)

                pOut.Add("ScheduledCheckout", pDT)


                pSql = GetQueryDashboard("NoOfReservation", datefrom, dateto)
                pDT = Common.WebRequest.ProData.Execute(pSql)

                pOut.Add("NoOfReservationMadeToday", pDT)

                pSql = GetQueryDashboard("NoOfCancelled", datefrom, dateto)
                pDT = Common.WebRequest.ProData.Execute(pSql)

                pOut.Add("NoOfCancelledToday", pDT)

                pSql = GetQueryDashboard("NoOfOccupied", datefrom, dateto)
                pDT = Common.WebRequest.ProData.Execute(pSql)

                pOut.Add("NoOfOccupiedToday", pDT)

                'pSql = " SELECT trans.* " & vbCrLf &
                '    " , ISNULL(pubppol.FirstName, pricusppol.FirstName) + ' ' + ISNULL(pubppol.MiddleName, pricusppol.MiddleName) + ' ' + ISNULL(pubppol.Surname, pricusppol.Surname) as CustomerName " & vbCrLf &
                '    " , pim.ProductName as AssetName " & vbCrLf &
                '    " FROM TransRentableAssets as trans " & vbCrLf &
                '    " INNER JOIN LibraryRentableAssets as ass ON ass.ID = trans.AssetID " & vbCrLf &
                '    " INNER JOIN MobizLibraryPimProduct as pim ON pim.ID = ass.ProductID " & vbCrLf &
                '    " LEFT JOIN MobizLibraryPeople as pubppol ON pubppol.ID = trans.ClientID " & vbCrLf &
                '    " LEFT JOIN LibraryPeople as pricusppol ON pricusppol.ID = trans.ClientID " & vbCrLf &
                '    " WHERE (GETDATE() BETWEEN Trans.ReservedFrom AND trans.ReservedUntil) " & vbCrLf &
                '    " AND IsDone = 0 ORDER BY trans.LastUpdated DESC" & vbCrLf
                'pDT = Common.WebRequest.ProData.Execute(pSql)

                'pOut.Add("NoOfUnoccupiedUnusedToday", pDT)

                pSql = GetQueryDashboard("NoOfUnoccupiedUnused", datefrom, dateto)
                pDT = Common.WebRequest.ProData.Execute(pSql)

                pOut.Add("NoOfUnoccupiedUnusedToday", pDT)

                pSql = GetQueryDashboard("NoOfReserved", datefrom, dateto)
                pDT = Common.WebRequest.ProData.Execute(pSql)

                pOut.Add("NoOfReservedToday", pDT)

                Return pOut.Serialize()
            Catch ex As Exception
                AppendToDebug("vbHTYYY7MP2C!", True, ex, True)
            End Try
        End Function

        Function GetQueryDashboard(ByVal type As String, ByVal datefrom As String, ByVal dateto As String)
            Dim pSql As String

            If type = "ScheduledCheckout" Then
                pSql = " SELECT trans.* " & vbCrLf &
                    " , CASE WHEN (SELECT COUNT(ID) FROM TransRentableAssetCustomer WHERE TransRentableAssetID = trans.ID) = 0 " & vbCrLf &
                    "   THEN (SELECT ISNULL(pubppol.FirstName, pricusppol.PersonFirstName) + ' ' + ISNULL(pubppol.MiddleName, pricusppol.PersonMiddleName) + ' ' + ISNULL(pubppol.Surname, pricusppol.PersonSurname) " & vbCrLf &
                    "           FROM TransRentableAssets " & vbCrLf &
                    "           LEFT JOIN MobizLibraryPeople as pubppol ON pubppol.ID = TransRentableAssets.ClientID " & vbCrLf &
                    "           LEFT JOIN MobizLibraryOrganization as pricusppol ON pricusppol.ID = TransRentableAssets.ClientID " & vbCrLf &
                    "           WHERE TransRentableAssets.ID = trans.ID) " & vbCrLf &
                    "   ELSE (SELECT DISTINCT STUFF((SELECT DISTINCT ', ' + " & vbCrLf &
                    "           ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) + ' ' + ISNULL(pubppol.PersonSurname, pricusppol.Surname) " & vbCrLf &
                    "           FROM TransRentableAssetCustomer as transcus " & vbCrLf &
                    "           LEFT JOIN MobizLibraryOrganization as pubppol ON pubppol.ID = transcus.ClientID " & vbCrLf &
                    "           LEFT JOIN LibraryPeople as pricusppol ON pricusppol.ID = transcus.ClientID " & vbCrLf &
                    "           WHERE transcus.TransRentableAssetID = trans.ID " & vbCrLf &
                    "           FOR XML PATH('')), 1, 1,'')) " & vbCrLf &
                    "   END as CustomerName " & vbCrLf &
                    " , pim.ProductName as AssetName, cat.C0020 as AssetType " & vbCrLf &
                    " FROM TransRentableAssets as trans " & vbCrLf &
                    " INNER JOIN LibraryRentableAssets as ass ON ass.ID = trans.AssetID " & vbCrLf &
                    " INNER JOIN MobizLibraryPimProduct as pim ON pim.ID = ass.ProductID " & vbCrLf &
                    " INNER JOIN MobizLibraryPIMProductVariableFeatures001 as cat ON cat.ProductID = ass.ProductID " & vbCrLf

                If Format(Now(), "MMM dd, yyyy") = Format(CDate(datefrom), "MMM dd, yyyy") And Format(Now(), "MMM dd, yyyy") = Format(CDate(dateto), "MMM dd, yyyy") Then
                    pSql += " WHERE (trans.CheckedOutReturnedOn BETWEEN CAST(" & ReadyString(datefrom) & " AS DATE) AND DATEADD(DAY, 1, CAST(" & ReadyString(dateto) & " AS DATE))) " & vbCrLf &
                            " AND IsDone = 0 AND ISNULL(CONVERT(varchar(max), TransferDate), '') = '' " & vbCrLf 'pag TODAY, katong wala pa nakacheckout ang kwaon na data
                Else
                    pSql += " WHERE (trans.ActualCheckedOutReturnedOn BETWEEN CAST(" & ReadyString(datefrom) & " AS DATE) AND DATEADD(DAY, 1, CAST(" & ReadyString(dateto) & " AS DATE))) " & vbCrLf
                End If

                pSql += " ORDER BY trans.LastUpdated DESC" & vbCrLf
            ElseIf type = "NoOfCheckedIn" Then
                pSql = " SELECT trans.* " & vbCrLf &
                    " , CASE WHEN (SELECT COUNT(ID) FROM TransRentableAssetCustomer WHERE TransRentableAssetID = trans.ID) = 0 " & vbCrLf &
                    "   THEN (SELECT ISNULL(pubppol.FirstName, pricusppol.PersonFirstName) + ' ' + ISNULL(pubppol.MiddleName, pricusppol.PersonMiddleName) + ' ' + ISNULL(pubppol.Surname, pricusppol.PersonSurname) " & vbCrLf &
                    "           FROM TransRentableAssets " & vbCrLf &
                    "           LEFT JOIN MobizLibraryPeople as pubppol ON pubppol.ID = TransRentableAssets.ClientID " & vbCrLf &
                    "           LEFT JOIN MobizLibraryOrganization as pricusppol ON pricusppol.ID = TransRentableAssets.ClientID " & vbCrLf &
                    "           WHERE TransRentableAssets.ID = trans.ID) " & vbCrLf &
                    "   ELSE (SELECT DISTINCT STUFF((SELECT DISTINCT ', ' + " & vbCrLf &
                    "           ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) + ' ' + ISNULL(pubppol.PersonSurname, pricusppol.Surname) " & vbCrLf &
                    "           FROM TransRentableAssetCustomer as transcus " & vbCrLf &
                    "           LEFT JOIN MobizLibraryOrganization as pubppol ON pubppol.ID = transcus.ClientID " & vbCrLf &
                    "           LEFT JOIN LibraryPeople as pricusppol ON pricusppol.ID = transcus.ClientID " & vbCrLf &
                    "           WHERE transcus.TransRentableAssetID = trans.ID " & vbCrLf &
                    "           FOR XML PATH('')), 1, 1,'')) " & vbCrLf &
                    "   END as CustomerName " & vbCrLf &
                    " , pim.ProductName as AssetName, cat.C0020 as AssetType " & vbCrLf &
                    " FROM TransRentableAssets as trans " & vbCrLf &
                    " INNER JOIN LibraryRentableAssets as ass ON ass.ID = trans.AssetID " & vbCrLf &
                    " INNER JOIN MobizLibraryPimProduct as pim ON pim.ID = ass.ProductID " & vbCrLf &
                    " INNER JOIN MobizLibraryPIMProductVariableFeatures001 as cat ON cat.ProductID = ass.ProductID " & vbCrLf &
                    " WHERE (CheckedInUsedOn BETWEEN CAST(" & ReadyString(datefrom) & " AS DATE) AND DATEADD(DAY, 1, CAST(" & ReadyString(dateto) & " AS DATE))) " & vbCrLf &
                    " AND ISNULL(TransferDate, '') = '' ORDER BY CheckedInUsedOn DESC"
            ElseIf type = "NoOfReservation" Then
                pSql = " SELECT trans.* " & vbCrLf &
                    " , CASE WHEN (SELECT COUNT(ID) FROM TransRentableAssetCustomer WHERE TransRentableAssetID = trans.ID) = 0 " & vbCrLf &
                    "   THEN (SELECT ISNULL(pubppol.FirstName, pricusppol.PersonFirstName) + ' ' + ISNULL(pubppol.MiddleName, pricusppol.PersonMiddleName) + ' ' + ISNULL(pubppol.Surname, pricusppol.PersonSurname) " & vbCrLf &
                    "           FROM TransRentableAssets " & vbCrLf &
                    "           LEFT JOIN MobizLibraryPeople as pubppol ON pubppol.ID = TransRentableAssets.ClientID " & vbCrLf &
                    "           LEFT JOIN MobizLibraryOrganization as pricusppol ON pricusppol.ID = TransRentableAssets.ClientID " & vbCrLf &
                    "           WHERE TransRentableAssets.ID = trans.ID) " & vbCrLf &
                    "   ELSE (SELECT DISTINCT STUFF((SELECT DISTINCT ', ' + " & vbCrLf &
                    "           ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) + ' ' + ISNULL(pubppol.PersonSurname, pricusppol.Surname) " & vbCrLf &
                    "           FROM TransRentableAssetCustomer as transcus " & vbCrLf &
                    "           LEFT JOIN MobizLibraryOrganization as pubppol ON pubppol.ID = transcus.ClientID " & vbCrLf &
                    "           LEFT JOIN LibraryPeople as pricusppol ON pricusppol.ID = transcus.ClientID " & vbCrLf &
                    "           WHERE transcus.TransRentableAssetID = trans.ID " & vbCrLf &
                    "           FOR XML PATH('')), 1, 1,'')) " & vbCrLf &
                    "   END as CustomerName " & vbCrLf &
                    " , pim.ProductName as AssetName, cat.C0020 as AssetType " & vbCrLf &
                    " FROM TransRentableAssets as trans " & vbCrLf &
                    " INNER JOIN LibraryRentableAssets as ass ON ass.ID = trans.AssetID " & vbCrLf &
                    " INNER JOIN MobizLibraryPimProduct as pim ON pim.ID = ass.ProductID " & vbCrLf &
                    " INNER JOIN MobizLibraryPIMProductVariableFeatures001 as cat ON cat.ProductID = ass.ProductID " & vbCrLf &
                    " WHERE (trans.DateReserved BETWEEN CAST(" & ReadyString(datefrom) & " AS DATE) AND DATEADD(DAY, 1, CAST(" & ReadyString(dateto) & " AS DATE))) " & vbCrLf &
                    " AND IsDone = 0 ORDER BY trans.DateReserved DESC"
            ElseIf type = "NoOfCancelled" Then
                pSql = " SELECT trans.* " & vbCrLf &
                    " , CASE WHEN (SELECT COUNT(ID) FROM TransRentableAssetCustomer WHERE TransRentableAssetID = trans.ID) = 0 " & vbCrLf &
                    "   THEN (SELECT ISNULL(pubppol.FirstName, pricusppol.PersonFirstName) + ' ' + ISNULL(pubppol.MiddleName, pricusppol.PersonMiddleName) + ' ' + ISNULL(pubppol.Surname, pricusppol.PersonSurname) " & vbCrLf &
                    "           FROM TransRentableAssets " & vbCrLf &
                    "           LEFT JOIN MobizLibraryPeople as pubppol ON pubppol.ID = TransRentableAssets.ClientID " & vbCrLf &
                    "           LEFT JOIN MobizLibraryOrganization as pricusppol ON pricusppol.ID = TransRentableAssets.ClientID " & vbCrLf &
                    "           WHERE TransRentableAssets.ID = trans.ID) " & vbCrLf &
                    "   ELSE (SELECT DISTINCT STUFF((SELECT DISTINCT ', ' + " & vbCrLf &
                    "           ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) + ' ' + ISNULL(pubppol.PersonSurname, pricusppol.Surname) " & vbCrLf &
                    "           FROM TransRentableAssetCustomer as transcus " & vbCrLf &
                    "           LEFT JOIN MobizLibraryOrganization as pubppol ON pubppol.ID = transcus.ClientID " & vbCrLf &
                    "           LEFT JOIN LibraryPeople as pricusppol ON pricusppol.ID = transcus.ClientID " & vbCrLf &
                    "           WHERE transcus.TransRentableAssetID = trans.ID " & vbCrLf &
                    "           FOR XML PATH('')), 1, 1,'')) " & vbCrLf &
                    "   END as CustomerName " & vbCrLf &
                    " , pim.ProductName as AssetName, cat.C0020 as AssetType " & vbCrLf &
                    " FROM TransRentableAssets as trans " & vbCrLf &
                    " INNER JOIN LibraryRentableAssets as ass ON ass.ID = trans.AssetID " & vbCrLf &
                    " INNER JOIN MobizLibraryPimProduct as pim ON pim.ID = ass.ProductID " & vbCrLf &
                    " INNER JOIN MobizLibraryPIMProductVariableFeatures001 as cat ON cat.ProductID = ass.ProductID " & vbCrLf &
                    " WHERE (trans.LastUpdated BETWEEN CAST(" & ReadyString(datefrom) & " AS DATE) AND DATEADD(DAY, 1, CAST(" & ReadyString(dateto) & " AS DATE))) " & vbCrLf &
                    " AND IsCancelled = 1 ORDER BY trans.LastUpdated DESC" & vbCrLf
            ElseIf type = "NoOfOccupied" Then
                pSql = " SELECT trans.* " & vbCrLf &
                    " , CASE WHEN (SELECT COUNT(ID) FROM TransRentableAssetCustomer WHERE TransRentableAssetID = trans.ID) = 0 " & vbCrLf &
                    "   THEN (SELECT ISNULL(pubppol.FirstName, pricusppol.PersonFirstName) + ' ' + ISNULL(pubppol.MiddleName, pricusppol.PersonMiddleName) + ' ' + ISNULL(pubppol.Surname, pricusppol.PersonSurname) " & vbCrLf &
                    "           FROM TransRentableAssets " & vbCrLf &
                    "           LEFT JOIN MobizLibraryPeople as pubppol ON pubppol.ID = TransRentableAssets.ClientID " & vbCrLf &
                    "           LEFT JOIN MobizLibraryOrganization as pricusppol ON pricusppol.ID = TransRentableAssets.ClientID " & vbCrLf &
                    "           WHERE TransRentableAssets.ID = trans.ID) " & vbCrLf &
                    "   ELSE (SELECT DISTINCT STUFF((SELECT DISTINCT ', ' + " & vbCrLf &
                    "           ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) + ' ' + ISNULL(pubppol.PersonSurname, pricusppol.Surname) " & vbCrLf &
                    "           FROM TransRentableAssetCustomer as transcus " & vbCrLf &
                    "           LEFT JOIN MobizLibraryOrganization as pubppol ON pubppol.ID = transcus.ClientID " & vbCrLf &
                    "           LEFT JOIN LibraryPeople as pricusppol ON pricusppol.ID = transcus.ClientID " & vbCrLf &
                    "           WHERE transcus.TransRentableAssetID = trans.ID " & vbCrLf &
                    "           FOR XML PATH('')), 1, 1,'')) " & vbCrLf &
                    "   END as CustomerName " & vbCrLf &
                    " , pim.ProductName as AssetName, cat.C0020 as AssetType " & vbCrLf &
                    " FROM TransRentableAssets as trans " & vbCrLf &
                    " INNER JOIN LibraryRentableAssets as ass ON ass.ID = trans.AssetID " & vbCrLf &
                    " INNER JOIN MobizLibraryPimProduct as pim ON pim.ID = ass.ProductID " & vbCrLf &
                    " INNER JOIN MobizLibraryPIMProductVariableFeatures001 as cat ON cat.ProductID = ass.ProductID " & vbCrLf &
                    " WHERE (--CAST(GETDATE() AS DATE) BETWEEN Trans.CheckedInUsedOn AND DATEADD(DAY, -1, CAST(trans.CheckedOutReturnedOn AS DATE)) OR " & vbCrLf &
                    " CAST(trans.CheckedInUsedOn AS DATE) BETWEEN CAST(" & ReadyString(datefrom) & " AS DATE) AND DATEADD(DAY, 1, CAST(" & ReadyString(dateto) & " AS DATE))" & vbCrLf &
                    " OR CAST(" & ReadyString(datefrom) & " AS DATE) BETWEEN CAST(trans.CheckedInUsedOn as DATE) AND CAST(trans.CheckedOutReturnedOn as DATE)" & vbCrLf &
                    " OR CAST(" & ReadyString(dateto) & " AS DATE) BETWEEN CAST(trans.CheckedInUsedOn as DATE) AND CAST(trans.CheckedOutReturnedOn as DATE))" & vbCrLf &
                    " AND IsDone = 0 ORDER BY trans.CheckedInUsedOn DESC" & vbCrLf
            ElseIf type = "NoOfUnoccupiedUnused" Then
                'pSql = " SELECT ass.ID, pim.ProductName as AssetName, '' as ReservedFrom, '' as ReservedUntil, '' as CustomerName " & vbCrLf &
                '    " FROM LibraryRentableAssets as ass " & vbCrLf &
                '    " INNER JOIN MobizLibraryPimProduct as pim ON pim.ID = ass.ProductID " & vbCrLf &
                '    " WHERE (ass.ID NOT IN (SELECT trans.AssetID " & vbCrLf &
                '    "                       FROM TransRentableAssets as trans " & vbCrLf &
                '    "                       WHERE (CAST(GETDATE() AS DATE) BETWEEN Trans.CheckedInUsedOn AND DATEADD(DAY, -1, CAST(trans.CheckedOutReturnedOn AS DATE)) " & vbCrLf &
                '    "                       OR trans.CheckedInUsedOn BETWEEN CAST(" & ReadyString(datefrom) & " AS DATE) AND DATEADD(DAY, 1, CAST(" & ReadyString(dateto) & " AS DATE))) " & vbCrLf &
                '    "                       AND IsDone = 0) " & vbCrLf &
                '    " --AND ass.ID NOT IN (SELECT trans.AssetID " & vbCrLf &
                '    " --                      FROM TransRentableAssets as trans " & vbCrLf &
                '    " --                      WHERE GETDATE() BETWEEN Trans.ReservedFrom AND trans.ReservedUntil " & vbCrLf &
                '    " --                      AND IsDone = 0) " & vbCrLf &
                '    " )AND pim.IsAsset = 1 ORDER BY pim.ProductName ASC "
                pSql = "SELECT * FROM( " & vbCrLf &
                        " SELECT ass.ID, cat.C0020 as AssetType, pim.ProductName as AssetName, '' as ReservedFrom, '' as ReservedUntil, '' as CustomerName " & vbCrLf &
                        " FROM LibraryRentableAssets as ass " & vbCrLf &
                        " INNER JOIN MobizLibraryPimProduct as pim ON pim.ID = ass.ProductID " & vbCrLf &
                        " INNER JOIN MobizLibraryPIMProductVariableFeatures001 as cat ON cat.ProductID = ass.ProductID " & vbCrLf &
                        " WHERE (ass.ID NOT IN (SELECT trans.AssetID " & vbCrLf &
                        "   FROM TransRentableAssets as trans " & vbCrLf &
                        "   WHERE (CAST(" & ReadyString(datefrom) & " AS DATE) BETWEEN trans.CheckedInUsedOn AND trans.CheckedOutReturnedOn" & vbCrLf &
                        "   OR CAST(" & ReadyString(dateto) & " AS DATE) BETWEEN trans.CheckedInUsedOn AND trans.CheckedOutReturnedOn" & vbCrLf &
                        "   OR trans.CheckedInUsedOn BETWEEN CAST(" & ReadyString(datefrom) & " AS DATE) AND DATEADD(DAY, 1, CAST(" & ReadyString(dateto) & " AS DATE))" & vbCrLf &
                        "   OR trans.CheckedOutReturnedOn BETWEEN CAST(" & ReadyString(datefrom) & " AS DATE) AND DATEADD(DAY, 1, CAST(" & ReadyString(dateto) & " AS DATE)))  " & vbCrLf &
                        "   AND IsDone = 0) " & vbCrLf &
                        " AND ass.ID NOT IN (SELECT trans.AssetID  " & vbCrLf &
                        "   FROM TransRentableAssets as trans " & vbCrLf &
                        "   WHERE (CAST(" & ReadyString(datefrom) & " AS DATE) BETWEEN Trans.ReservedFrom AND Trans.ReservedUntil" & vbCrLf &
                        "   OR CAST(" & ReadyString(dateto) & " AS DATE) BETWEEN Trans.ReservedFrom AND Trans.ReservedUntil) " & vbCrLf &
                        "   AND IsDone = 0)) AND pim.IsAsset = 1 " & vbCrLf &
                        " UNION ALL " & vbCrLf &
                        " SELECT ID, AssetType, AssetName, ReservedFrom, ReservedUntil, CustomerName FROM (SELECT ass.ID, cat.C0020 as AssetType, pim.ProductName as AssetName, ISNULL(convert(varchar, CAST(ReservedFrom AS DATE)), '') as ReservedFrom, ISNULL(convert(varchar, CAST(ReservedUntil AS DATE)), '') as ReservedUntil, " & vbCrLf &
                        " CheckedInUsedOn, (SELECT DISTINCT STUFF((SELECT DISTINCT ', ' + " & vbCrLf &
                        "   ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) + ' ' + ISNULL(pubppol.PersonSurname, pricusppol.Surname) " & vbCrLf &
                        "   FROM TransRentableAssetCustomer as transcus " & vbCrLf &
                        "   LEFT JOIN MobizLibraryOrganization as pubppol ON pubppol.ID = transcus.ClientID " & vbCrLf &
                        "   LEFT JOIN LibraryPeople as pricusppol ON pricusppol.ID = transcus.ClientID " & vbCrLf &
                        "   WHERE transcus.TransRentableAssetID = trans.ID " & vbCrLf &
                        "   FOR XML PATH('') " & vbCrLf &
                        " ), 1, 1,'')) as CustomerName " & vbCrLf &
                        " FROM TransRentableAssets as trans" & vbCrLf &
                        " INNER JOIN LibraryRentableAssets as ass ON ass.ID = trans.AssetID " & vbCrLf &
                        " INNER JOIN MobizLibraryPimProduct as pim ON pim.ID = ass.ProductID " & vbCrLf &
                        " INNER JOIN MobizLibraryPIMProductVariableFeatures001 as cat ON cat.ProductID = ass.ProductID " & vbCrLf &
                        " WHERE (CAST(" & ReadyString(datefrom) & " AS DATE) BETWEEN Trans.ReservedFrom AND Trans.ReservedUntil " & vbCrLf &
                        " OR CAST(" & ReadyString(dateto) & " AS DATE) BETWEEN Trans.ReservedFrom AND Trans.ReservedUntil)  " & vbCrLf &
                        " AND IsDone = 0 AND pim.IsAsset = 1) as t2 WHERE ISNULL(CheckedInUsedOn, '') = '') as T " & vbCrLf &
                        " ORDER BY AssetName ASC " & vbCrLf
            ElseIf type = "NoOfReserved" Then
                pSql = " SELECT * FROM (SELECT trans.* " & vbCrLf &
                    " , CASE WHEN (SELECT COUNT(ID) FROM TransRentableAssetCustomer WHERE TransRentableAssetID = trans.ID) = 0 " & vbCrLf &
                    "   THEN (SELECT ISNULL(pubppol.FirstName, pricusppol.PersonFirstName) + ' ' + ISNULL(pubppol.MiddleName, pricusppol.PersonMiddleName) + ' ' + ISNULL(pubppol.Surname, pricusppol.PersonSurname) " & vbCrLf &
                    "           FROM TransRentableAssets " & vbCrLf &
                    "           LEFT JOIN MobizLibraryPeople as pubppol ON pubppol.ID = TransRentableAssets.ClientID " & vbCrLf &
                    "           LEFT JOIN MobizLibraryOrganization as pricusppol ON pricusppol.ID = TransRentableAssets.ClientID " & vbCrLf &
                    "           WHERE TransRentableAssets.ID = trans.ID) " & vbCrLf &
                    "   ELSE (SELECT DISTINCT STUFF((SELECT DISTINCT ', ' + " & vbCrLf &
                    "           ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) + ' ' + ISNULL(pubppol.PersonSurname, pricusppol.Surname) " & vbCrLf &
                    "           FROM TransRentableAssetCustomer as transcus " & vbCrLf &
                    "           LEFT JOIN MobizLibraryOrganization as pubppol ON pubppol.ID = transcus.ClientID " & vbCrLf &
                    "           LEFT JOIN LibraryPeople as pricusppol ON pricusppol.ID = transcus.ClientID " & vbCrLf &
                    "           WHERE transcus.TransRentableAssetID = trans.ID " & vbCrLf &
                    "           FOR XML PATH('')), 1, 1,'')) " & vbCrLf &
                    "   END as CustomerName " & vbCrLf &
                    " , pim.ProductName as AssetName, cat.C0020 as AssetType " & vbCrLf &
                    " FROM TransRentableAssets as trans " & vbCrLf &
                    " INNER JOIN LibraryRentableAssets as ass ON ass.ID = trans.AssetID " & vbCrLf &
                    " INNER JOIN MobizLibraryPimProduct as pim ON pim.ID = ass.ProductID " & vbCrLf &
                    " INNER JOIN MobizLibraryPIMProductVariableFeatures001 as cat ON cat.ProductID = ass.ProductID " & vbCrLf &
                    " WHERE (CAST(" & ReadyString(datefrom) & " AS DATE) BETWEEN CAST(Trans.ReservedFrom as Date) AND CAST(Trans.ReservedUntil as Date)" & vbCrLf &
                    " OR CAST(" & ReadyString(dateto) & " AS DATE) BETWEEN CAST(Trans.ReservedFrom as Date) AND CAST(Trans.ReservedUntil as Date))) as T WHERE IsDone = 0 AND ISNULL(CheckedInUsedOn, '') = '' ORDER BY ReservedFrom DESC" & vbCrLf
            End If

            Return pSql
        End Function

        Function SaveAssetNow() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim data As Object = Common.WebRequest.AllData("data")
            Dim result As String = jss.DeserializeObject(SaveAssetTrans(data, False))("d")("Results") '' Generated trans id

            If IsGuid(result) And data("Action") <> "checkout" And data("Action") <> "cancel" Then
                If CBool2(data("IsTransfer")) And data("Action") = "checkin" Then
                    data("TransID") = ""
                    data("IsTransfer") = False

                    result = jss.DeserializeObject(SaveAssetTrans(data, True))("d")("Results")

                    Return JsonDictionary.Serialize("Results", result)
                Else
                    Return JsonDictionary.Serialize("Results", result)
                End If
            Else
                Return JsonDictionary.Serialize("Results", result)
            End If
        End Function

        Function SaveAssetCustomer(ByVal data As Object, ByVal transID As String, Optional ByVal IsTransferCheckInNow As Boolean = False) As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            'Dim data As Object = Common.WebRequest.AllData("data")            
            Dim guestID As New List(Of String)()
            Dim p As New InsertUpdate("TransRentableAssetCustomerRequest")
            Dim sortorderno As Integer = 0
            AppendToDebug("vbHSRRFTXPM7! SaveAssetCustomer", True, , True)
            'Common.WebRequest.ProData.BeginTransaction()

            Try
                For Each ppoldata As Object In data("Customer")
                    Dim pPri As New InsertUpdate("LibraryPeople")
                    Dim pPub As New InsertUpdate("MobizLibraryOrganization")
                    Dim ppolID As String = Guid.NewGuid.ToString()
                    Dim isExist As Integer = 0

                    sortorderno += 1

                    pPri("FirstName") = ReadyString(ppoldata("FirstName"))
                    pPri("MiddleName") = ReadyString(ppoldata("MiddleName"))
                    pPri("Surname") = ReadyString(ppoldata("Surname"))
                    pPri("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    pPri("LastUpdated") = "getdate()"
                    pPri("IsActive") = ReadyString("True")
                    pPri("DateOfBirth") = IIf(ppoldata("DateOfBirth") = "", "NULL", ReadyString(ppoldata("DateOfBirth")))

                    If ppoldata("ClientID") <> "" Then 'CHECK IF ALREADY EXIST
                        pSql = "SELECT ID FROM LibraryPeople WHERE ID = " & ReadyString(ppoldata("ClientID"))
                        isExist = Common.WebRequest.ProData.Execute(pSql).Rows.Count
                    End If

                    If ppoldata("ClientID") = "" Or isExist = 0 Then
                        pPri("ID") = ReadyString(ppolID)
                        pSql = pPri.GetInsert()
                        Common.WebRequest.ProData.Execute(pSql)
                    Else
                        ppolID = ppoldata("ClientID")
                        pSql = pPri.GetUpdate(" ID = " & ReadyString(ppolID))
                        Common.WebRequest.ProData.Execute(pSql)
                    End If

                    isExist = 0

                    pPub("PersonFirstName") = ReadyString(ppoldata("FirstName"))
                    pPub("PersonMiddleName") = ReadyString(ppoldata("MiddleName"))
                    pPub("PersonSurname") = ReadyString(ppoldata("Surname"))
                    pPub("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    pPub("LastUpdated") = "getdate()"
                    pPub("IsOrganization") = "0"
                    pPub("IsActive") = ReadyString("True")
                    pPub("PersonDateOfBirth") = IIf(ppoldata("DateOfBirth") = "", "NULL", ReadyString(ppoldata("DateOfBirth")))

                    If ppoldata("ClientID") <> "" Then ' CHECK IF ALREADY EXIST
                        pSql = "SELECT ID FROM MobizLibraryOrganization WHERE ID = " & ReadyString(ppoldata("ClientID"))
                        isExist = Common.WebRequest.ProData.Execute(pSql).Rows.Count
                    End If

                    If ppoldata("ClientID") = "" Or isExist = 0 Then
                        pPub("ID") = ReadyString(ppolID)
                        pSql = pPub.GetInsert()
                        Common.WebRequest.ProData.Execute(pSql)

                        pPub = New InsertUpdate("LibraryOrganizationCustomer")

                        pPub("ID") = ReadyString(Guid.NewGuid.ToString())
                        pPub("OrganizationID") = ReadyString(ppolID)
                        pPub("ActivationDate") = "GetDate()"
                        pPub("DeActivated") = "0"
                        pPub("IsBan") = ReadyString("0")
                        pPub("LastUpdated") = "GetDate()"

                        pPub("SalesRepID") = "NULL"
                        pPub("PriceLevelID") = "NULL"
                        pPub("CreditTermsID") = "NULL"
                        pPub("CreditLimit") = ReadyString("")
                        pPub("CreditBegBalance") = ReadyString("")
                        pPub("CreditBalanceAsOf") = ReadyString("")
                        pSql = pPub.GetInsert()

                        Common.WebRequest.ProData.ExecuteSQL(pSql)
                    Else
                        ppolID = ppoldata("ClientID")
                        pSql = pPub.GetUpdate(" ID = " & ReadyString(ppolID))
                        Common.WebRequest.ProData.Execute(pSql)
                    End If

                    isExist = 0

                    '\\\\\\\\\\\\\\\\\\\\\\\\\//////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\////////////////////////

                    pPri = New InsertUpdate("LibraryPeopleAddress")
                    pPub = New InsertUpdate("MobizLibraryPeopleAddress")
                    Dim addressid As String = Guid.NewGuid.ToString()

                    pPri("PeopleID") = ReadyString(ppolID)
                    pPri("HouseBuildingNo") = ReadyString(ppoldata("HouseBuildingNo"))
                    pPri("Street") = ReadyString(ppoldata("Street"))

                    If ppoldata("CityID") <> "" Then
                        pPri("CityID") = ReadyString(ppoldata("CityID"))
                    End If

                    If ppoldata("CountryID") <> "" Then
                        pPri("CountryID") = ReadyString(ppoldata("CountryID"))
                    End If

                    pPri("AddressType") = ReadyString("Permanent Address")
                    pPri("LastUpdated") = "getdate()"

                    If ppoldata("AddressID") <> "" Then 'CHECK IF ALREADY EXIST
                        addressid = ppoldata("AddressID")
                        pSql = "SELECT ID FROM LibraryPeopleAddress WHERE ID = " & ReadyString(ppoldata("AddressID"))
                        isExist = Common.WebRequest.ProData.Execute(pSql).Rows.Count
                    End If

                    If ppoldata("AddressID") = "" Or isExist = 0 Then
                        pPri("ID") = ReadyString(addressid)
                        pSql = pPri.GetInsert()
                        Common.WebRequest.ProData.Execute(pSql)
                    Else
                        pSql = pPri.GetUpdate(" ID = " & ReadyString(ppoldata("AddressID")))
                        Common.WebRequest.ProData.Execute(pSql)
                    End If

                    isExist = 0

                    pPub("PeopleID") = ReadyString(ppolID)
                    pPub("HouseBuildingNo") = ReadyString(ppoldata("HouseBuildingNo"))
                    pPub("Street") = ReadyString(ppoldata("Street"))

                    If ppoldata("CityID") <> "" Then
                        pPub("CityID") = ReadyString(ppoldata("CityID"))
                    End If

                    If ppoldata("CountryID") <> "" Then
                        pPub("CountryID") = ReadyString(ppoldata("CountryID"))
                    End If

                    pPub("AddressType") = ReadyString("Permanent Address")
                    pPub("LastUpdated") = "getdate()"

                    If ppoldata("AddressID") <> "" Then 'CHECK IF ALREADY EXIST
                        addressid = ppoldata("AddressID")
                        pSql = "SELECT ID FROM MobizLibraryPeopleAddress WHERE ID = " & ReadyString(ppoldata("AddressID"))
                        isExist = Common.WebRequest.ProData.Execute(pSql).Rows.Count
                    End If

                    If ppoldata("AddressID") = "" Or isExist = 0 Then
                        pPub("ID") = ReadyString(addressid)
                        pSql = pPub.GetInsert()
                        Common.WebRequest.ProData.Execute(pSql)
                    Else
                        pSql = pPub.GetUpdate(" ID = " & ReadyString(ppoldata("AddressID")))
                        Common.WebRequest.ProData.Execute(pSql)
                    End If

                    isExist = 0

                    '\\\\\\\\\\\\\\\\\\\\\\\\\//////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\////////////////////////

                    pPri = New InsertUpdate("LibraryPeopleContactMobile")
                    pPub = New InsertUpdate("MobizLibraryPeopleContactMobile")
                    Dim mobileid As String = Guid.NewGuid.ToString()

                    pPri("PeopleID") = ReadyString(ppolID)
                    pPri("CountryID") = ReadyString(ppoldata("CountryCodeID"))
                    pPri("TelCoPrefixID") = ReadyString(WebRequest.Instance.Business.LibraryTelcoPrefixes.GetOrCreateByMobileNumber(ppoldata("ContactNo"))("ID"))
                    pPri("ContactNo") = ReadyString(LibraryTelcoPrefixes.ParseNumber(ppoldata("ContactNo")))
                    pPri("ContactType") = ReadyString("Personal Mobile")
                    pPri("LastUpdated") = "getdate()"

                    If ppoldata("MobileID") <> "" Then 'CHECK IF ALREADY EXIST
                        mobileid = ppoldata("MobileID")
                        pSql = "SELECT ID FROM LibraryPeopleContactMobile WHERE ID = " & ReadyString(ppoldata("MobileID"))
                        isExist = Common.WebRequest.ProData.Execute(pSql).Rows.Count
                    End If

                    If ppoldata("MobileID") = "" Or isExist = 0 Then
                        pPri("ID") = ReadyString(mobileid)
                        pSql = pPri.GetInsert()
                        Common.WebRequest.ProData.Execute(pSql)
                    Else
                        pSql = pPri.GetUpdate(" ID = " & ReadyString(ppoldata("MobileID")))
                        Common.WebRequest.ProData.Execute(pSql)
                    End If

                    isExist = 0

                    pPub("PeopleID") = ReadyString(ppolID)
                    pPub("CountryID") = ReadyString(ppoldata("CountryCodeID"))
                    pPub("TelCoPrefixID") = ReadyString(WebRequest.Instance.Business.LibraryTelcoPrefixes.GetOrCreateByMobileNumber(ppoldata("ContactNo"))("ID"))
                    pPub("ContactNo") = ReadyString(LibraryTelcoPrefixes.ParseNumber(ppoldata("ContactNo")))
                    pPub("ContactType") = ReadyString("Personal Mobile")
                    pPub("LastUpdated") = "getdate()"

                    If ppoldata("MobileID") <> "" Then 'CHECK IF ALREADY EXIST
                        mobileid = ppoldata("MobileID")
                        pSql = "SELECT ID FROM MobizLibraryPeopleContactMobile WHERE ID = " & ReadyString(ppoldata("MobileID"))
                        isExist = Common.WebRequest.ProData.Execute(pSql).Rows.Count
                    End If

                    If ppoldata("MobileID") = "" Or isExist = 0 Then
                        pPub("ID") = ReadyString(mobileid)
                        pSql = pPub.GetInsert()
                        Common.WebRequest.ProData.Execute(pSql)
                    Else
                        pSql = pPub.GetUpdate(" ID = " & ReadyString(ppoldata("MobileID")))
                        Common.WebRequest.ProData.Execute(pSql)
                    End If

                    isExist = 0

                    '\\\\\\\\\\\\\\\\\\\\\\\\\//////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\////////////////////////

                    pPri = New InsertUpdate("LibraryPeopleContactEmail")
                    pPub = New InsertUpdate("MobizLibraryPeopleContactEmail")
                    Dim emailaddid As String = Guid.NewGuid.ToString()

                    If ppoldata("EmailAddress") <> "" Then
                        pPri("PeopleID") = ReadyString(ppolID)
                        pPri("EmailAddress") = ReadyString(ppoldata("EmailAddress"))
                        pPri("EmailType") = ReadyString("Personal Email")
                        pPri("LastUpdated") = "getdate()"

                        If ppoldata("EmailID") <> "" Then 'CHECK IF ALREADY EXIST
                            emailaddid = ppoldata("EmailID")
                            pSql = "SELECT ID FROM LibraryPeopleContactEmail WHERE ID = " & ReadyString(ppoldata("EmailID"))
                            isExist = Common.WebRequest.ProData.Execute(pSql).Rows.Count
                        End If

                        If ppoldata("EmailID") = "" Or isExist = 0 Then
                            pPri("ID") = ReadyString(emailaddid)
                            pSql = pPri.GetInsert()
                            Common.WebRequest.ProData.Execute(pSql)
                        Else
                            pSql = pPri.GetUpdate(" ID = " & ReadyString(ppoldata("EmailID")))
                            Common.WebRequest.ProData.Execute(pSql)
                        End If
                    End If

                    isExist = 0

                    If ppoldata("EmailAddress") <> "" Then
                        pPub("PeopleID") = ReadyString(ppolID)
                        pPub("EmailAddress") = ReadyString(ppoldata("EmailAddress"))
                        pPub("EmailType") = ReadyString("Personal Email")
                        pPub("LastUpdated") = "getdate()"

                        If ppoldata("EmailID") <> "" Then 'CHECK IF ALREADY EXIST
                            emailaddid = ppoldata("EmailID")
                            pSql = "SELECT ID FROM MobizLibraryPeopleContactEmail WHERE ID = " & ReadyString(ppoldata("EmailID"))
                            isExist = Common.WebRequest.ProData.Execute(pSql).Rows.Count
                        End If

                        If ppoldata("EmailID") = "" Or isExist = 0 Then
                            pPub("ID") = ReadyString(emailaddid)
                            pSql = pPub.GetInsert()
                            Common.WebRequest.ProData.Execute(pSql)
                        Else
                            pSql = pPub.GetUpdate(" ID = " & ReadyString(ppoldata("EmailID")))
                            Common.WebRequest.ProData.Execute(pSql)
                        End If
                    End If

                    If ppoldata.ContainsKey("Request") Then
                        For Each req As Object In ppoldata("Request")
                            If req("State") = "new" Or req("State") = "edit" Or req("State") = "modified" Then
                                p = New InsertUpdate("TransRentableAssetCustomerRequest")

                                p("TransRentableAssetID") = ReadyString(transID)
                                p("Request") = ReadyString(req("Request"))

                                If ppoldata.ContainsKey("ClientID") Then
                                    p("ClientID") = ReadyString(IIf(ppoldata("ClientID") = "", ppolID, ppoldata("ClientID")))
                                End If

                                If req("State") = "modified" Then
                                    p("Action") = ReadyString("Modified")
                                    p("ParentHistoryID") = ReadyString(req("ID"))

                                    pSql = "UPDATE TransRentableAssetCustomerRequest SET IsActive = 'False' WHERE ID = " & ReadyString(req("ID"))
                                    Common.WebRequest.ProData.Execute(pSql)
                                End If

                                If req("State") = "new" Then
                                    p("Action") = ReadyString("Added")
                                End If

                                If req("State") = "new" Or req("State") = "modified" Then
                                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                    p("LastUpdated") = "getdate()"
                                    p("IsDone") = ReadyString("False")
                                    p("IsActive") = ReadyString("True")

                                    pSql = p.GetInsert()

                                    Common.WebRequest.ProData.Execute(pSql)
                                End If
                            Else
                                p = New InsertUpdate("TransRentableAssetCustomerRequest")

                                p("Action") = ReadyString("Deleted")
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                p("LastUpdated") = "getdate()"

                                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(req("ID"))))
                            End If
                        Next req
                    End If

                    p = New InsertUpdate("TransRentableAssetCustomer")

                    pSql = "SELECT ID FROM TransRentableAssetCustomer WHERE TransRentableAssetID = " & ReadyString(transID) & vbCrLf &
                            " AND ClientID = " & ReadyString(ppolID)

                    If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        p("TransRentableAssetID") = ReadyString(transID)
                        p("ClientID") = ReadyString(ppolID)
                        p("SortOrderNo") = sortorderno

                        guestID.Add(ppolID)

                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    Else
                        If ppoldata.ContainsKey("Status") Then
                            If ppoldata("Status") = "removed" Then
                                pSql = "DELETE FROM TransRentableAssetCustomer WHERE ClientID = " & ReadyString(ppolID) & vbCrLf &
                                        " AND TransRentableAssetID = " & ReadyString(transID)
                                Common.WebRequest.ProData.Execute(pSql)
                                sortorderno -= 1
                            Else
                                guestID.Add(ppolID)
                            End If
                        Else
                            guestID.Add(ppolID)
                        End If
                    End If

                    If data.ContainsKey("ClientOrgID") Then
                        If data("ClientOrgID") <> "" Then
                            pSql = "SELECT ID FROM LibraryOrganizationContactPerson WHERE OrganizationID = " & ReadyString(data("ClientOrgID")) & vbCrLf &
                                    " AND PeopleID = " & ReadyString(IIf(ppoldata("ClientID") = "", ppolID, ppoldata("ClientID")))

                            If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                                p = New InsertUpdate("LibraryOrganizationContactPerson")

                                p("ID") = ReadyString(Guid.NewGuid.ToString())
                                p("PeopleID") = ReadyString(IIf(ppoldata("ClientID") = "", ppolID, ppoldata("ClientID")))
                                p("OrganizationID") = ReadyString(data("ClientOrgID"))
                                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                                p("LastUpdated") = "getdate()"
                                p("IsUseForPersonOrgAffiliates") = ReadyString(0)

                                Common.WebRequest.ProData.Execute(p.GetInsert())
                            End If

                        End If
                    End If
                Next ppoldata

                p = New InsertUpdate("TransRentableAssets")

                p("ClientID") = ReadyString(guestID(0).ToString()) 'ibutang ang pinakauna na guest para mahimoan ug invoice

                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(transID)))

                'Dim savetransresult As String = jss.DeserializeObject(SaveAssetTrans("", IsTransferCheckInNow))("d")("Results")

                'If IsGuid(savetransresult) Then
                '    'WebRequest.Instance.Business.LibraryRepeatingTransactions.Handle1Org(WebRequest.Instance.OrgID)
                '    Common.WebRequest.ProData.CommitTransaction()
                'Else
                '    Common.WebRequest.ProData.RollbackTransaction()
                'End If

                'Return savetransresult
                Return "saved"
            Catch ex As Exception
                AppendToDebug("vbCX3JLBH4VD! " & ex.Message, True, ex, True)
                Return ex.Message
            End Try

        End Function

        Function GetAssetsSchedule() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = " SELECT ass.ID " & vbCrLf &
                    " , pim.ProductNo as AssetNo " & vbCrLf &
                    " , pim.ProductName as AssetName " & vbCrLf &
                    " , cat.C0020 as AssetType " & vbCrLf &
                    " FROM LibraryRentableAssets as ass " & vbCrLf &
                    " INNER JOIN MobizLibraryPimProduct as pim ON pim.ID = ass.ProductID " & vbCrLf &
                    " INNER JOIN MobizLibraryPIMProductVariableFeatures001 as cat ON cat.ProductID = ass.ProductID " & vbCrLf
            Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)
            Dim assets As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDT))

            For i As Integer = 0 To assets.Count - 1

            Next

            Return JsonDictionary.Serialize("Results", pDT)
        End Function

        Function SaveDoorLockSettings() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim data As Object = Common.WebRequest.AllData("data")
            Dim p As New InsertUpdate("SystemDefaults")

            p("Description") = ReadyString("DoorLockSystem")
            p("DefaultValue") = ReadyString(data("DoorLock"))
            p("LastUpdated") = "getdate()"

            If data("DoorLockID").ToString() = "" Then
                p("ID") = ReadyString(Guid.NewGuid.ToString())
                pSql = p.GetInsert()
            Else
                pSql = p.GetUpdate(" ID = " & ReadyString(data("DoorLockID").ToString()))
            End If

            Common.WebRequest.ProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", "saved")
        End Function

        Function SaveSettings() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Common.WebRequest.ProData.BeginTransaction()

                Dim data As Object = Common.WebRequest.AllData("data")
                Dim p As New InsertUpdate("SystemDefaults")

                p("Description") = ReadyString("AssMgmtLabelSetting")
                p("DefaultValue") = ReadyString(data("LabelSetting"))
                p("LastUpdated") = "getdate()"

                If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate("Description = " & ReadyString("AssMgmtLabelSetting"))) = 0 Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                    Common.WebRequest.ProData.Execute(pSql)
                End If

                p = New InsertUpdate("SystemDefaults")
                p("Description") = ReadyString("AssMgmtCustomLabelForAssets")
                p("DefaultValue") = ReadyString(data("CustomLableForAssets"))
                p("LastUpdated") = "getdate()"

                If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate("Description = " & ReadyString("AssMgmtCustomLabelForAssets"))) = 0 Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                    Common.WebRequest.ProData.Execute(pSql)
                End If

                p = New InsertUpdate("SystemDefaults")
                p("Description") = ReadyString("AssMgmtCustomLabelForCustomers")
                p("DefaultValue") = ReadyString(data("CustomLableForCustomers"))
                p("LastUpdated") = "getdate()"

                If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate("Description = " & ReadyString("AssMgmtCustomLabelForCustomers"))) = 0 Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                    Common.WebRequest.ProData.Execute(pSql)
                End If

                p = New InsertUpdate("SystemDefaults")
                p("Description") = ReadyString("CheckOutTimeDefault")
                p("DefaultValue") = ReadyString(data("CheckOutTime"))
                p("LastUpdated") = "getdate()"

                If data("CheckOutTimeID").ToString() = "" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(data("CheckOutTimeID").ToString()))
                End If

                Common.WebRequest.ProData.Execute(pSql)

                p = New InsertUpdate("SystemDefaults")
                p("Description") = ReadyString("IsAllowOutdatedReservation")
                p("DefaultValue") = ReadyString(data("IsAllowOutdatedRes"))
                p("LastUpdated") = "getdate()"

                If data("OutdatedResID").ToString() = "" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(data("OutdatedResID").ToString()))
                End If

                Common.WebRequest.ProData.Execute(pSql)

                p = New InsertUpdate("SystemDefaults")
                p("Description") = ReadyString("IsAllowFutureDatedCheckIns")
                p("DefaultValue") = ReadyString(data("IsAllowFutureDatedCheckIns"))
                p("LastUpdated") = "getdate()"

                If data("FutureDatedCheckInsID").ToString() = "" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(data("FutureDatedCheckInsID").ToString()))
                End If

                Common.WebRequest.ProData.Execute(pSql)

                p = New InsertUpdate("SystemDefaults")
                p("Description") = ReadyString("MaxNumberOfGuestCards")
                p("DefaultValue") = ReadyString(data("MaxNumberOfGuestCards"))
                p("LastUpdated") = "getdate()"

                If data("MaxNumberOfGuestCardsID").ToString() = "" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(data("MaxNumberOfGuestCardsID").ToString()))
                End If

                Common.WebRequest.ProData.Execute(pSql)

                p = New InsertUpdate("SystemDefaults")
                p("Description") = ReadyString("AllowLostCardCreation")
                p("DefaultValue") = ReadyString(data("AllowLostCardCreation"))
                p("LastUpdated") = "getdate()"

                If data("AllowLostCardCreationID").ToString() = "" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(data("AllowLostCardCreationID").ToString()))
                End If

                Common.WebRequest.ProData.Execute(pSql)

                p = New InsertUpdate("SystemDefaults")
                p("Description") = ReadyString("MaximumNumberOfLostCard")
                p("DefaultValue") = ReadyString(data("MaximumNumberOfLostCard"))
                p("LastUpdated") = "getdate()"

                If data("MaximumNumberOfLostCardID").ToString() = "" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(data("MaximumNumberOfLostCardID").ToString()))
                End If

                Common.WebRequest.ProData.Execute(pSql)

                p = New InsertUpdate("SystemDefaults")
                p("Description") = ReadyString("DefaultChargeOfLostCard")
                p("DefaultValue") = ReadyString(data("DefaultChargeOfLostCard"))
                p("LastUpdated") = "getdate()"

                If data("DefaultChargeOfLostCardID").ToString() = "" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(data("DefaultChargeOfLostCardID").ToString()))
                End If

                Common.WebRequest.ProData.Execute(pSql)

                p = New InsertUpdate("SystemDefaults")
                p("Description") = ReadyString("CreateKeyCardAfterCheckIn")
                p("DefaultValue") = ReadyString(data("CreateKeyCardAfterCheckIn"))
                p("LastUpdated") = "getdate()"

                If data("CreateKeyCardAfterCheckInID").ToString() = "" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(data("CreateKeyCardAfterCheckInID").ToString()))
                End If

                Common.WebRequest.ProData.Execute(pSql)

                p = New InsertUpdate("SystemDefaults")
                p("Description") = ReadyString("DisplayProductNoInAssets")
                p("DefaultValue") = ReadyString(data("DisplayProductNoInAssets"))
                p("LastUpdated") = "getdate()"

                If data("DisplayProductNoInAssetsID").ToString() = "" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(data("DisplayProductNoInAssetsID").ToString()))
                End If

                Common.WebRequest.ProData.Execute(pSql)

                WebRequest.Instance.Business.SystemDefaults.CheckInTimeDefault = data("CheckInTime")
                WebRequest.Instance.Business.SystemDefaults.EarlyCheckInTime = data("EarlyCheckInTime")
                WebRequest.Instance.Business.SystemDefaults.EarlyCheckInProductID = (data("EarlyCheckInProductID"))
                WebRequest.Instance.Business.SystemDefaults.LateCheckOutTime = data("LateCheckOutTime")
                WebRequest.Instance.Business.SystemDefaults.LateCheckOutProductID = (data("LateCheckOutProductID"))
                WebRequest.Instance.Business.SystemDefaults.DisplayAssetPrice = (data("DisplayAssetPrice"))
                WebRequest.Instance.Business.SystemDefaults.AssetMgmtDownpaymentRequired = (data("DownpaymentRequired"))
                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")

            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function SaveAssetTrans(Optional ByVal pdata As Object = Nothing, Optional ByVal IsTransferCheckInNow As Boolean = False) As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Common.WebRequest.ProData.BeginTransaction()

                Dim data As Object = IIf(pdata.Count = 0, Common.WebRequest.AllData("data"), pdata)
                Dim p As New InsertUpdate("TransRentableAssets")
                Dim transID As String = data("TransID").ToString()
                Dim tempTransID As String = Guid.NewGuid.ToString()
                Dim CheckOutTime As String = "12:00 PM"
                Dim CheckInTime As String = "2:00 PM"
                pSql = "SELECT DefaultValue FROM SystemDefaults WHERE Description = 'CheckOutTimeDefault'"
                Dim pDtDefaultCheckOutTime As DataTable = Common.WebRequest.ProData.Execute(pSql)

                pSql = "SELECT DefaultValue FROM SystemDefaults WHERE Description = 'CheckInTimeDefault'"
                Dim pDtDefaultCheckInTime As DataTable = Common.WebRequest.ProData.Execute(pSql)

                If pDtDefaultCheckOutTime.Rows.Count > 0 Then
                    CheckOutTime = pDtDefaultCheckOutTime.Rows(0)("DefaultValue") 'Convert.ToDateTime().AddMinutes(65).ToString("hh:mm tt")
                End If

                If pDtDefaultCheckInTime.Rows.Count > 0 Then
                    CheckInTime = Format(WebRequest.Instance.Business.SystemDefaults.CheckInTimeDefault, cGlobal.FormatTime) 'Convert.ToDateTime().AddMinutes(65).ToString("hh:mm tt")
                End If

                p("CreatedBy") = ReadyString(WebSession.Instance.PeopleUserID) 'aka UPDATED BY
                p("LastUpdated") = "getdate()"

                If data("Action") = "donecleaning" Then
                    p("IsDoneCleaning") = ReadyString("True")
                    pSql = p.GetUpdate(" ID = " & ReadyString(transID))
                    Common.WebRequest.ProData.Execute(pSql)

                    pSql = "SELECT ISNULL(convert(varchar(max), TransferFromID), '') as TransferFromID FROM TransRentableAssets WHERE ID = " & ReadyString(transID)
                    Dim transferfromid As String = Common.WebRequest.ProData.Execute(pSql).Rows(0)("TransferFromID")

                    If CStr2(transferfromid) <> "" Then
                        p = New InsertUpdate("TransRentableAssets")

                        p("IsDoneCleaning") = ReadyString("True")
                        pSql = p.GetUpdate(" ID = " & ReadyString(transferfromid) & " OR TransferFromID = " & ReadyString(transferfromid))
                        Common.WebRequest.ProData.Execute(pSql)
                    End If

                    Common.WebRequest.ProData.CommitTransaction()

                    Return JsonDictionary.Serialize("Results", "saved")
                End If

                If data("Action") <> "checkout" And data("Action") <> "cancel" And data("Action") <> "outoforder" Then
                    p("IsDoneCleaning") = ReadyString("False")
                    p("IsOutOfOrder") = ReadyString("False")
                    'If data.ContainsKey("ClientID") Then
                    '    p("ClientID") = ReadyString(IIf(data("ClientID") = "", PpolID, data("ClientID")))
                    'End If
                    If data.ContainsKey("AssetID") Then
                        If IsTransferCheckInNow Then
                            p("AssetID") = ReadyString(data("TransferToAssetID"))
                        Else
                            p("AssetID") = ReadyString(data("AssetID"))
                        End If
                    End If

                    If Not CBool2(data("IsTransfer")) Then
                        If data.ContainsKey("NoOfAdults") Then
                            p("NoOfAdults") = ReadyString(data("NoOfAdults"))
                        End If
                        If data.ContainsKey("NoOfChild") Then
                            p("NoOfChild") = ReadyString(data("NoOfChild"))
                        End If
                        If data.ContainsKey("NoOfGuestCards") Then
                            p("NoOfGuestCards") = ReadyString(data("NoOfGuestCards"))
                        End If
                        If data.ContainsKey("ClientOrgID") Then
                            If data("ClientOrgID") <> "" Then
                                p("OrgID") = ReadyString(data("ClientOrgID"))
                            End If
                        End If
                        If data.ContainsKey("PriceLevelID") Then
                            p("PriceLevelID") = IIf(CStr2(data("PriceLevelID")) = "" Or CStr2(data("PriceLevelID")) = "freetype", "NULL", ReadyString(data("PriceLevelID")))
                        End If
                    End If

                    If CBool2(data("IsTransfer")) Then
                        p("IsDone") = ReadyString("True")
                        p("IsDoneCleaning") = ReadyString("False")
                        p("TransferDate") = "getdate()"
                    End If

                    If IsTransferCheckInNow Then
                        p("TransferFromID") = ReadyString(data("TransferFromID"))
                    End If
                End If

                If data("Action") = "reserve" Then
                    If transID = "" Then
                        p("DateReserved") = "getdate()"
                        p("ReservationWasMadeBy") = ReadyString(WebSession.Instance.PeopleUserID)
                    End If

                    p("ReservedFrom") = ReadyString(data("DateFrom"))
                    p("ReservedUntil") = ReadyString(data("DateTo"))
                    p("IsDone") = ReadyString("False")

                    'pSql = "SELECT ID FROM TransRentableAssets " & vbCrLf &
                    '        " WHERE (" & ReadyString(Format(CDate(data("DateFrom")), "MMM dd, yyyy") & " " & CheckOutTime) & " BETWEEN ReservedFrom AND ReservedUntil " & vbCrLf &
                    '        " OR " & ReadyString(data("DateTo")) & " BETWEEN ReservedFrom AND ReservedUntil) AND IsDone = 0 " & vbCrLf &
                    '        " AND AssetID = " & ReadyString(data("AssetID")) & vbCrLf &
                    '        If(transID = "", "", " AND ID <> " & ReadyString(transID))

                    pSql = "SELECT ID FROM TransRentableAssets " & vbCrLf &
                            " WHERE ((ReservedFrom >=" & ReadyString(Format(CDate(data("DateFrom")), "MMM dd, yyyy ") & CheckInTime) & " AND ReservedUntil <= " & ReadyString(Format(CDate(data("DateTo")), "MMM dd, yyyy ") & CheckOutTime) & ") " & vbCrLf &
                            " --OR (ReservedFrom <" & ReadyString(data("DateTo")) & "AND ReservedUntil >= " & ReadyString(data("DateTo")) & ")" & vbCrLf &
                            ") AND IsDone = 0 " & vbCrLf &
                            " AND ISNULL(TransferDate, '') = '' " & vbCrLf &
                            " AND AssetID = " & ReadyString(data("AssetID")) & vbCrLf &
                            If(transID = "", "", " AND ID <> " & ReadyString(transID))

                ElseIf data("Action") = "cancel" Then
                    p("DateCancelled") = "getdate()"
                    p("IsCancelled") = ReadyString("True")
                    p("IsDone") = ReadyString("True")
                    p("IsDoneCleaning") = ReadyString("False")
                    p("IsOutOfOrder") = ReadyString("False")
                ElseIf data("Action") = "checkin" Then
                    Dim earliestcheckin = data("DateNow") & " " & Format(WebRequest.Instance.Business.SystemDefaults.EarlyCheckInTime, cGlobal.FormatTime)
                    Dim currenttime = data("DateNow") & " " & data("TimeNow")

                    'commented by arnie feb 11, 2020 10:33 AM
                    'CheckedInUsedOn aka "AdjustedCheckinUsedOn", pwede ni maadjust ang date depende kung on the day nagcheckin or late checkin (previous date ang ibutang)
                    'ActualCheckedInUsedOn mao ni ang actual jud na nagcheckin si guest

                    If CBool2(data("IsFromReserved")) Then
                        If CDate(earliestcheckin) > CDate(currenttime) Then
                            ' kung late checkin, dapat previous date ang ibutang kay bayaran pa niya ang gahapon na rate
                            p("CheckedInUsedOn") = ReadyString(Format(CDate(data("DateNow")).AddDays(-1), "MMM dd, yyyy"))
                        Else
                            p("CheckedInUsedOn") = "getdate()"
                        End If

                        p("ActualCheckedInUsedOn") = "getdate()"
                        p("CheckedInByUser") = ReadyString(WebSession.Instance.PeopleUserID)
                    Else
                        If IsTransferCheckInNow Then
                            p("CheckedInUsedOn") = "getdate()"
                            p("ActualCheckedInUsedOn") = "getdate()"
                        Else
                            If CBool2(data("IsTransfer")) = False Then
                                If CBool2(data("IsAlreadyCheckin")) = False Then
                                    If CDate(earliestcheckin) > CDate(currenttime) Then
                                        p("CheckedInUsedOn") = ReadyString(Format(CDate(data("DateNow")).AddDays(-1), "MMM dd, yyyy"))
                                    Else
                                        p("CheckedInUsedOn") = ReadyString(data("DateFrom"))
                                    End If

                                    p("ActualCheckedInUsedOn") = "getdate()"
                                End If
                            End If
                        End If
                    End If

                    If Not IsTransferCheckInNow And CBool2(data("IsTransfer")) Then
                        p("CheckedOutReturnedOn") = "getdate()"
                        p("ActualCheckedOutReturnedOn") = "getdate()"
                    Else
                        p("CheckedOutReturnedOn") = ReadyString(data("DateTo"))
                    End If

                    p("IsDone") = ReadyString("False")
                    p("IsDoneCleaning") = ReadyString("False")
                    p("IsOutOfOrder") = ReadyString("False")

                    'pSql = "SELECT ID FROM TransRentableAssets " & vbCrLf &
                    '        " WHERE (getdate() BETWEEN CheckedInUsedOn AND CheckedOutReturnedOn " & vbCrLf &
                    '        " OR " & ReadyString(data("DateTo")) & " BETWEEN CheckedInUsedOn AND CheckedOutReturnedOn) AND IsDone = 0 " & vbCrLf &
                    '        " AND AssetID = " & ReadyString(data("AssetID")) & vbCrLf &
                    '        If(transID = "", "", " AND ID <> " & ReadyString(transID))

                    If Not IsTransferCheckInNow Then
                        pSql = "SELECT ID FROM TransRentableAssets " & vbCrLf &
                           " WHERE ((CheckedInUsedOn <=" & ReadyString(data("DateFrom")) & " AND CheckedOutReturnedOn > " & ReadyString(data("DateFrom")) & ") " & vbCrLf &
                           " OR (CheckedInUsedOn <" & ReadyString(data("DateTo")) & "AND CheckedOutReturnedOn >= " & ReadyString(data("DateTo")) & ")) AND IsDone = 0 " & vbCrLf &
                           " AND AssetID = " & ReadyString(data("AssetID")) & vbCrLf &
                           If(transID = "", "", " AND ID <> " & ReadyString(transID))
                    Else
                        pSql = "SELECT ID FROM TransRentableAssets " & vbCrLf &
                           " WHERE ((ReservedFrom <=" & ReadyString(data("DateFrom")) & " AND ReservedUntil > " & ReadyString(data("DateFrom")) & ") " & vbCrLf &
                           " OR (ReservedFrom <" & ReadyString(data("DateTo")) & "AND ReservedUntil >= " & ReadyString(data("DateTo")) & ")) AND IsDone = 0 " & vbCrLf &
                           " AND ISNULL(TransferDate, '') = '' " & vbCrLf &
                           " AND AssetID = " & ReadyString(data("TransferToAssetID")) & vbCrLf &
                           If(transID = "", "", " AND ID <> " & ReadyString(transID))
                    End If

                ElseIf data("Action") = "checkout" Then
                    p("ActualCheckedOutReturnedOn") = "getdate()"
                    p("IsDone") = ReadyString("True")
                    p("IsOutOfOrder") = ReadyString("False")
                    p("CheckedOutByUser") = ReadyString(WebSession.Instance.PeopleUserID)

                    pSql = "SELECT ID FROM TransRentableAssets WHERE IsDoneCleaning = 1 AND ID = " & ReadyString(transID)
                    If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                        p("IsDoneCleaning") = ReadyString("False")
                    End If

                ElseIf data("Action") = "outoforder" Then
                    p("AssetID") = ReadyString(data("AssetID"))
                    p("OutOfOrderFrom") = ReadyString(data("DateFrom"))
                    p("OutOfOrderTo") = ReadyString(data("DateTo"))
                    p("OutOfOrderReason") = ReadyString(data("OutOfOrderReason"))
                    p("IsDone") = ReadyString("False")
                    p("IsDoneCleaning") = ReadyString("False")
                    p("IsOutOfOrder") = ReadyString(data("IsOutOfOrder"))
                End If

                If (data("Action") = "reserve" Or data("Action") = "checkin") Then
                    p("OnlineTravelAgenyName") = ReadyString(data("OTAName"))
                    p("OTAConfirmationNumber") = ReadyString(data("OTAConfirmationNo"))
                    p("CustomRates") = IIf(data("CustomRates") = "", "NULL", ReadyString(data("CustomRates")))

                    If Common.WebRequest.ProData.Execute(pSql).Rows.Count > 0 Then
                        Return JsonDictionary.Serialize("Results", data("AssetName") & " is not available from " & data("DateFrom") & " to " & data("DateTo"))
                    End If
                End If

                If transID = "" Then
                    If data("Action") = "checkin" Then
                        p("CheckedInByUser") = ReadyString(WebSession.Instance.PeopleUserID)
                    End If
                    transID = tempTransID
                    p("TransactionNumber") = ReadyString(GetTransNumber())
                    p("CreatedDate") = "getdate()"
                    p("ID") = ReadyString(tempTransID)
                    pSql = p.GetInsert()
                Else
                    If data.ContainsKey("TransactionNumber") Then
                        If CStr2(data("TransactionNumber")) = "" Then
                            p("TransactionNumber") = ReadyString(GetTransNumber())
                        End If
                    End If

                    pSql = p.GetUpdate(" ID = " & ReadyString(transID))
                End If

                ' Common.WebRequest.ProData.BeginTransaction()
                Common.WebRequest.ProData.Execute(pSql)
                AssignTransIdToAsset(data("AssetID"), transID, data("Action"))
                'Common.WebRequest.ProData.CommitTransaction()

                If data("Action") = "outoforder" Then
                    Common.WebRequest.ProData.CommitTransaction()
                    Return JsonDictionary.Serialize("Results", "saved")
                End If

                If data("Action") <> "checkout" Then
                    If data("Action") <> "cancel" Then
                        Dim transocids As New ArrayList()
                        For Each oc As Object In data("OtherCharges")
                            Dim ocid As String = Guid.NewGuid.ToString()
                            p = New InsertUpdate("TransRentableAssetOtherCharges")
                            p("TransRentableAssetID") = ReadyString(transID)
                            p("Qty") = ReadyString(oc("RequestQty"))
                            p("OtherChargesID") = ReadyString(oc("ID"))

                            If oc.ContainsKey("TransOCId") Then
                                transocids.Add(oc("TransOCId"))
                                pSql = p.GetUpdate(" ID = " & ReadyString(oc("TransOCId")))
                            Else
                                transocids.Add(ocid)
                                p("ID") = ReadyString(ocid)
                                pSql = p.GetInsert()
                            End If

                            Common.WebRequest.ProData.Execute(pSql)
                        Next oc

                        If transocids.Count > 0 Then
                            pSql = "DELETE FROM TransRentableAssetOtherCharges " & vbCrLf &
                                " WHERE ID NOT IN ( '" & String.Join("', '", transocids.ToArray()) & "') " & vbCrLf &
                                " AND TransRentableAssetID = " & ReadyString(transID)
                            pSql = Common.WebRequest.ProData.ExecuteSQL(pSql) ' output is for debugging only
                        End If
                    Else
                        pSql = "DELETE FROM TransRentableAssetOtherCharges WHERE TransRentableAssetID = " & ReadyString(transID)
                        pSql = Common.WebRequest.ProData.ExecuteSQL(pSql) ' output is for debugging only
                    End If
                End If

                If data("Action") = "checkout" Then
                    pSql = "SELECT ID, AssetID FROM TransRentableAssets WHERE TransferFromID = " & ReadyString(transID)
                    Dim relatedcheckins As Object = Common.WebRequest.ProData.Execute(pSql).Rows

                    For Each ck As Object In relatedcheckins
                        p = New InsertUpdate("TransRentableAssets")
                        p("LastUpdated") = "getdate()"
                        p("ActualCheckedOutReturnedOn") = "getdate()"
                        p("IsDone") = ReadyString("True")

                        Common.WebRequest.ProData.Execute(p.GetUpdate("ID = " & ReadyString(ck("ID"))))

                        AssignTransIdToAsset(ck("AssetID").ToString(), ck("ID").ToString(), data("Action"))

                        p = New InsertUpdate("TransRentableAssetCustomerRequest")

                        p("IsDone") = ReadyString("True")

                        Common.WebRequest.ProData.Execute(p.GetUpdate("TransRentableAssetID = " & ReadyString(ck("ID"))))
                    Next ck

                    p = New InsertUpdate("TransRentableAssetCustomerRequest")

                    p("IsDone") = ReadyString("True")

                    Common.WebRequest.ProData.Execute(p.GetUpdate("TransRentableAssetID = " & ReadyString(transID)))
                End If

                MoveClientToNewTable(transID)

                If (data("Action") = "reserve" Or data("Action") = "checkin") Then
                    Dim msg As String = SaveAssetCustomer(data, transID)

                    If msg = "saved" Then
                        WebRequest.Instance.Business.LibraryRepeatingTransactions.HandleUponCheckInInvoices(transID, WebSession.Instance.PeopleUserID, WebRequest.Instance.OrgID)
                        WebRequest.Instance.Business.LibraryRepeatingTransactions.Handle1Org(WebRequest.Instance.OrgID)

                        Common.WebRequest.ProData.CommitTransaction()
                    Else
                        Common.WebRequest.ProData.RollbackTransaction()
                        Return JsonDictionary.Serialize("Results", msg)
                    End If
                ElseIf (data("Action") = "checkout") Then
                    WebRequest.Instance.Business.LibraryRepeatingTransactions.Handle1Org(WebRequest.Instance.OrgID)
                    Common.WebRequest.ProData.CommitTransaction()
                Else
                    Common.WebRequest.ProData.CommitTransaction()
                End If
                
                Return JsonDictionary.Serialize("Results", transID)
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try

        End Function

        Function MoveClientToNewTable(ByVal transID As String)
            'updated by ARNIE Aug 7, 2019
            'pwede na multiple guests per check-in, maong gina move ang currently checked-in with 1 guest sa new table
            pSql = "SELECT ClientID FROM TransRentableAssets WHERE ID = " + ReadyString(transID)
            Dim clientID = Common.WebRequest.ProData.Execute(pSql).Rows(0)("ClientID").ToString()

            If clientID <> "" Then
                pSql = "SELECT ID FROM TransRentableAssetCustomer WHERE TransRentableAssetID = " & ReadyString(transID) & vbCrLf &
                            " AND ClientID = " & ReadyString(clientID)

                If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                    Dim p As New InsertUpdate("TransRentableAssetCustomer")

                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("TransRentableAssetID") = ReadyString(transID)
                    p("ClientID") = ReadyString(clientID)
                    p("SortOrderNo") = 1

                    Common.WebRequest.ProData.Execute(p.GetInsert())

                    'p = New InsertUpdate("TransRentableAssets")

                    'p("ClientID") = "NULL"

                    'Common.WebRequest.ProData.Execute(p.GetUpdate("ID = " & ReadyString(transID)))
                End If
            End If

        End Function

        Function GetTransNumber()
            Dim transno As String = Format(Now, "ddMMyyyy") & "-" & GenerateString(3, "0123456789")
            Dim pSql As String

            pSql = "SELECT ID FROM TransRentableAssets WHERE TransactionNumber = " & ReadyString(transno)

            If Common.WebRequest.ProData.Execute(pSql).Rows.Count > 0 Then
                Return GetTransNumber()
            End If

            Return transno
        End Function

        Function GetHistoryOfSelectedRequest()
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                pSql = "SELECT req.* " & vbCrLf &
                        " , ppol.FirstName + ' ' + ppol.Surname as ModifiedBy" & vbCrLf &
                        " , convert(varchar, req.LastUpdated, 120) as ModifiedDate " & vbCrLf &
                        " FROM TransRentableAssetCustomerRequest as req" & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = req.UserID " & vbCrLf &
                        " WHERE req.ID = " & ReadyString(Common.WebRequest.Data("ID")) &
                        " OR req.ParentHistoryID = " & ReadyString(Common.WebRequest.Data("ID")) & vbCrLf &
                        " ORDER BY req.LastUpdated DESC"

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetCustomerRequestHistory()
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                pSql = "SELECT req.* " & vbCrLf &
                        " , ppol.FirstName + ' ' + ppol.Surname as ModifiedBy" & vbCrLf &
                        " , convert(varchar, req.LastUpdated, 120) as ModifiedDate " & vbCrLf &
                        " FROM TransRentableAssetCustomerRequest as req" & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = req.UserID " & vbCrLf &
                        " WHERE req.ClientID = " & ReadyString(Common.WebRequest.Data("ClientID")) &
                        " AND req.IsActive = 1 " & vbCrLf &
                        " ORDER BY req.LastUpdated DESC"

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function AssignTransIdToAsset(ByVal AssetID As String, ByVal TransID As String, ByVal Action As String) As String
            Dim p As New InsertUpdate("LibraryRentableAssets")

            If Action = "checkout" Or Action = "cancel" Then
                p("TransID") = "NULL"
                p("Status") = ReadyString("Available")
            Else
                If Action = "reserve" Then
                    p("Status") = ReadyString("Reserved")
                ElseIf Action = "unavailable" Then
                    p("Status") = ReadyString("Unvailable")
                ElseIf Action = "outoforder" Then
                    p("Status") = ReadyString("Out of Order")
                End If
                p("TransID") = ReadyString(TransID)
            End If

            Common.WebRequest.ProData.Execute(p.GetUpdate("ID = " & ReadyString(AssetID)))
        End Function

        Function GetListOfAssets() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pDT As DataTable
            Dim data As Object = Common.WebRequest.AllData("data")
            Dim isFilter As Boolean = False
            Dim operators As String = ""
            Dim preSearch As String = ""
            Dim where As String = ""

            Try
                pSql = "SELECT DefaultValue FROM SystemDefaults WHERE Description = 'CheckOutTimeDefault'"
                pDT = Common.WebRequest.ProData.Execute(pSql)

                pSql = "SELECT * FROM ( " & vbCrLf &
                        " SELECT ass.ID " & vbCrLf &
                        " , pim.ID as ProductID " & vbCrLf &
                        " , pim.ProductNo as AssetNo " & vbCrLf &
                        " , pim.ProductName as AssetName " & vbCrLf &
                        " , cat.C0020 as AssetType " & vbCrLf &
                        " , CONVERT(varchar, pimmsr.Quantity) as Capacity " & vbCrLf &
                        " , lbl.Label as CapacityUnit " & vbCrLf &
                        " , convert(varchar, ass.LastUpdated, 120) as LastUpdated " & vbCrLf &
                        " , convert(varchar, tranass.ReservedFrom, 120) as ReservedFrom " & vbCrLf &
                        " , convert(varchar, tranass.ReservedUntil, 120) as ReservedUntil " & vbCrLf &
                        " , convert(varchar, tranass.CheckedOutReturnedOn, 120) as CheckedOutReturnedOn " & vbCrLf &
                        " , convert(varchar, tranass.ActualCheckedOutReturnedOn, 120) as ActualCheckedOutReturnedOn " & vbCrLf &
                        " , convert(varchar, tranass.CheckedInUsedOn, 120) as CheckedInUsedOn " & vbCrLf &
                        " , tranass.IsCancelled " & vbCrLf &
                        " , tranass.ID as TransID " & vbCrLf &
                        " , tranass.ClientID " & vbCrLf &
                        " , tranass.NoOfAdults " & vbCrLf &
                        " , tranass.NoOfChild " & vbCrLf &
                        " , tranass.PriceLevelID " & vbCrLf &
                        " , ISNULL(ass.Status, 'Available') as Status " & vbCrLf &
                        " , ISNULL(tranass.IsDone, 'False') as IsDone " & vbCrLf &
                        " , ISNULL(pubppol.FirstName, pricusppol.FirstName) + ' ' + ISNULL(pubppol.MiddleName, pricusppol.MiddleName) + ' ' + ISNULL(pubppol.Surname, pricusppol.Surname) as CustomerName " & vbCrLf &
                        " , ISNULL(pubppol.FirstName, pricusppol.FirstName) as FirstName " & vbCrLf &
                        " , ISNULL(pubppol.MiddleName, pricusppol.MiddleName) as MiddleName " & vbCrLf &
                        " , ISNULL(pubppol.Surname, pricusppol.Surname) as Surname " & vbCrLf &
                        " , convert(varchar, pubppol.DateOfBirth, 120) as DateOfBirth " & vbCrLf &
                        " , ppoladd.CityID " & vbCrLf &
                        " , ppoladd.CountryID " & vbCrLf &
                        " , ppoladd.HouseBuildingNo " & vbCrLf &
                        " , ppoladd.Street " & vbCrLf &
                        " , ppoladd.ID as AddressID " & vbCrLf &
                        " , ppolemail.ID as EmailID " & vbCrLf &
                        " , ppolemail.EmailAddress " & vbCrLf &
                        " , ppolmobile.ID as MobileID " & vbCrLf &
                        " , ppolmobile.ContactNo " & vbCrLf &
                        " , ppolmobile.CountryID as CountryCodeID " & vbCrLf &
                        " , ppolmobile.TelCoPrefixID " & vbCrLf &
                        " , cusorg.ID as OrgID " & vbCrLf &
                        " , cusorg.CompanyName " & vbCrLf &
                        " FROM LibraryRentableAssets as ass " & vbCrLf &
                        " INNER JOIN MobizLibraryPimProduct as pim ON pim.ID = ass.ProductID " & vbCrLf &
                        " INNER JOIN MobizLibraryPIMProductMeasurement as pimmsr ON pimmsr.ProductID = ass.ProductID " & vbCrLf &
                        " INNER JOIN MobizLibraryPIMMeasure as msr ON msr.ID = pimmsr.UnitMeasureID " & vbCrLf &
                        " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (msr.MeasureNameID = lbl.ID) " & vbCrLf &
                        " INNER JOIN MobizLibraryPIMProductVariableFeatures001 as cat ON cat.ProductID = ass.ProductID " & vbCrLf &
                        " LEFT JOIN TransRentableAssets as tranass ON tranass.ID = ass.TransID " & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganization as cusorg ON cusorg.ID = tranass.OrgID " & vbCrLf &
                        " LEFT JOIN MobizLibraryPeople as pubppol ON pubppol.ID = tranass.ClientID " & vbCrLf &
                        " LEFT JOIN LibraryPeople as pricusppol ON pricusppol.ID = tranass.ClientID " & vbCrLf &
                        " LEFT JOIN LibraryPeopleAddress as ppoladd ON ppoladd.PeopleID = tranass.ClientID " & vbCrLf &
                        " LEFT JOIN LibraryPeopleContactEmail as ppolemail ON ppolemail.PeopleID = tranass.ClientID " & vbCrLf &
                        " LEFT JOIN LibraryPeopleContactMobile as ppolmobile ON ppolmobile.ID = (SELECT TOP 1 ID FROM LibraryPeopleContactMobile WHERE PeopleID = tranass.ClientID) " & vbCrLf &
                    ") as q1 "

                If data("Status") <> "All" Then
                    isFilter = True
                    where += "(Status = " & ReadyString(data("Status"))
                    If data("Status") = "Available" Then
                        where += " OR Status = ''"
                    End If
                    where += ") " & vbCrLf
                    operators = " AND "
                End If

                If data("Search") <> "" Then
                    isFilter = True
                    where += operators & " (AssetName + ' ' + Capacity + ' ' + AssetType + ' ' + CustomerName LIKE " & ReadyStringWild(data("Search")) & ")" & vbCrLf
                    operators = " AND "
                End If

                If data("Type") <> "All" Then
                    where += operators & " (AssetType = " & ReadyString(data("Type")) & ")"
                    operators = " AND "
                End If

                If isFilter Then
                    pSql += " WHERE " + where
                End If

                pSql += " ORDER BY AssetName ASC "

                pDT = Common.WebRequest.ProData.Execute(pSql)

                Return JsonDictionary.Serialize("Results", pDT)
            Catch ex As Exception
                AppendToLog(ex.Message)
            End Try

        End Function

        Function GetAssetSetting() As Object
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut As New JsonDictionary
            Dim pDT As DataTable

            pSql = "SELECT * FROM SystemDefaults WHERE Description = 'AssMgmtLabelSetting'"
            pDT = Common.WebRequest.ProData.Execute(pSql)

            pOut.Add("LabelSetting", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDT)))

            pSql = "SELECT * FROM SystemDefaults WHERE Description = 'AssMgmtCustomLabelForAssets'"
            pDT = Common.WebRequest.ProData.Execute(pSql)

            pOut.Add("CustomLableForAssets", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDT)))

            pSql = "SELECT * FROM SystemDefaults WHERE Description = 'AssMgmtCustomLabelForCustomers'"
            pDT = Common.WebRequest.ProData.Execute(pSql)

            pOut.Add("CustomLableForCustomers", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDT)))

            pSql = "SELECT * FROM SystemDefaults WHERE Description = 'CheckOutTimeDefault'"
            pDT = Common.WebRequest.ProData.Execute(pSql)

            pOut.Add("DefaultCheckOutTime", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDT)))

            pSql = "SELECT * FROM SystemDefaults WHERE Description = 'IsAllowOutdatedReservation'"
            pDT = Common.WebRequest.ProData.Execute(pSql)

            pOut.Add("OutdatedReservationSettings", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDT)))

            pSql = "SELECT * FROM SystemDefaults WHERE Description = 'IsAllowFutureDatedCheckIns'"
            pDT = Common.WebRequest.ProData.Execute(pSql)

            pOut.Add("IsAllowFutureDatedCheckIns", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDT)))

            pOut.Add("InvoiceMethod", jss.DeserializeObject(GetInvoicingMethod())("d")("Results"))

            pSql = "SELECT * FROM SystemDefaults WHERE Description = 'MaxNumberOfGuestCards'"
            pDT = Common.WebRequest.ProData.Execute(pSql)

            pOut.Add("MaxNumberOfGuestCards", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDT)))

            pSql = "SELECT * FROM SystemDefaults WHERE Description = 'AllowLostCardCreation'"
            pDT = Common.WebRequest.ProData.Execute(pSql)

            pOut.Add("AllowLostCardCreation", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDT)))

            pSql = "SELECT * FROM SystemDefaults WHERE Description = 'MaximumNumberOfLostCard'"
            pDT = Common.WebRequest.ProData.Execute(pSql)

            pOut.Add("MaximumNumberOfLostCard", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDT)))

            pSql = "SELECT * FROM SystemDefaults WHERE Description = 'DefaultChargeOfLostCard'"
            pDT = Common.WebRequest.ProData.Execute(pSql)

            pOut.Add("DefaultChargeOfLostCard", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDT)))

            pSql = "SELECT * FROM SystemDefaults WHERE Description = 'CreateKeyCardAfterCheckIn'"
            pDT = Common.WebRequest.ProData.Execute(pSql)

            pOut.Add("CreateKeyCardAfterCheckIn", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDT)))

            pSql = "SELECT * FROM SystemDefaults WHERE Description = 'DisplayProductNoInAssets'"
            pDT = Common.WebRequest.ProData.Execute(pSql)

            pOut.Add("DisplayProductNoInAssets", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDT)))

            pSql = "SELECT * FROM SystemDefaults WHERE Description = 'DoorLockSystem'"
            pDT = Common.WebRequest.ProData.Execute(pSql)

            pOut.Add("DoorLockSystem", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pDT)))

            pOut.Add("CheckInTime", Format(WebRequest.Instance.Business.SystemDefaults.CheckInTimeDefault, cGlobal.FormatTime))
            pOut.Add("EarlyCheckInTime", Format(WebRequest.Instance.Business.SystemDefaults.EarlyCheckInTime, cGlobal.FormatTime))
            Dim pEarlyCheckInProductID = WebRequest.Instance.Business.SystemDefaults.EarlyCheckInProductID
            pOut.Add("EarlyCheckInProductID", pEarlyCheckInProductID)
            If IsGuid(pEarlyCheckInProductID) Then
                pDT = WebRequest.Instance.Business.LibraryStock.ByIds(ReadyString(pEarlyCheckInProductID))
                If pDT.Rows.Count > 0 Then
                    pOut.Add("EarlyCheckInProductName", pDT.Rows(0)("ProductName"))
                End If
            End If
            pOut.Add("LateCheckOutTime", Format(WebRequest.Instance.Business.SystemDefaults.LateCheckOutTime, cGlobal.FormatTime))
            Dim pLateCheckOutProductID = WebRequest.Instance.Business.SystemDefaults.LateCheckOutProductID
            pOut.Add("LateCheckOutProductID", pLateCheckOutProductID)
            If IsGuid(pLateCheckOutProductID) Then
                pDT = WebRequest.Instance.Business.LibraryStock.ByIds(ReadyString(pLateCheckOutProductID))
                If pDT.Rows.Count > 0 Then
                    pOut.Add("LateCheckOutProductName", pDT.Rows(0)("ProductName"))
                End If
            End If
            pOut.Add("DisplayAssetPrice", WebRequest.Instance.Business.SystemDefaults.DisplayAssetPrice)
            pOut.Add("AssetMgmtDownpaymentRequired", WebRequest.Instance.Business.SystemDefaults.AssetMgmtDownpaymentRequired)
            Return pOut.Serialize()
        End Function

        Function GetPreviousCheckedIn()
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim dates As String = Common.WebRequest.Data("Date")
            Dim searchname As String = Common.WebRequest.Data("Search")

            Try
                pSql = " SELECT trans.* " & vbCrLf &
                    " , CASE WHEN (SELECT COUNT(ID) FROM TransRentableAssetCustomer WHERE TransRentableAssetID = trans.ID) = 0 " & vbCrLf &
                    "   THEN (SELECT ISNULL(pubppol.FirstName, pricusppol.PersonFirstName) + ' ' + ISNULL(pubppol.MiddleName, pricusppol.PersonMiddleName) + ' ' + ISNULL(pubppol.Surname, pricusppol.PersonSurname) " & vbCrLf &
                    "           FROM TransRentableAssets " & vbCrLf &
                    "           LEFT JOIN MobizLibraryPeople as pubppol ON pubppol.ID = TransRentableAssets.ClientID " & vbCrLf &
                    "           LEFT JOIN MobizLibraryOrganization as pricusppol ON pricusppol.ID = TransRentableAssets.ClientID " & vbCrLf &
                    "           WHERE TransRentableAssets.ID = trans.ID) " & vbCrLf &
                    "   ELSE (SELECT DISTINCT STUFF((SELECT DISTINCT ', ' + " & vbCrLf &
                    "           ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) + ' ' + ISNULL(pubppol.PersonSurname, pricusppol.Surname) " & vbCrLf &
                    "           FROM TransRentableAssetCustomer as transcus " & vbCrLf &
                    "           LEFT JOIN MobizLibraryOrganization as pubppol ON pubppol.ID = transcus.ClientID " & vbCrLf &
                    "           LEFT JOIN LibraryPeople as pricusppol ON pricusppol.ID = transcus.ClientID " & vbCrLf &
                    "           WHERE transcus.TransRentableAssetID = trans.ID " & vbCrLf &
                    "           FOR XML PATH('')), 1, 1,'')) " & vbCrLf &
                    "   END as CustomerName " & vbCrLf &
                    " , pim.ProductName as AssetName, cat.C0020 as AssetType " & vbCrLf &
                    " FROM TransRentableAssets as trans " & vbCrLf &
                    " INNER JOIN LibraryRentableAssets as ass ON ass.ID = trans.AssetID " & vbCrLf &
                    " INNER JOIN MobizLibraryPimProduct as pim ON pim.ID = ass.ProductID " & vbCrLf &
                    " INNER JOIN MobizLibraryPIMProductVariableFeatures001 as cat ON cat.ProductID = ass.ProductID " & vbCrLf &
                    " WHERE IsDone = 1 "

                If searchname = "" Then
                    pSql += " AND CAST(" & ReadyString(dates) & " AS DATE) = CAST(CheckedInUsedOn AS DATE) " & vbCrLf &
                            " ORDER BY CheckedInUsedOn ASC "
                Else
                    pSql = "SELECT * FROM (" & pSql & ") as T WHERE IsDone = 1 AND ISNULL(CheckedInUsedOn, '') <> '' AND CustomerName Like " & ReadyStringWild(searchname) & vbCrLf &
                            " ORDER BY CheckedInUsedOn ASC "
                End If

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function LibraryAssets() As Object
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                pSql = " SELECT DISTINCT CASE WHEN prdfea1.C0020 = '' THEN 'Other(s)' ELSE prdfea1.C0020 END as Category " & vbCrLf &
                        " FROM LibraryRentableAssets as ass " & vbCrLf &
                        " INNER JOIN MobizLibraryPimProduct as pim ON pim.ID = ass.ProductID " & vbCrLf &
                        " LEFT JOIN MobizLibraryPIMProductVariableFeatures001 prdfea1 ON (prdfea1.ProductID = pim.ID) " & vbCrLf &
                        " INNER JOIN LibraryOrganizationPIM as orgpim ON orgpim.ProductID = ass.ProductID " & vbCrLf &
                        " WHERE pim.PublishedByID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                        " AND pim.IsAsset = 1 " & vbCrLf &
                        " AND orgpim.IsDeActivated = 0 " & vbCrLf

                Dim arrAssetCategory As New List(Of Dictionary(Of String, Object))

                For Each c As Object In Common.WebRequest.ProData.Execute(pSql).Rows
                    Dim pAssets As New Dictionary(Of String, Object)

                    pAssets.Add("Category", c("Category"))

                    pSql = "SELECT * FROM ( " & vbCrLf &
                        " SELECT ass.ID " & vbCrLf &
                        " , pim.ID as ProductID " & vbCrLf &
                        " , pim.ProductNo as AssetNo " & vbCrLf &
                        " , pim.ProductName as AssetName " & vbCrLf &
                        " , cat.C0020 as AssetType " & vbCrLf &
                        " , CONVERT(varchar, pimmsr.Quantity) as Capacity " & vbCrLf &
                        " , lbl.Label as CapacityUnit " & vbCrLf &
                        " , ass.LastUpdated " & vbCrLf &
                        " , prdfea1.C0020 as Category " & vbCrLf &
                        " FROM LibraryRentableAssets as ass " & vbCrLf &
                        " INNER JOIN MobizLibraryPimProduct as pim ON pim.ID = ass.ProductID " & vbCrLf &
                        " LEFT JOIN MobizLibraryPIMProductVariableFeatures001 prdfea1 ON (prdfea1.ProductID = pim.ID) " & vbCrLf &
                        " INNER JOIN LibraryOrganizationPIM as orgpim ON orgpim.ProductID = ass.ProductID " & vbCrLf &
                        " INNER JOIN MobizLibraryPIMProductMeasurement as pimmsr ON pimmsr.ProductID = ass.ProductID " & vbCrLf &
                        " INNER JOIN MobizLibraryPIMMeasure as msr ON msr.ID = pimmsr.UnitMeasureID " & vbCrLf &
                        " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (msr.MeasureNameID = lbl.ID) " & vbCrLf &
                        " INNER JOIN MobizLibraryPIMProductVariableFeatures001 as cat ON cat.ProductID = ass.ProductID " & vbCrLf &
                        " WHERE pim.PublishedByID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                        " AND pim.IsAsset = 1 " & vbCrLf &
                        " AND orgpim.IsDeActivated = 0 " & vbCrLf &
                        " AND prdfea1.C0020 = " & ReadyString(IIf(c("Category") = "Other(s)", "", c("Category"))) & vbCrLf &
                    ") as q1 ORDER BY AssetName ASC "

                    Dim pdtAsset As DataTable = Common.WebRequest.ProData.Execute(pSql)
                    Dim arrAsset As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(pdtAsset))
                    Dim pOut As New JsonDictionary

                    For i As Integer = 0 To arrAsset.Length - 1
                        Dim selectedMonth As String = ReadyString(CInt2(Common.WebRequest.Data("Year")) & "" & Format(CInt2(Common.WebRequest.Data("Month")) + 1, "0#"))
                        Dim row As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(New DataTable)) 'Common.WebRequest.ProData.Execute(pSql)

                        arrAsset(i).Add("Trans", row)
                    Next

                    pAssets.Add("Assets", arrAsset)
                    arrAssetCategory.Add(pAssets)
                Next c

                Return Json(New With {Key .Results = arrAssetCategory
                                 })

                'pOut.Add("Results", arrAsset)

                'Return pOut.Serialize()
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return Json(New With {Key .Results = ex.Message})
            End Try

        End Function

        Function GetAllTransactions()
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim selectedMonth As String = ReadyString(CInt2(Common.WebRequest.Data("Year")) & "" & Format(CInt2(Common.WebRequest.Data("Month")) + 1, "0#"))
                Dim previousMonth As String = ReadyString(Common.WebRequest.Data("PreviousMonth"))
                Dim pSql = "SELECT " & vbCrLf &
                        " ISNULL(trans.IsDone, 'False') as IsDone " & vbCrLf &
                        " , ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) + ' ' + ISNULL(pubppol.PersonMiddleName, pricusppol.MiddleName) + ' ' + ISNULL(pubppol.PersonSurname, pricusppol.Surname) as CustomerName " & vbCrLf &
                        " --, cusorg.ID as OrgID " & vbCrLf &
                        " --, cusorg.CompanyName " & vbCrLf &
                        " , trans.ID " & vbCrLf &
                        " --, trans.TransactionNumber " & vbCrLf &
                        " , trans.AssetID " & vbCrLf &
                        " --, trans.OrgID " & vbCrLf &
                        " --, ISNULL(convert(varchar(max), trans.ClientID), (SELECT TOP 1 ClientID FROM TransRentableAssetCustomer WHERE TransRentableAssetID = trans.ID))  as ClientID " & vbCrLf &
                        " , trans.TransRefID " & vbCrLf &
                        " , trans.IsCancelled " & vbCrLf &
                        " , trans.ID as TransID " & vbCrLf &
                        " --, trans.NoOfAdults " & vbCrLf &
                        " --, trans.NoOfChild " & vbCrLf &
                        " --, trans.OnlineTravelAgenyName " & vbCrLf &
                        " --, trans.OTAConfirmationNumber " & vbCrLf &
                        " --, trans.CustomRates " & vbCrLf &
                        " --, trans.PriceLevelID " & vbCrLf &
                        " --, trans.OutOfOrderReason " & vbCrLf &
                        " , ISNULL(trans.IsDoneCleaning, 'False') as IsDoneCleaning " & vbCrLf &
                        " , ISNULL(trans.IsOutOfOrder, 'False') as IsOutOfOrder " & vbCrLf &
                        " , convert(varchar, trans.CreatedDate, 120) as CreatedDate " & vbCrLf &
                        " , convert(varchar, trans.LastUpdated, 120) as LastUpdated " & vbCrLf &
                        " , convert(varchar, trans.DateCancelled, 120) as DateCancelled " & vbCrLf &
                        " , convert(varchar, trans.TransferDate, 120) as TransferDate " & vbCrLf &
                        " , trans.TransferFromID " & vbCrLf &
                        " , convert(varchar, trans.ReservedFrom, 120) as ReservedFrom " & vbCrLf &
                        " , convert(varchar, trans.ReservedUntil, 120) as ReservedUntil " & vbCrLf &
                        " , convert(varchar, trans.CheckedOutReturnedOn, 120) as CheckedOutReturnedOn " & vbCrLf &
                        " , convert(varchar, trans.ActualCheckedOutReturnedOn, 120) as ActualCheckedOutReturnedOn " & vbCrLf &
                        " , convert(varchar, trans.OutOfOrderFrom, 120) as OutOfOrderFrom " & vbCrLf &
                        " , convert(varchar, trans.OutOfOrderTo, 120) as OutOfOrderTo " & vbCrLf &
                        " , convert(varchar, trans.CheckedInUsedOn, 120) as CheckedInUsedOn " & vbCrLf &
                        " , convert(varchar, ISNULL(trans.ActualCheckedInUsedOn, trans.CheckedInUsedOn), 120) as ActualCheckedInUsedOn " & vbCrLf &
                        " --, (SELECT prdprc.Price FROM TransRentableAssets as transren " & vbCrLf &
                        " --      INNER JOIN LibraryRentableAssets as ren ON transren.AssetID = ren.ID " & vbCrLf &
                        " --      INNER JOIN MobizLibraryPIMProductPriceLevel as prdprc ON ren.ProductID = prdprc.ProductID " & vbCrLf &
                        " --      INNER JOIN MobizLibraryPIMPriceLevel prc ON prdprc.PriceLevelID = prc.ID " & vbCrLf &
                        " --      WHERE prdprc.PriceLevelID = trans.PriceLevelID AND prdprc.ProductID = ren.ProductID AND transren.ID = trans.ID " & vbCrLf &
                        " --      AND prdprc.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & ") as Rate" & vbCrLf &
                        " --, cusorg.OfficeAddressBuildingNo + ' ' + cusorg.OfficeAddressStreet + ', ' + cusorgcity.Name + ', ' + cusorgcountry.Name as CompanyAddress " & vbCrLf &
                        " FROM TransRentableAssets as trans " & vbCrLf &
                        " --LEFT JOIN MobizLibraryOrganization as cusorg ON cusorg.ID = trans.OrgID " & vbCrLf &
                        " LEFT JOIN MobizLibraryOrganization as pubppol ON pubppol.ID = CASE WHEN ISNULL(convert(varchar(max), trans.ClientID), '') = '' THEN (SELECT TOP 1 ClientID FROM TransRentableAssetCustomer WHERE TransRentableAssetID = trans.ID) ELSE trans.ClientID END " & vbCrLf &
                        " LEFT JOIN LibraryPeople as pricusppol ON pricusppol.ID = CASE WHEN ISNULL(convert(varchar(max), trans.ClientID), '') = '' THEN (SELECT TOP 1 ClientID FROM TransRentableAssetCustomer WHERE TransRentableAssetID = trans.ID) ELSE trans.ClientID END " & vbCrLf &
                        " --LEFT JOIN MobizLibraryCities as cusorgcity ON cusorgcity.ID = cusorg.OfficeAddressCityID " & vbCrLf &
                        " --LEFT JOIN MobizLibraryCountries as cusorgcountry ON cusorgcountry.ID = cusorg.OfficeAddressCountryID " & vbCrLf &
                        " WHERE ((" & selectedMonth & " BETWEEN YEAR(ReservedFrom) * 100 + MONTH(ReservedFrom) AND YEAR(ReservedUntil) * 100 + MONTH(ReservedUntil)" & vbCrLf &
                        " OR " & selectedMonth & " BETWEEN YEAR(CheckedInUsedOn) * 100 + MONTH(CheckedInUsedOn) AND YEAR(CheckedOutReturnedOn) * 100 + MONTH(CheckedOutReturnedOn)" & vbCrLf &
                        " OR (" & selectedMonth & " BETWEEN YEAR(OutOfOrderFrom) * 100 + MONTH(OutOfOrderFrom) AND YEAR(OutOfOrderTo) * 100 + MONTH(OutOfOrderTo) AND ISNULL(trans.IsOutOfOrder, 0) = 1) " & vbCrLf &
                        IIf(CBool2(Common.WebRequest.Data("isShowPastCheckin")), "OR (" & selectedMonth & " BETWEEN YEAR(CheckedOutReturnedOn) * 100 + MONTH(CheckedOutReturnedOn) AND YEAR(ActualCheckedOutReturnedOn) * 100 + MONTH(ActualCheckedOutReturnedOn) AND ISNULL(trans.IsDone, 0) = 1)", "") & vbCrLf &
                        " ) AND ((ISNULL(trans.IsCancelled, 0) = 0 AND ISNULL(trans.IsDone, 0) = 0 AND ISNULL(trans.IsDoneCleaning, 0) = 0 AND ISNULL(trans.TransferDate, '') = '') OR (ISNULL(trans.IsCancelled, 0) = 0 AND ISNULL(trans.TransferDate, '') = '' AND ISNULL(trans.IsDone, 0) = 1 AND (ISNULL(trans.IsDoneCleaning, 0) = 0 OR ISNULL(trans.ActualCheckedOutReturnedOn, '') = '')) OR (ISNULL(trans.IsCancelled, 0) = 0 AND ISNULL(trans.IsDone, 0) = 0 AND ISNULL(trans.IsDoneCleaning, 0) = 1)" & vbCrLf &
                        IIf(CBool2(Common.WebRequest.Data("isShowPastCheckin")), "  OR (ISNULL(trans.IsDone, 0) = 1 AND " & selectedMonth & " BETWEEN YEAR(CheckedInUsedOn) * 100 + MONTH(CheckedInUsedOn) AND YEAR(ActualCheckedOutReturnedOn) * 100 + MONTH(ActualCheckedOutReturnedOn))", "") & vbCrLf &
                        "))"

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function GetAssetDetails()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String
            Dim transid As String = Common.WebRequest.Data("TransID")
            Dim IsOutOfOrder As String = Common.WebRequest.Data("IsOutOfOrder")

            pSql = "SELECT " & vbCrLf &
                    " ISNULL(trans.IsDone, 'False') as IsDone " & vbCrLf &
                    " , ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) + ' ' + ISNULL(pubppol.PersonMiddleName, pricusppol.MiddleName) + ' ' + ISNULL(pubppol.PersonSurname, pricusppol.Surname) as CustomerName " & vbCrLf &
                    " , cusorg.ID as OrgID " & vbCrLf &
                    " , cusorg.CompanyName " & vbCrLf &
                    " , trans.ID " & vbCrLf &
                    " , trans.TransactionNumber " & vbCrLf &
                    " , trans.AssetID " & vbCrLf &
                    " , trans.OrgID " & vbCrLf &
                    " , ISNULL(convert(varchar(max), trans.ClientID), (SELECT TOP 1 ClientID FROM TransRentableAssetCustomer WHERE TransRentableAssetID = trans.ID))  as ClientID " & vbCrLf &
                    " , trans.TransRefID " & vbCrLf &
                    " , trans.IsCancelled " & vbCrLf &
                    " , trans.ID as TransID " & vbCrLf &
                    " , trans.NoOfAdults " & vbCrLf &
                    " , trans.NoOfChild " & vbCrLf &
                    " , trans.OnlineTravelAgenyName " & vbCrLf &
                    " , trans.OTAConfirmationNumber " & vbCrLf &
                    " , trans.CustomRates " & vbCrLf &
                    " , trans.PriceLevelID " & vbCrLf &
                    " , trans.OutOfOrderReason " & vbCrLf &
                    " , trans.DateReserved " & vbCrLf &
                    " , ISNULL(trans.IsDoneCleaning, 'False') as IsDoneCleaning " & vbCrLf &
                    " , ISNULL(trans.IsOutOfOrder, 'False') as IsOutOfOrder " & vbCrLf &
                    " , convert(varchar, trans.CreatedDate, 120) as CreatedDate " & vbCrLf &
                    " , convert(varchar, trans.LastUpdated, 120) as LastUpdated " & vbCrLf &
                    " , convert(varchar, trans.DateCancelled, 120) as DateCancelled " & vbCrLf &
                    " , convert(varchar, trans.TransferDate, 120) as TransferDate " & vbCrLf &
                    " , trans.TransferFromID " & vbCrLf &
                    " , convert(varchar, trans.ReservedFrom, 120) as ReservedFrom " & vbCrLf &
                    " , convert(varchar, trans.ReservedUntil, 120) as ReservedUntil " & vbCrLf &
                    " , convert(varchar, trans.CheckedOutReturnedOn, 120) as CheckedOutReturnedOn " & vbCrLf &
                    " , convert(varchar, trans.ActualCheckedOutReturnedOn, 120) as ActualCheckedOutReturnedOn " & vbCrLf &
                    " , convert(varchar, trans.OutOfOrderFrom, 120) as OutOfOrderFrom " & vbCrLf &
                    " , convert(varchar, trans.OutOfOrderTo, 120) as OutOfOrderTo " & vbCrLf &
                    " , convert(varchar, trans.CheckedInUsedOn, 120) as CheckedInUsedOn " & vbCrLf &
                    " , convert(varchar, ISNULL(trans.ActualCheckedInUsedOn, trans.CheckedInUsedOn), 120) as ActualCheckedInUsedOn " & vbCrLf &
                    " , (SELECT prdprc.Price FROM TransRentableAssets as transren " & vbCrLf &
                    "       INNER JOIN LibraryRentableAssets as ren ON transren.AssetID = ren.ID " & vbCrLf &
                    "       INNER JOIN MobizLibraryPIMProductPriceLevel as prdprc ON ren.ProductID = prdprc.ProductID " & vbCrLf &
                    "       INNER JOIN MobizLibraryPIMPriceLevel prc ON prdprc.PriceLevelID = prc.ID " & vbCrLf &
                    "       WHERE prdprc.PriceLevelID = trans.PriceLevelID AND prdprc.ProductID = ren.ProductID AND transren.ID = trans.ID" & vbCrLf &
                    "       AND prdprc.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & ") as Rate" & vbCrLf &
                    " , cusorg.OfficeAddressBuildingNo + ' ' + cusorg.OfficeAddressStreet + ', ' + cusorgcity.Name + ', ' + cusorgcountry.Name as CompanyAddress " & vbCrLf &
                    " , CASE WHEN TransferDate IS NOT NULL " & vbCrLf &
                    "       THEN (SELECT " & vbCrLf &
                    "           ProductName " & vbCrLf &
                    "           FROM TransRentableAssets as transferto " & vbCrLf &
                    "           INNER JOIN LibraryRentableAssets as ass ON ass.ID = transferto.AssetID " & vbCrLf &
                    "           INNER JOIN MobizLibraryPIMProduct as pim ON pim.ID = ass.ProductID " & vbCrLf &
                    "           WHERE transferto.TransferFromID = trans.ID " & vbCrLf &
                    "       ) ELSE '' END as TransferTo " & vbCrLf &
                    " , CASE WHEN TransferFromID IS NOT NULL " & vbCrLf &
                    "       THEN (SELECT " & vbCrLf &
                    "           ProductName " & vbCrLf &
                    "           FROM TransRentableAssets as transferfrom " & vbCrLf &
                    "           INNER JOIN LibraryRentableAssets as ass ON ass.ID = transferfrom.AssetID " & vbCrLf &
                    "           INNER JOIN MobizLibraryPIMProduct as pim ON pim.ID = ass.ProductID " & vbCrLf &
                    "           WHERE transferfrom.ID = trans.TransferFromID " & vbCrLf &
                    "       ) ELSE '' END as TransferFrom " & vbCrLf &
                    " , resmadeby.FirstName + ' ' + resmadeby.Surname as ReservationWasMadeByName " & vbCrLf &
                    " FROM TransRentableAssets as trans " & vbCrLf &
                    " LEFT JOIN MobizLibraryOrganization as cusorg ON cusorg.ID = trans.OrgID " & vbCrLf &
                    " LEFT JOIN MobizLibraryOrganization as pubppol ON pubppol.ID = CASE WHEN ISNULL(convert(varchar(max), trans.ClientID), '') = '' THEN (SELECT TOP 1 ClientID FROM TransRentableAssetCustomer WHERE TransRentableAssetID = trans.ID) ELSE trans.ClientID END " & vbCrLf &
                    " LEFT JOIN LibraryPeople as pricusppol ON pricusppol.ID = CASE WHEN ISNULL(convert(varchar(max), trans.ClientID), '') = '' THEN (SELECT TOP 1 ClientID FROM TransRentableAssetCustomer WHERE TransRentableAssetID = trans.ID) ELSE trans.ClientID END " & vbCrLf &
                    " LEFT JOIN MobizLibraryCities as cusorgcity ON cusorgcity.ID = cusorg.OfficeAddressCityID " & vbCrLf &
                    " LEFT JOIN MobizLibraryCountries as cusorgcountry ON cusorgcountry.ID = cusorg.OfficeAddressCountryID " & vbCrLf &
                    " LEFT JOIN MobizLibraryPeople as resmadeby ON resmadeby.ID = trans.ReservationWasMadeBy " & vbCrLf &
                    " WHERE trans.ID = " & ReadyString(transid) & vbCrLf

            Dim details As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

            pSql = " SELECT " & vbCrLf &
                    " transcus.ClientID, ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) + ' ' + ISNULL(pubppol.PersonMiddleName, pricusppol.MiddleName) + ' ' + ISNULL(pubppol.PersonSurname, pricusppol.Surname) as CustomerName " & vbCrLf &
                    " , ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) as FirstName " & vbCrLf &
                    " , ISNULL(pubppol.PersonMiddleName, pricusppol.MiddleName) as MiddleName " & vbCrLf &
                    " , ISNULL(pubppol.PersonSurname, pricusppol.Surname) as Surname " & vbCrLf &
                    " , convert(varchar, pubppol.PersonDateOfBirth, 107) as DateOfBirth " & vbCrLf &
                    " , ppolcity.Name as CityName " & vbCrLf &
                    " , ppolcountry.Name as CountryName " & vbCrLf &
                    " , ISNULL(CONVERT(varchar(MAX), ppoladd.CityID), '') as CityID " & vbCrLf &
                    " , ISNULL(CONVERT(varchar(MAX), ppoladd.CountryID), '') as CountryID " & vbCrLf &
                    " , ppoladd.HouseBuildingNo " & vbCrLf &
                    " , ppoladd.Street " & vbCrLf &
                    " , ISNULL(CONVERT(varchar(MAX), ppoladd.ID), '') as AddressID " & vbCrLf &
                    " , ISNULL(CONVERT(varchar(MAX), ppolemail.ID), '') as EmailID " & vbCrLf &
                    " , ppolemail.EmailAddress " & vbCrLf &
                    " , ISNULL(CONVERT(varchar(MAX), ppolmobile.ID), '') as MobileID " & vbCrLf &
                    " , MobizLibraryTelcoPrefixes.Prefix + '' + ppolmobile.ContactNo as ContactNo " & vbCrLf &
                    " , ppolmobile.CountryID as CountryCodeID " & vbCrLf &
                    " , ppolmobile.TelCoPrefixID " & vbCrLf &
                    " FROM TransRentableAssetCustomer as transcus " & vbCrLf &
                    " LEFT JOIN MobizLibraryOrganization as pubppol ON pubppol.ID = transcus.ClientID " & vbCrLf &
                    " LEFT JOIN LibraryPeople as pricusppol ON pricusppol.ID = transcus.ClientID " & vbCrLf &
                    " OUTER APPLY (SELECT TOP 1 * FROM MobizLibraryPeopleAddress WHERE transcus.ClientID = PeopleID ORDER BY LastUpdated DESC) as ppoladd " & vbCrLf &
                    " OUTER APPLY (SELECT TOP 1 * FROM LibraryPeopleContactEmail WHERE PeopleID = transcus.ClientID AND EmailAddress <> '' ORDER BY LastUpdated DESC) as ppolemail " & vbCrLf &
                    " LEFT JOIN LibraryPeopleContactMobile as ppolmobile ON ppolmobile.ID = (SELECT TOP 1 ID FROM LibraryPeopleContactMobile WHERE PeopleID = transcus.ClientID) " & vbCrLf &
                    " LEFT JOIN MobizLibraryTelcoPrefixes ON MobizLibraryTelcoPrefixes.ID = ppolmobile.TelCoPrefixID " & vbCrLf &
                    " LEFT JOIN MobizLibraryCities as ppolcity ON ppolcity.ID = ppoladd.CityID " & vbCrLf &
                    " LEFT JOIN MobizLibraryCountries as ppolcountry ON ppolcountry.ID = ppoladd.CountryID " & vbCrLf &
                    " WHERE transcus.TransRentableAssetID = " & ReadyString(transid) & vbCrLf &
                    " ORDER BY transcus.SortOrderNo ASC "

            Dim rowCus As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

            If rowCus.Length = 0 Then
                pSql = " SELECT " & vbCrLf &
                    " trans.ClientID, ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) + ' ' + ISNULL(pubppol.PersonMiddleName, pricusppol.MiddleName) + ' ' + ISNULL(pubppol.PersonSurname, pricusppol.Surname) as CustomerName " & vbCrLf &
                    " , ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) as FirstName " & vbCrLf &
                    " , ISNULL(pubppol.PersonMiddleName, pricusppol.MiddleName) as MiddleName " & vbCrLf &
                    " , ISNULL(pubppol.PersonSurname, pricusppol.Surname) as Surname " & vbCrLf &
                    " , convert(varchar, pubppol.PersonDateOfBirth, 107) as DateOfBirth " & vbCrLf &
                    " , ppolcity.Name as CityName " & vbCrLf &
                    " , ppolcountry.Name as CountryName " & vbCrLf &
                    " , ISNULL(CONVERT(varchar(MAX), ppoladd.CityID), '') as CityID " & vbCrLf &
                    " , ISNULL(CONVERT(varchar(MAX), ppoladd.CountryID), '') as CountryID " & vbCrLf &
                    " , ppoladd.HouseBuildingNo " & vbCrLf &
                    " , ppoladd.Street " & vbCrLf &
                    " , ISNULL(CONVERT(varchar(MAX), ppoladd.ID), '') as AddressID " & vbCrLf &
                    " , ISNULL(CONVERT(varchar(MAX), ppolemail.ID), '') as EmailID " & vbCrLf &
                    " , ppolemail.EmailAddress " & vbCrLf &
                    " , ISNULL(CONVERT(varchar(MAX), ppolmobile.ID), '') as MobileID " & vbCrLf &
                    " , MobizLibraryTelcoPrefixes.Prefix + '' + ppolmobile.ContactNo as ContactNo " & vbCrLf &
                    " , ppolmobile.CountryID as CountryCodeID " & vbCrLf &
                    " , ppolmobile.TelCoPrefixID " & vbCrLf &
                    " FROM TransRentableAssets as trans " & vbCrLf &
                    " LEFT JOIN MobizLibraryOrganization as pubppol ON pubppol.ID = trans.ClientID " & vbCrLf &
                    " LEFT JOIN LibraryPeople as pricusppol ON pricusppol.ID = trans.ClientID " & vbCrLf &
                    " OUTER APPLY (SELECT TOP 1 * FROM MobizLibraryPeopleAddress WHERE trans.ClientID = PeopleID ORDER BY LastUpdated DESC) as ppoladd " & vbCrLf &
                    " OUTER APPLY (SELECT TOP 1 * FROM LibraryPeopleContactEmail WHERE PeopleID = trans.ClientID AND EmailAddress <> '' ORDER BY LastUpdated DESC) as ppolemail " & vbCrLf &
                    " LEFT JOIN LibraryPeopleContactMobile as ppolmobile ON ppolmobile.ID = (SELECT TOP 1 ID FROM LibraryPeopleContactMobile WHERE PeopleID = trans.ClientID) " & vbCrLf &
                    " LEFT JOIN MobizLibraryTelcoPrefixes ON MobizLibraryTelcoPrefixes.ID = ppolmobile.TelCoPrefixID " & vbCrLf &
                    " LEFT JOIN MobizLibraryCities as ppolcity ON ppolcity.ID = ppoladd.CityID " & vbCrLf &
                    " LEFT JOIN MobizLibraryCountries as ppolcountry ON ppolcountry.ID = ppoladd.CountryID " & vbCrLf &
                    " WHERE trans.ID = " & ReadyString(transid.ToString())

                rowCus = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))
            End If

            For Each cus As Object In rowCus
                pSql = "SELECT req.*, 'edit' as State " & vbCrLf &
                    " , ppol.FirstName + ' ' + ppol.Surname as ModifiedBy" & vbCrLf &
                    " , convert(varchar, req.LastUpdated, 120) as ModifiedDate " & vbCrLf &
                    " FROM TransRentableAssetCustomerRequest as req" & vbCrLf &
                    " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = req.UserID " & vbCrLf &
                    " WHERE req.TransRentableAssetID = " & ReadyString(transid) &
                    " AND req.IsActive = 1 AND req.Action <> 'Deleted' ORDER BY req.LastUpdated DESC"
                cus.Add("Request", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

                If CBool2(IsOutOfOrder) = False Then
                    pSql = "SELECT req.*, 'edit' as State " & vbCrLf &
                        " , ppol.FirstName + ' ' + ppol.Surname as ModifiedBy" & vbCrLf &
                        " , convert(varchar, req.LastUpdated, 120) as ModifiedDate " & vbCrLf &
                        " FROM TransRentableAssetCustomerRequest as req" & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = req.UserID " & vbCrLf &
                        " WHERE req.ClientID = " & ReadyString(cus("ClientID")) &
                        " AND req.IsDone = 1 AND req.IsActive = 1 ORDER BY req.LastUpdated DESC"
                    cus.Add("PreviousRequest", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))
                End If
            Next cus

            pSql = "SELECT ID as TransOCId, Qty as RequestQty, OtherChargesID " & vbCrLf &
                    " FROM TransRentableAssetOtherCharges WHERE TransRentableAssetID = " & ReadyString(transid)

            Dim othercharges As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

            details(0)("Customer") = rowCus
            details(0)("OtherCharges") = othercharges

            Return Json(New With {Key .Details = details})

            'Return Json(New With {Key .Details = details, .Customer = rowCus, .OtherCharges = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))})
        End Function

        Function SearchSku() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String
            Dim keywords As String = Common.WebRequest.Data("keywords")

            Try
                pSql = "SELECT TOP 50 * FROM ( " & vbCrLf &
                        " SELECT pim.ID as ProductID " & vbCrLf &
                        " , pim.ProductNo " & vbCrLf &
                        " , pim.ProductName " & vbCrLf &
                        " , cat.C0020 as Category " & vbCrLf &
                        " , CONVERT(varchar, pimmsr.Quantity) as Qty " & vbCrLf &
                        " , lbl.Label as UomName " & vbCrLf &
                        " , pim.ProductSearchString " & vbCrLf &
                        " , msr.ID as UomID" & vbCrLf &
                        " FROM MobizLibraryPimProduct as pim " & vbCrLf &
                        " INNER JOIN LibraryOrganizationPIM as orgpim ON orgpim.ProductID = pim.ID  " & vbCrLf &
                        " INNER JOIN MobizLibraryPIMProductMeasurement as pimmsr ON pimmsr.ProductID = pim.ID  " & vbCrLf &
                        " INNER JOIN MobizLibraryPIMMeasure as msr ON msr.ID = pimmsr.UnitMeasureID " & vbCrLf &
                        " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON (msr.MeasureNameID = lbl.ID) " & vbCrLf &
                        " INNER JOIN MobizLibraryPIMProductVariableFeatures001 as cat ON cat.ProductID = pim.ID  " & vbCrLf &
                        " WHERE pim.PublishedByID = " & ReadyString(WebRequest.Instance.OrgID) & vbCrLf &
                        " AND ISNULL(pim.IsAsset, 0) = 0 " & vbCrLf &
                        " AND orgpim.IsDeActivated = 0 " & vbCrLf &
                    ") as q1 " & If(keywords = "", "", "WHERE ProductSearchString LIKE " & ReadyStringWild(keywords)) & vbCrLf &
                    " ORDER BY ProductName ASC "

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function SaveOtherCharges() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim msg As String
            Dim pSql As String

            Try
                Dim p As New InsertUpdate("LibraryAssetOtherCharges")
                Dim data As Object = Common.WebRequest.AllData("data")

                'pSql = "SELECT IsActive FROM LibraryAssetOtherCharges WHERE ProductID = " & ReadyString(data("SkuID"))
                'Dim row As Object = Common.WebRequest.ProData.Execute(pSql).Rows
                'If row.Count > 0 Then
                '    msg = "Selected SKU already exist and the current status is <b>" & If(row(0)("IsActive") = 1, "active", "inactive") & "</b>"
                'Else
                p("ProductID") = ReadyString(data("SkuID"))
                p("Qty") = ReadyString(data("Qty"))
                p("InvoicingType") = ReadyString(data("InvoicingType"))
                p("UomID") = ReadyString(data("UomID"))
                p("PriceLevelID") = ReadyString(data("PriceLevelID"))
                p("IsRequired") = ReadyString(data("IsRequired"))
                p("IsActive") = ReadyString(data("IsActive"))
                p("LastUpdated") = "getdate()"
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)

                If data("ID") = "" Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(data("ID")))
                End If

                Common.WebRequest.ProData.Execute(pSql)

                msg = "saved"
                'End If
            Catch ex As Exception
                AppendToLog(ex.Message)
                msg = ex.Message
            End Try

            Return JsonDictionary.Serialize("Results", msg)
        End Function

        Function GetListOtherCharges() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim pSql As String
                Dim keyword As String = Common.WebRequest.Data("keywords")
                Dim status As String = Common.WebRequest.Data("Status")

                pSql = "SELECT oc.*, ppol.FirstName + ' ' + ppol.Surname as ModifiedBy, " & vbCrLf &
                        " lbl.Label as UomName, pim.ProductName, pim.ProductNo " & vbCrLf &
                        " , prdprc.Price " & vbCrLf &
                        " FROM LibraryAssetOtherCharges as oc " & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as ppol ON ppol.ID = oc.UserID " & vbCrLf &
                        " INNER JOIN MobizLibraryPIMMeasure as msr ON msr.ID = oc.UomID " & vbCrLf &
                        " INNER JOIN MobizLibraryCountriesLanguageLabel lbl ON msr.MeasureNameID = lbl.ID " & vbCrLf &
                        " INNER JOIN MobizLibraryPIMProduct as pim ON pim.ID = oc.ProductID " & vbCrLf &
                        " INNER JOIN MobizLibraryPIMProductPriceLevel prdprc ON prdprc.PriceLevelID = oc.PriceLevelID and prdprc.ProductID = oc.ProductID and prdprc.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID)

                If status = "all" Then
                    pSql += " WHERE oc.IsActive = 0 OR oc.IsActive = 1 "
                Else
                    pSql += " WHERE oc.IsActive = " & status
                End If

                If keyword <> "" Then
                    pSql += " AND pim.ProductName LIKE " & ReadyString(keyword)
                End If

                pSql += " ORDER BY oc.LastUpdated DESC "

                'gisakay naku ang pagsave sa orgid pagmaopen ang asset management
                Dim pFilename As String
                Dim DoorLockSystem As String

                Dim pSqlDoor = "SELECT DefaultValue FROM SystemDefaults WHERE Description = 'DoorLockSystem'"
                DoorLockSystem = CStr2(Common.WebRequest.ProData.ExecuteScalar(pSqlDoor))

                pFilename = Common.ServerPath & "temp\UUID\"

                If Not System.IO.File.Exists(pFilename) Then
                    IO.Directory.CreateDirectory(pFilename)
                End If

                pFilename &= CStr2(Common.WebRequest.Data("UUID")) & "ORG.dat"

                If System.IO.File.Exists(pFilename) Then
                    System.IO.File.Delete(pFilename)
                End If

                Using sw As System.IO.StreamWriter = System.IO.File.CreateText(pFilename)

                    sw.WriteLine(CStr2(Common.WebRequest.Data("OrgID")))
                    sw.WriteLine(DoorLockSystem)
                End Using
                'end

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function GetCardInformation() As String
            Dim pSql As String
            Dim dtCardInformation As DataTable

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "Select TOP 1 * from TransPendingCardRequest WHERE [Status]='ReadCard'"

            dtCardInformation = Common.WebRequest.ProData.Execute(pSql)

            Dim pOut As New JsonDictionary

            pOut.Add("Results", dtCardInformation)

            Return pOut.Serialize

            'Return JsonDictionary.Serialize("Results", dtWritableCards)
        End Function

        Function UpdateLostCardKeyCounter() As String
            Dim pSql As String
            Dim p As InsertUpdate

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pSql = "Update TransRentableAssets set LostCardKeyCount=LostCardKeyCount+1 WHERE ID = " & ReadyString(Common.WebRequest.Data("TransID"))
            Common.WebRequest.ProData.Execute(pSql)

            p = New InsertUpdate("TransRentableAssetOtherCharges")
            p("ID") = ReadyString(Guid.NewGuid.ToString())
            p("TransRentableAssetID") = ReadyString(Common.WebRequest.Data("TransID"))
            p("Qty") = 1
            p("OtherChargesID") = ReadyString(Common.WebRequest.Data("ChargesID"))

            pSql = p.GetInsert()
            Common.WebRequest.ProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", "Success")
        End Function

        Function SaveCardCreation() As String
            Dim p As New InsertUpdate("TransPendingCardRequest")
            Dim pSql As String
            Dim data As Object = Common.WebRequest.AllData("data")

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            p("ID") = ReadyString(data("ID"))
            p("RoomNo") = ReadyString(data("RoomNo"))
            p("CardType") = ReadyString(data("CardType"))
            p("CheckInTime") = ReadyString(data("CheckInTime"))
            p("CheckOutTime") = ReadyString(data("CheckOutTime"))
            p("Status") = ReadyString(data("Status"))
            p("DateRequested") = "GETDATE()"

            pSql = p.GetInsert()

            Common.WebRequest.ProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", "Success")
        End Function

        Function FollowupCardCreation() As String
            Dim pSql As String
            Dim pDT As DataTable

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim data As Object = Common.WebRequest.AllData("data")

            pSql = "Select * from TransPendingCardRequest WHERE ID = " & ReadyString(data("ID"))

            pDT = Common.WebRequest.ProData.Execute(pSql)

            Return JsonDictionary.Serialize("Results", pDT)
        End Function

        Function GetWritableCards(Optional CardReaderType As String = "") As String
            Dim pSql As String
            Dim dtWritableCards As DataTable

            pSql = "Select TOP 1 * from TransPendingCardRequest WHERE [Status]='Pending'"

            'pSql = "Select top 1 " & vbCrLf &
            '"MobizLibraryPIMProduct.ID, " & vbCrLf &
            '"MobizLibraryPIMProduct.ProductNo, MobizLibraryPIMProduct.ProductName, " & vbCrLf &
            '"CardType, " & vbCrLf &
            '"CheckedInUsedOn, " & vbCrLf &
            '"CheckedOutReturnedOn " & vbCrLf &
            '" FROM " & vbCrLf &
            '"TransRentableAssets " & vbCrLf &
            '"inner join LibraryRentableAssets on LibraryRentableAssets.ID = TransRentableAssets.AssetID " & vbCrLf &
            '"inner join MobizLibraryPIMProduct on MobizLibraryPIMProduct.ID = LibraryRentableAssets.ProductID " & vbCrLf &
            '"WHERE " & vbCrLf &
            '"TransRentableAssets.IsCancelled IS NULL  " & vbCrLf &
            '"AND TransRentableAssets.ActualCheckedOutReturnedOn IS NULL " & vbCrLf &
            '"AND TransRentableAssets.CheckedInUsedOn IS NOT NULL " & vbCrLf &
            '"AND (NoOfRoomCardRequested-NoOfRoomCardIssued)>0"

            dtWritableCards = Common.WebRequest.ProData.Execute(pSql)

            Dim pOut As New JsonDictionary

            pOut.Add("OrgID", WebRequest.Instance.OrgID.ToString.Substring(0, 36))
            Dim pCardReaderType = CardReaderType.Replace(vbCr, "").Replace(vbLf, "")
            AppendToLog("vb7UE97AS347! pCardReaderType = " & pCardReaderType)
            pOut.Add("CardReaderType", pCardReaderType)
            pOut.Add("Results", dtWritableCards)

            Return pOut.Serialize

            'Return JsonDictionary.Serialize("Results", dtWritableCards)
        End Function

        Function UpdateCardCreation() As String
            Dim pSql As String
            Dim Status As String
            Dim Message As String
            Dim ID As String

            Status = CStr2(Common.WebRequest.QueryString("Status"))
            ID = CStr2(Common.WebRequest.QueryString("TransID"))

            If Status = "Created" Then
                pSql = "Update TransPendingCardRequest set Status='Created', DateCreated=getdate() WHERE ID=" & ReadyString(ID)
            Else
                Message = CStr2(Common.WebRequest.QueryString("Message"))
                If Message = "DeviceDisconnected" Then
                    Message = "Please connect card reader device to your PC/Laptop"
                    pSql = "Update TransPendingCardRequest set StatusMessage = " & ReadyString(Message) & " WHERE ID=" & ReadyString(ID)
                ElseIf Message = "CannotWriteToCard" Then
                    Message = "Please put card on top of the card reader"
                    pSql = "Update TransPendingCardRequest set StatusMessage = " & ReadyString(Message) & " WHERE ID=" & ReadyString(ID)
                End If

            End If

            Try
                Common.WebRequest.ProData.Execute(pSql)
                Return True
            Catch ex As Exception
                Return False
            End Try

        End Function

        Function UpdateReadCard() As String
            Dim pSql As String
            Dim RoomNo As String
            Dim CheckInTime As String
            Dim CheckOutTime As String
            Dim pID As String

            RoomNo = CStr2(Common.WebRequest.QueryString("RoomNo")).Replace("%20", " ")
            CheckInTime = CStr2(Common.WebRequest.QueryString("CheckInTime")).Replace("%20", " ")
            CheckOutTime = CStr2(Common.WebRequest.QueryString("CheckOutTime")).Replace("%20", " ")

            pSql = "Select ID from TransPendingCardRequest WHERE Status = 'ReadCard'"
            pID = CStr2(Common.WebRequest.ProData.ExecuteScalar(pSql))

            If pID.Length > 0 Then
                pSql = "Update TransPendingCardRequest set " & vbCrLf &
                        "RoomNo = " & ReadyString(RoomNo) & "," & vbCrLf &
                        "CheckInTime=" & ReadyString(CheckInTime) & "," & vbCrLf &
                        "CheckOutTime=" & ReadyString(CheckOutTime) & vbCrLf &
                        " WHERE ID = " & ReadyString(pID)
            Else
                pSql = "Insert into TransPendingCardRequest(RoomNo, CardType, CheckInTime, CheckOutTime, Status, DateRequested, DateCreated)" & vbCrLf &
                        "VALUES(" & vbCrLf &
                        ReadyString(RoomNo) & "," & vbCrLf &
                        ReadyString("Guest Card") & "," & vbCrLf &
                        ReadyString(CheckInTime) & "," & vbCrLf &
                        ReadyString(CheckOutTime) & "," & vbCrLf &
                        ReadyString("ReadCard") & "," & vbCrLf &
                        "GETDATE()" & "," & vbCrLf &
                        "GETDATE()" & vbCrLf &
                        ")"
            End If

            Try
                Common.WebRequest.ProData.Execute(pSql)
                Return True
            Catch ex As Exception
                Return False
            End Try

        End Function

        Function DownloadAssetFile()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim zip As New ProZip
                Dim pFilename As String = Request.Params.Get("FileName")
                Dim guestname As String
                Dim zipfile As String = Server.MapPath("~/FileTemplates/" & pFilename & ".zip")

                pSql = "SELECT ClientID FROM TransRentableAssetCustomer WHERE TransRentableAssetID = " & ReadyString(Request.Params.Get("TransID"))
                Dim pDT As DataTable = Common.WebRequest.ProData.Execute(pSql)

                If pDT.Rows.Count = 0 Then
                    pSql = "SELECT ClientID FROM TransRentableAssets WHERE ID = " & ReadyString(Request.Params.Get("TransID"))
                    pDT = Common.WebRequest.ProData.Execute(pSql)
                End If

                GenerateCheckInData(pDT, Request.Params.Get("TransID"), zip, pFilename)

                Dim file As System.IO.FileInfo

                If pDT.Rows.Count > 1 Then
                    file = New System.IO.FileInfo(zipfile)

                    Response.Clear()
                    Response.ContentType = "application/octet-stream"
                    Response.AddHeader("content-disposition", "attachment; filename=" & pFilename & ".zip")
                    zip.Save(zipfile)
                    Response.WriteFile(file.FullName)
                    Response.End()
                Else
                    file = New System.IO.FileInfo(pFilename)

                    Response.Clear()
                    Response.AddHeader("Content-Disposition", "attachment; filename=" & Request.Params.Get("FileName") & ".pdf")
                    Response.AddHeader("Content-Length", file.Length.ToString())
                    Response.ContentType = "application/octet-stream"
                    Response.WriteFile(file.FullName)
                    Response.End()
                End If

                Return JsonDictionary.Serialize("Results", "Exist")

                Return JsonDictionary.Serialize("Results", "ok")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GenerateCheckInData(ByVal dtClient As DataTable,
                                     ByVal transid As String,
                                     ByRef zip As ProZip,
                                     Optional ByRef pFilename As String = "")
            Dim pReport As ProReport
            Dim pSql As String
            Dim copiedfilename As String = String.Copy(pFilename)
            Dim displaytotal = WebRequest.Instance.Business.SystemDefaults.DisplayAssetPrice

            For Each Client As Object In dtClient.Rows
                pSql = " SELECT " & vbCrLf &
                    " trans.ID, " & vbCrLf &
                    " trans.TransactionNumber, " & vbCrLf &
                    " trans.DateReserved, " & vbCrLf &
                    " CheckedInUsedOn as CheckedInUsedOn, " & vbCrLf &
                    " ISNULL(ActualCheckedOutReturnedOn, CheckedOutReturnedOn) as CheckedOutReturnedOn, " & vbCrLf &
                    " pimpro.ProductName as RoomNo, " & vbCrLf &
                    " ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) + ' ' + ISNULL(pubppol.PersonMiddleName, pricusppol.MiddleName) + ' ' + ISNULL(pubppol.PersonSurname, pricusppol.Surname) as GuestName, " & vbCrLf &
                    " convert(varchar, ISNULL(pubppol.PersonDateOfBirth, pricusppol.DateOfBirth), 23) as DateOfBirth, " & vbCrLf &
                    " '0' + MobizLibraryTelcoPrefixes.Prefix + '' + ppolmobile.ContactNo as ContactNo, " & vbCrLf &
                    " ppolemail.EmailAddress, " & vbCrLf &
                    " CASE WHEN prdfea1.C0020 = '' THEN 'Other(s)' ELSE prdfea1.C0020 END as RoomType, " & vbCrLf &
                    " trans.NoOfAdults, trans.NoOfChild, " & vbCrLf &
                    " cusorg.CompanyName, " & vbCrLf &
                    " cusorg.OfficeAddressBuildingNo + ' ' + cusorg.OfficeAddressStreet + ' ' + cusorgcity.Name + ', ' + cusorgcountry.Name as CompanyAddress, " & vbCrLf &
                    " (SELECT prdprc.Price FROM TransRentableAssets as transren " & vbCrLf &
                    "   INNER JOIN LibraryRentableAssets as ren ON transren.AssetID = ren.ID " & vbCrLf &
                    "   INNER JOIN MobizLibraryPIMProductPriceLevel as prdprc ON ren.ProductID = prdprc.ProductID " & vbCrLf &
                    "   INNER JOIN MobizLibraryPIMPriceLevel prc ON prdprc.PriceLevelID = prc.ID " & vbCrLf &
                    "   WHERE prdprc.PriceLevelID = trans.PriceLevelID AND prdprc.ProductID = ren.ProductID AND transren.ID = trans.ID " & vbCrLf &
                    "       AND prdprc.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & ") as Rate, " & vbCrLf &
                    " ppoladd.Street + ' ' + ppoladd.HouseBuildingNo + ' ' + ppolcity.Name + ', ' + ppolcountry.Name as GuestAddress, " & vbCrLf &
                    " ppolCheckin.FirstName + ' ' + ppolCheckin.MiddleName + ' ' + ppolCheckin.Surname as CheckedInBy, " & vbCrLf &
                    " ppolCheckout.FirstName + ' ' + ppolCheckout.MiddleName + ' ' + ppolCheckout.Surname as CheckedOutBy, " & vbCrLf &
                    " resmadeby.FirstName + ' ' + resmadeby.Surname as ReservationWasMadeByName " & vbCrLf &
                    " FROM TransRentableAssets as trans " & vbCrLf &
                    " INNER JOIN LibraryRentableAssets as resass ON resass.ID = trans.AssetID " & vbCrLf &
                    " INNER JOIN MobizLibraryPIMProduct as pimpro ON pimpro.ID = resass.ProductID " & vbCrLf &
                    " INNER JOIN MobizLibraryPIMProductVariableFeatures001 as cat ON cat.ProductID = resass.ProductID " & vbCrLf &
                    " LEFT JOIN MobizLibraryPIMProductVariableFeatures001 prdfea1 ON (prdfea1.ProductID = pimpro.ID) " & vbCrLf &
                    " LEFT JOIN MobizLibraryOrganization as pubppol ON pubppol.ID = " & ReadyString(Client("ClientID")) & vbCrLf &
                    " LEFT JOIN LibraryPeople as pricusppol ON pricusppol.ID = " & ReadyString(Client("ClientID")) & vbCrLf &
                    " OUTER APPLY(SELECT TOP 1 * FROM LibraryPeopleContactEmail WHERE PeopleID = " & ReadyString(Client("ClientID")) & " AND EmailAddress <> '' ORDER BY LastUpdated DESC) as ppolemail " & vbCrLf &
                    " LEFT JOIN LibraryPeopleContactMobile as ppolmobile ON ppolmobile.ID = (SELECT TOP 1 ID FROM LibraryPeopleContactMobile WHERE PeopleID = " & ReadyString(Client("ClientID")) & ") " & vbCrLf &
                    " LEFT JOIN MobizLibraryTelcoPrefixes ON MobizLibraryTelcoPrefixes.ID = ppolmobile.TelCoPrefixID " & vbCrLf &
                    " LEFT JOIN MobizLibraryOrganization as cusorg ON cusorg.ID = trans.OrgID " & vbCrLf &
                    " LEFT JOIN MobizLibraryCities as cusorgcity ON cusorgcity.ID = cusorg.OfficeAddressCityID " & vbCrLf &
                    " LEFT JOIN MobizLibraryCountries as cusorgcountry ON cusorgcountry.ID = cusorg.OfficeAddressCountryID " & vbCrLf &
                    " OUTER APPLY (SELECT TOP 1 * FROM MobizLibraryPeopleAddress WHERE " & ReadyString(Client("ClientID")) & " = PeopleID ORDER BY LastUpdated DESC) as ppoladd " & vbCrLf &
                    " LEFT JOIN MobizLibraryCities as ppolcity ON ppolcity.ID = ppoladd.CityID " & vbCrLf &
                    " LEFT JOIN MobizLibraryCountries as ppolcountry ON ppolcountry.ID = ppoladd.CountryID " & vbCrLf &
                    " LEFT JOIN MobizLibraryPeople as ppolCheckin ON ppolCheckin.ID = trans.CheckedInByUser " & vbCrLf &
                    " LEFT JOIN MobizLibraryPeople as ppolCheckout ON ppolCheckout.ID = trans.CheckedOutByUser " & vbCrLf &
                    " LEFT JOIN MobizLibraryPeople as resmadeby ON resmadeby.ID = trans.ReservationWasMadeBy " & vbCrLf &
                    " WHERE trans.ID = " & ReadyString(transid)

                Dim row As Object = Common.WebRequest.ProData.Execute(pSql).Rows(0)
                Dim newpDt As New DataTable
                Dim newRow As DataRow
                Dim guestname As String = row("GuestName")

                newRow = newpDt.NewRow

                newpDt.Columns.Add("CheckedInUsedOn")
                newRow(0) = Format(CDate(row("CheckedInUsedOn")), "yyyy-MM-dd hh:mm tt")
                newpDt.Columns.Add("CheckedOutReturnedOn")
                newRow(1) = Format(CDate(row("CheckedOutReturnedOn")), "yyyy-MM-dd hh:mm tt")
                newpDt.Columns.Add("RoomNo")
                newRow(2) = row("RoomNo")
                newpDt.Columns.Add("GuestName")
                newRow(3) = row("GuestName")
                newpDt.Columns.Add("DateOfBirth")
                newRow(4) = row("DateOfBirth")
                newpDt.Columns.Add("ContactNo")
                newRow(5) = row("ContactNo")
                newpDt.Columns.Add("EmailAddress")
                newRow(6) = row("EmailAddress")
                newpDt.Columns.Add("RoomType")
                newRow(7) = row("RoomType")
                newpDt.Columns.Add("NoOfAdults")
                newRow(8) = row("NoOfAdults")
                newpDt.Columns.Add("NoOfChild")
                newRow(9) = row("NoOfChild")
                newpDt.Columns.Add("CompanyName")
                newRow(10) = row("CompanyName")
                newpDt.Columns.Add("Rate")
                newRow(11) = FormatNumber(row("Rate"), 2)
                newpDt.Columns.Add("ConfirmationNumber")
                newRow(12) = row("TransactionNumber")
                newpDt.Columns.Add("GuestAddress")
                newRow(13) = row("GuestAddress")
                newpDt.Columns.Add("NoOfNights")
                newRow(14) = DateDiff(DateInterval.Day, Convert.ToDateTime(row("CheckedInUsedOn")), Convert.ToDateTime(row("CheckedOutReturnedOn")))
                newpDt.Columns.Add("CheckedInBy")
                newRow(15) = row("CheckedInBy")
                newpDt.Columns.Add("CheckedOutBy")
                newRow(16) = row("CheckedOutBy")
                newpDt.Columns.Add("CompanyAddress")
                newRow(17) = row("CompanyAddress")
                newpDt.Columns.Add("DateReserved")

                If String.IsNullOrWhiteSpace(CStr2(row("DateReserved"))) Then
                    newRow(18) = Nothing
                Else
                    newRow(18) = Format(CDate(row("DateReserved")), "yyyy-MM-dd hh:mm tt")
                End If

                newpDt.Columns.Add("ReservationWasMadeByName")
                newRow(19) = row("ReservationWasMadeByName")

                pSql = "SELECT TOP 1 BusinessTradeName FROM MobizLibraryOrganizationTradeName WHERE OrganizationID = " & ReadyString(WebRequest.Instance.OrgID)
                row = Common.WebRequest.ProData.Execute(pSql).Rows(0)

                newpDt.Columns.Add("BusinessTradeName")
                newRow(20) = row("BusinessTradeName")

                newpDt.Columns.Add("CompanyLogo")
                Dim logo As String = Common.ServerPath & "\Content\OrganizationLogo\" & WebRequest.Instance.OrgID & ".png"
                If IO.File.Exists(logo) Then
                    newRow(21) = logo
                Else
                    newRow(21) = ""
                End If

                newpDt.Rows.Add(newRow)

                WebRequest.Instance.Business.LibraryRepeatingTransactions.ReconstructionData(transid, WebRequest.Instance.OrgID)

                pSql = " SELECT TransInvoiceItems.ID" & vbCrLf &
                    " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf &
                    "  ,ISNULL(NULLIF(LTRIM(RTRIM(MobizLibraryPIMProduct.Brand)), '') + ' ', '') + MobizLibraryPIMProduct.ProductName AS ProductName" & vbCrLf &
                    "  ,TransInvoiceItems.Quantity" & vbCrLf &
                    "  ,TransInvoiceItems.QuantityBad" & vbCrLf &
                    "  ,TransInvoiceItems.HiddenServedQuantity" & vbCrLf &
                    "  ,TransInvoiceItems.OrderedQuantity" & vbCrLf &
                    "  ,TransInvoiceItems.UnitMeasure" & vbCrLf &
                    "  ,CONVERT(varchar, TransInvoiceItems.TermDiscount) + '% ' + CONVERT(varchar, TransInvoiceItems.DiscountIfPaidWithIn) + ' NET ' + CONVERT(varchar, TransInvoiceItems.NetDue) AS Terms" & vbCrLf &
                    "  ,TransInvoiceItems.UnitPrice" & vbCrLf

                If displaytotal = "roundup" Then
                    pSql &= "  ,CEILING(TransInvoiceItems.SubTotal) as SubTotal" & vbCrLf
                ElseIf displaytotal = "roundup" Then
                    pSql &= "  ,FLOOR(TransInvoiceItems.SubTotal) as SubTotal" & vbCrLf
                Else
                    pSql &= "  ,TransInvoiceItems.SubTotal" & vbCrLf
                End If

                pSql &= "  ,TransInvoiceItems.UnitPrice * TransInvoiceItems.OrderedQuantity AS OrderedAmount" & vbCrLf &
                    "  ,TransInvoiceItems.UnitPrice * TransInvoiceItems.Quantity AS ServedAmount" & vbCrLf &
                    "  ,TransInvoiceItems.DiscountIfPaidWithIn" & vbCrLf &
                    "  ,TransInvoiceItems.NetDue" & vbCrLf &
                    "  ,TransInvoiceItems.TermNumericValue" & vbCrLf &
                    "  ,TransInvoiceItems.TermDiscount AS TermDiscountNumeric" & vbCrLf &
                    "  ,TransInvoiceItems.ActualQuantity" & vbCrLf &
                    "  ,TransInvoiceItems.ProductID" & vbCrLf &
                    "  ,TransInvoiceItems.InvoiceID" & vbCrLf &
                    "  ,TransInvoiceItems.LineNumber" & vbCrLf &
                    "  ,TransInvoiceItems.PriceLevelID" & vbCrLf &
                    "  ,TransInvoiceItems.LineCost" & vbCrLf &
                    "  ,TransInvoiceItems.RequestedUnitPrice" & vbCrLf &
                    "  ,TransInvoiceItems.TaxPercent" & vbCrLf &
                    "  ,TransInvoiceItems.TaxAmount" & vbCrLf &
                    "  ,TransInvoiceItems.TaxSales" & vbCrLf &
                    "  ,TransInvoiceItems.NonVATSale" & vbCrLf &
                    "  ,TransInvoiceItems.VATExemptSale" & vbCrLf &
                    "  ,TransInvoiceItems.ZeroRatedSale" & vbCrLf &
                    "  ,TransInvoiceItems.ServedQtyIsEdited" & vbCrLf &
                    "  ,TransInvoice.InvoiceDate" & vbCrLf &
                    " FROM TransInvoiceItems" & vbCrLf &
                    " LEFT JOIN MobizLibraryPIMProduct ON TransInvoiceItems.ProductID = MobizLibraryPIMProduct.ID" & vbCrLf &
                    " JOIN TransInvoice ON TransInvoice.ID = TransInvoiceItems.InvoiceID" & vbCrLf &
                    " JOIN LibraryRepeatingTransactions ON LibraryRepeatingTransactions.ID = transinvoice.RepeatingTransactionID" & vbCrLf &
                    " JOIN TransRentableAssets ON TransRentableAssets.ID = LibraryRepeatingTransactions.SourceTransRentableAssetID" & vbCrLf &
                    " WHERE TransRentableAssets.ID = " & ReadyString(transid) & vbCrLf &
                    " AND TransInvoice.CancelledByID IS NULL" & vbCrLf &
                    " ORDER BY ProductName"
                Dim pDtProducts = Common.WebRequest.ProData.Execute(pSql)
                pDtProducts.Columns.Add("Credit", GetType(Decimal))
                Dim pDtIds = ExtractFieldValues(pDtProducts, "InvoiceID")
                Dim pInvoiceIds As String
                For Each pDrId In pDtIds.Rows
                    pInvoiceIds &= pDrId("InvoiceID")
                Next pDrId
                If String.IsNullOrWhiteSpace(pInvoiceIds) = False Then
                    pSql = " SELECT TransCustomerPaymentInvoices.AmountPaid" & vbCrLf &
                        " ,TransCustomerPayment.PaymentDate" & vbCrLf &
                        " FROM TransCustomerPaymentInvoices" & vbCrLf &
                        " JOIN TransCustomerPayment ON TransCustomerPayment.ID = TransCustomerPaymentInvoices.CustomerPaymentID" & vbCrLf &
                        " WHERE TransCustomerPaymentInvoices.InvoiceID IN (" & pInvoiceIds & ")" & vbCrLf &
                        " AND TransCustomerPaymentInvoices.AmountPaid > 0"
                    Dim pDtPayments = Common.WebRequest.ProData.Execute(pSql)
                    For Each pDrPayment In pDtPayments.Rows
                        Dim pDrProduct = pDtProducts.NewRow
                        pDrProduct("InvoiceDate") = pDrPayment("PaymentDate")
                        pDrProduct("ProductName") = "Payment"
                        pDrProduct("Credit") = pDrPayment("AmountPaid")
                        pDtProducts.Rows.Add(pDrProduct)
                    Next
                End If

                pDtProducts = New DataView(pDtProducts, "", "InvoiceDate, Credit DESC", DataViewRowState.CurrentRows).ToTable
                pDtProducts.Columns.Add("Balance", GetType(Decimal))
                Dim pBalance As Decimal = 0
                For Each pDrProduct In pDtProducts.Rows
                    pBalance += CDec2(pDrProduct("SubTotal")) - CDec2(pDrProduct("Credit"))
                    If displaytotal = "roundup" Then
                        pBalance = Math.Ceiling(pBalance)
                    ElseIf displaytotal = "rounddown" Then
                        pBalance = Math.Floor(pBalance)
                    End If
                    pDrProduct("Balance") = pBalance
                Next
                pReport = New ProReport()
                pReport.AutoFillHeader = False
                pReport.SourceFilename = pFilename

                If dtClient.Rows.Count > 1 Then
                    pFilename = Common.ServerPath & "FileTemplates\" & pFilename & " of " & guestname & ".pdf"
                Else
                    pFilename = Common.ServerPath & "FileTemplates\" & pFilename & ".pdf"
                End If

                Dim clsSalesReport As New ReportPOS
                Dim pDtHeader = clsSalesReport.getReportHeader(Common.WebRequest.ProData, Request.Params.Get("FowstID"), "")
                pReport.AddTable("Info", newpDt)
                pReport.AddTable("InvoicesItems", pDtProducts)
                pReport.AddTable("SubscriberInfo", pDtHeader)
                ProReport.DebugXmlFilename = ProWeb.Common.ServerPath + "Reports\Stimulsoft XML Data.xml"

                pReport.ExportXmlFile()
                pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename)

                If dtClient.Rows.Count > 1 Then
                    zip.AddFile(Server.MapPath("~/FileTemplates/" & copiedfilename & " of " & guestname & ".pdf"), "")
                    My.Computer.FileSystem.DeleteFile(pFilename)
                    pFilename = copiedfilename
                End If
            Next Client
        End Function

        Function DownloadSalesReport()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim newpDt As New DataTable
            Dim newRow As DataRow
            Dim pFileName As String
            Dim pReport As ProReport
            Dim salesdate As String = Format(CDate(Request.Params.Get("Date")), "MMM dd, yyyy")
            newRow = newpDt.NewRow

            newpDt.Columns.Add("Date")
            newRow(0) = salesdate
            newpDt.Rows.Add(newRow)

            Dim pSelect As String = " SELECT " & vbCrLf &
                                    " pimpro.ProductName as RoomNo, " & vbCrLf &
                                    " (SELECT DISTINCT STUFF((SELECT DISTINCT ', ' +  " & vbCrLf &
                                    "   ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) + ' ' + ISNULL(pubppol.PersonSurname, pricusppol.Surname) " & vbCrLf &
                                    "   FROM TransRentableAssetCustomer as transcus " & vbCrLf &
                                    "   LEFT JOIN MobizLibraryOrganization as pubppol ON pubppol.ID = transcus.ClientID " & vbCrLf &
                                    "   LEFT JOIN LibraryPeople as pricusppol ON pricusppol.ID = transcus.ClientID " & vbCrLf &
                                    "   WHERE transcus.TransRentableAssetID = trans.ID " & vbCrLf &
                                    "   FOR XML PATH('') " & vbCrLf &
                                    " ), 1, 1,'')) as CustomerName, " & vbCrLf &
                                    " cusorg.CompanyName, " & vbCrLf &
                                    " convert(varchar, trans.CheckedInUsedOn, 1) as CheckedInDate," & vbCrLf &
                                    " convert(varchar, trans.CheckedOutReturnedOn, 1) as CheckedOutDate, " & vbCrLf &
                                    " DATEDIFF(day, trans.CheckedInUsedOn, trans.CheckedOutReturnedOn) as NoOfNights, " & vbCrLf &
                                    " (SELECT prdprc.Price FROM TransRentableAssets as transren " & vbCrLf &
                                    "   INNER JOIN LibraryRentableAssets as ren ON transren.AssetID = ren.ID " & vbCrLf &
                                    "   INNER JOIN MobizLibraryPIMProductPriceLevel as prdprc ON ren.ProductID = prdprc.ProductID " & vbCrLf &
                                    "   INNER JOIN MobizLibraryPIMPriceLevel prc ON prdprc.PriceLevelID = prc.ID " & vbCrLf &
                                    "   WHERE prdprc.PriceLevelID = trans.PriceLevelID AND prdprc.ProductID = ren.ProductID AND transren.ID = trans.ID " & vbCrLf &
                                    "   AND prdprc.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & ") as Rate" & vbCrLf

            Dim pFrom As String = " FROM TransRentableAssets as trans " & vbCrLf &
                                    " INNER JOIN LibraryRentableAssets as resass ON resass.ID = trans.AssetID " & vbCrLf &
                                    " INNER JOIN MobizLibraryPIMProduct as pimpro ON pimpro.ID = resass.ProductID " & vbCrLf &
                                    " LEFT JOIN MobizLibraryOrganization as cusorg ON cusorg.ID = trans.OrgID " & vbCrLf

            Dim pWhere As String = " WHERE convert(varchar, CheckedInUsedOn, 107) = convert(varchar, " & ReadyString(salesdate) & ", 107) " & vbCrLf

            pSql = pSelect & pFrom & pWhere

            pReport = New ProReport()
            pReport.AutoFillHeader = False
            pReport.SourceFilename = "AssetMgmtSalesReport"

            Dim pDTSalesReport As DataTable = Common.WebRequest.ProData.Execute(pSql)
            Dim totalDailySales = 0
            pDTSalesReport.Columns.Add("Total")

            For Each dt As Object In pDTSalesReport.Rows
                Dim linetotal = CDec2(dt("NoOfNights")) * CDec2(dt("Rate"))
                dt("Total") = linetotal
                totalDailySales += linetotal
            Next

            newpDt.Columns.Add("DailySalesTotal")
            newpDt.Rows(0).Item("DailySalesTotal") = Format(totalDailySales, "Standard")

            pReport.AddTable("Info", newpDt)
            pReport.AddTable("DailySalesReport", pDTSalesReport)

            Dim chargesSelect As String = pSelect &
                                        " , pimprooc.ProductName " & vbCrLf &
                                        " , (SELECT prdprc.Price FROM MobizLibraryPIMProductPriceLevel as prdprc " & vbCrLf &
                                        "   INNER JOIN MobizLibraryPIMPriceLevel prc ON prdprc.PriceLevelID = prc.ID " & vbCrLf &
                                        "   WHERE prdprc.PriceLevelID = trans.PriceLevelID AND prdprc.ProductID = oc.ProductID " & vbCrLf &
                                        "   AND prdprc.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & ") as ChargeAmount" & vbCrLf

            Dim chargesFrom As String = pFrom &
                                    " INNER JOIN TransRentableAssetOtherCharges as transoc ON transoc.TransRentableAssetID = trans.ID " & vbCrLf &
                                    " INNER JOIN LibraryAssetOtherCharges as oc ON oc.ID = transoc.OtherChargesID " & vbCrLf &
                                    " INNER JOIN MobizLibraryPIMProduct as pimprooc ON pimprooc.ID = oc.ProductID" & vbCrLf

            pWhere = " WHERE convert(varchar, ActualCheckedOutReturnedOn, 107) = convert(varchar, " & ReadyString(salesdate) & ", 107) "

            pSql = chargesSelect & chargesFrom & pWhere

            Dim pDTCharges As DataTable = Common.WebRequest.ProData.Execute(pSql)
            Dim totalCharges = 0

            For Each dt As Object In pDTCharges.Rows
                totalCharges += CDec2(dt("ChargeAmount"))
            Next

            pReport.AddTable("Charges", pDTCharges)
            newpDt.Columns.Add("ChargeTotal")
            newpDt.Rows(0).Item("ChargeTotal") = Format(totalCharges, "Standard")
            newpDt.Columns.Add("CompanyLogo")
            Dim logo As String = Common.ServerPath & "\Content\OrganizationLogo\" & WebRequest.Instance.OrgID & ".png"
            If IO.File.Exists(logo) Then
                newpDt.Rows(0).Item("CompanyLogo") = logo
            Else
                newpDt.Rows(0).Item("CompanyLogo") = ""
            End If

            pSql = " -- asset management sales report payments" & vbCrLf &
                "  SELECT DISTINCT" & vbCrLf &
                "  pimpro.ProductName as RoomNo," & vbCrLf &
                "  (SELECT DISTINCT STUFF((SELECT DISTINCT ', ' +" & vbCrLf &
                "    ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) + ' ' + ISNULL(pubppol.PersonSurname, pricusppol.Surname)" & vbCrLf &
                "    FROM TransRentableAssetCustomer as transcus" & vbCrLf &
                "    LEFT JOIN MobizLibraryOrganization as pubppol ON pubppol.ID = transcus.ClientID" & vbCrLf &
                "    LEFT JOIN LibraryPeople as pricusppol ON pricusppol.ID = transcus.ClientID" & vbCrLf &
                "    WHERE transcus.TransRentableAssetID = trans.ID" & vbCrLf &
                "    FOR XML PATH('')" & vbCrLf &
                "  ), 1, 1,'')) as CustomerName," & vbCrLf &
                "  cusorg.CompanyName, trans.OnlineTravelAgenyName," & vbCrLf &
                "  convert(varchar, trans.CheckedInUsedOn, 1) as CheckedInDate," & vbCrLf &
                "  convert(varchar, ISNULL(trans.ActualCheckedOutReturnedOn, trans.CheckedOutReturnedOn), 1) as CheckedOutDate," & vbCrLf &
                "  DATEDIFF(day, trans.CheckedInUsedOn, ISNULL(trans.ActualCheckedOutReturnedOn, trans.CheckedOutReturnedOn)) as NoOfNights," & vbCrLf &
                "  (SELECT prdprc.Price FROM TransRentableAssets as transren" & vbCrLf &
                "    INNER JOIN LibraryRentableAssets as ren ON transren.AssetID = ren.ID" & vbCrLf &
                "    INNER JOIN MobizLibraryPIMProductPriceLevel as prdprc ON ren.ProductID = prdprc.ProductID" & vbCrLf &
                "    INNER JOIN MobizLibraryPIMPriceLevel prc ON prdprc.PriceLevelID = prc.ID" & vbCrLf &
                "    WHERE prdprc.PriceLevelID = trans.PriceLevelID AND prdprc.ProductID = ren.ProductID AND transren.ID = trans.ID " & vbCrLf &
                "    AND prdprc.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & ") as Rate" & vbCrLf &
                "  , pimprooc.ProductName" & vbCrLf &
                "  ,TransCustomerPayment.CreditCardAmount" & vbCrLf &
                "  ,TransCustomerPayment.DebitCardAmount" & vbCrLf &
                "  ,TransCustomerPayment.CashAmount" & vbCrLf &
                "  ,TransCustomerPayment.OtherTypeAmount " & vbCrLf &
                "  ,REPLACE(CASE WHEN TransCustomerPayment.CashAmount = 0 THEN '' ELSE 'Cash' END + '' + " & vbCrLf &
                "  CASE WHEN TransCustomerPayment.CreditCardAmount = 0 THEN '' ELSE '  Credit Card' END + '' + " & vbCrLf &
                "  CASE WHEN TransCustomerPayment.DebitCardAmount = 0 THEN '' ELSE '  Debit Card' END + '' + " & vbCrLf &
                "  CASE WHEN TransCustomerPayment.OtherTypeAmount = 0 THEN '' ELSE '  Other Payment' END, '  ', ', ') as PaymentType " & vbCrLf &
                "  ,(SELECT SUM(prdprc.Price) FROM MobizLibraryPIMProductPriceLevel as prdprc " & vbCrLf &
                "    INNER JOIN MobizLibraryPIMPriceLevel prc ON prdprc.PriceLevelID = prc.ID" & vbCrLf &
                "   INNER JOIN TransRentableAssetOtherCharges as transoc ON transoc.TransRentableAssetID = trans.ID " & vbCrLf &
                "   INNER JOIN LibraryAssetOtherCharges as oc ON oc.ID = transoc.OtherChargesID " & vbCrLf &
                "   INNER JOIN MobizLibraryPIMProduct as pimprooc ON pimprooc.ID = oc.ProductID" & vbCrLf &
                "    WHERE prdprc.PriceLevelID = trans.PriceLevelID AND prdprc.ProductID = oc.ProductID " & vbCrLf &
                "    AND prdprc.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & ") as TotalCharges" & vbCrLf &
                "  FROM TransRentableAssets as trans" & vbCrLf &
                "  INNER JOIN LibraryRentableAssets as resass ON resass.ID = trans.AssetID" & vbCrLf &
                "  INNER JOIN MobizLibraryPIMProduct as pimpro ON pimpro.ID = resass.ProductID" & vbCrLf &
                "  LEFT JOIN MobizLibraryOrganization as cusorg ON cusorg.ID = trans.OrgID" & vbCrLf &
                "  LEFT JOIN TransRentableAssetOtherCharges as transoc ON transoc.TransRentableAssetID = trans.ID" & vbCrLf &
                "  LEFT JOIN LibraryAssetOtherCharges as oc ON oc.ID = transoc.OtherChargesID" & vbCrLf &
                "  LEFT JOIN MobizLibraryPIMProduct as pimprooc ON pimprooc.ID = oc.ProductID" & vbCrLf &
                "  JOIN LibraryRepeatingTransactions ON LibraryRepeatingTransactions.SourceTransRentableAssetID = trans.ID" & vbCrLf &
                "  JOIN TransInvoice ON TransInvoice.RepeatingTransactionID = LibraryRepeatingTransactions.ID" & vbCrLf &
                "  JOIN TransCustomerPaymentInvoices ON TransCustomerPaymentInvoices.InvoiceID = TransInvoice.ID" & vbCrLf &
                "  JOIN TransCustomerPayment ON TransCustomerPayment.ID = TransCustomerPaymentInvoices.CustomerPaymentID" & vbCrLf

            Dim pSqlPayment As String = pSql & "  WHERE convert(varchar, ActualCheckedOutReturnedOn, 107) = convert(varchar, " & ReadyString(salesdate) & ", 107) "
            pReport.AddTable("PaymentsCheckOut", Common.WebRequest.ProData.Execute(pSqlPayment))

            pSqlPayment = pSql & "  WHERE convert(varchar, ActualCheckedInUsedOn, 107) = convert(varchar, " & ReadyString(salesdate) & ", 107) "
            pReport.AddTable("PaymentsCheckIn", Common.WebRequest.ProData.Execute(pSqlPayment))

            pFileName = Common.ServerPath & "FileTemplates\DailySalesReport.pdf"
            ProReport.DebugXmlFilename = ProWeb.Common.ServerPath + "Reports\Stimulsoft XML Data.xml"

            pReport.ExportXmlFile()
            pReport.ExportFile(ProReport.eExportFormat.Pdf, pFileName)

            Dim file As System.IO.FileInfo

            file = New System.IO.FileInfo(pFilename)

            'Response.Clear()
            'Response.AddHeader("Content-Disposition", "attachment; filename=Sales Report as of " & Format(CDate(salesdate), "MMM-dd-yyyy") & ".pdf")
            'Response.AddHeader("Content-Length", file.Length.ToString())
            'Response.ContentType = "application/octet-stream"
            'Response.WriteFile(file.FullName)
            'Response.SetCookie(New HttpCookie("fileDownload", "true"))
            'Response.End()

            Return JsonDictionary.Serialize("success", "DailySalesReport.pdf")
        End Function

        Function CheckInsMadeToday()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSelect As String = " SELECT " & vbCrLf &
                                    " pimpro.ProductName as RoomNo, " & vbCrLf &
                                    " (SELECT DISTINCT STUFF((SELECT DISTINCT ', ' +  " & vbCrLf &
                                    "   ISNULL(pubppol.PersonFirstName, pricusppol.FirstName) + ' ' + ISNULL(pubppol.PersonSurname, pricusppol.Surname) " & vbCrLf &
                                    "   FROM TransRentableAssetCustomer as transcus " & vbCrLf &
                                    "   LEFT JOIN MobizLibraryOrganization as pubppol ON pubppol.ID = transcus.ClientID " & vbCrLf &
                                    "   LEFT JOIN LibraryPeople as pricusppol ON pricusppol.ID = transcus.ClientID " & vbCrLf &
                                    "   WHERE transcus.TransRentableAssetID = trans.ID " & vbCrLf &
                                    "   FOR XML PATH('') " & vbCrLf &
                                    " ), 1, 1,'')) as CustomerName, " & vbCrLf &
                                    " cusorg.CompanyName, trans.OnlineTravelAgenyName, " & vbCrLf &
                                    " convert(varchar, trans.CheckedInUsedOn, 1) as CheckedInDate," & vbCrLf &
                                    " convert(varchar, trans.CheckedOutReturnedOn, 1) as CheckedOutDate, " & vbCrLf &
                                    " DATEDIFF(day, trans.CheckedInUsedOn, trans.CheckedOutReturnedOn) as NoOfNights, " & vbCrLf &
                                    " (SELECT prdprc.Price FROM TransRentableAssets as transren " & vbCrLf &
                                    "   INNER JOIN LibraryRentableAssets as ren ON transren.AssetID = ren.ID " & vbCrLf &
                                    "   INNER JOIN MobizLibraryPIMProductPriceLevel as prdprc ON ren.ProductID = prdprc.ProductID " & vbCrLf &
                                    "   INNER JOIN MobizLibraryPIMPriceLevel prc ON prdprc.PriceLevelID = prc.ID " & vbCrLf &
                                    "   WHERE prdprc.PriceLevelID = trans.PriceLevelID AND prdprc.ProductID = ren.ProductID AND transren.ID = trans.ID " & vbCrLf &
                                    "   AND prdprc.OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & ") as Rate" & vbCrLf

            Dim pFrom As String = " FROM TransRentableAssets as trans " & vbCrLf &
                                    " INNER JOIN LibraryRentableAssets as resass ON resass.ID = trans.AssetID " & vbCrLf &
                                    " INNER JOIN MobizLibraryPIMProduct as pimpro ON pimpro.ID = resass.ProductID " & vbCrLf &
                                    " LEFT JOIN MobizLibraryOrganization as cusorg ON cusorg.ID = trans.OrgID " & vbCrLf

            Dim pWhere As String = " WHERE convert(varchar, CheckedInUsedOn, 107) = convert(varchar, " & ReadyString(Common.WebRequest.Data("SalesDate")) & ", 107) " & vbCrLf

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSelect & pFrom & pWhere))
        End Function

        Function PreviewReports() As ActionResult
            Dim pFolder As String
            Dim pFilename As String

            pFolder = Common.ServerPath & "FileTemplates\"
            pFilename = Common.WebRequest.QueryString("Filename")
            pFilename = IO.Path.Combine(pFolder, pFilename)
            If IO.File.Exists(pFilename) Then
                Return File(pFilename, "application/pdf")
            Else
                Return Content("File does not exist. ref: vb2JAFJSN82j!")
            End If
        End Function
        Public Function IsValidRentableAsset() As String

            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pError As String
            Dim pProductID = WebRequest.Instance.Data("ProductID")
            Dim pResult = WebRequest.Instance.Business.LibraryStock.IsValidRentableAsset(pProductID, pError)
            If pResult Then
                Return JsonDictionary.Serialize("Results", "ok")
            Else
                Return JsonDictionary.Serialize("Error", pError)
            End If

        End Function
    End Class
End Namespace