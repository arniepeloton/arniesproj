Public Class DeliveryInfo

    Public Property ID As String
    Public Property CustomerID As String
    Public Property PONum As String
    Public Property PODate As String
    Public Property CusRepresentativeID As String
    Public Property CustomerAddressID As String
    Public Property DeliverToID As String
    Public Property DeliveryMethod As String
    Public Property IsDownTown As String
    Public Property IsRushDelivery As String
    Public Property DeliverWithin As String
    Public Property DeliveryType As String
    Public Property PickUpLocationID As String
    Public Property PickupDate As String
    Public Property IsHold As String

    Function getOutOfTownBusDefault() As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pReturn As String

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "sysdef.ID"
                    .Searchable = False
                End With
                With .Add("Description", SqlDbType.VarChar)
                    .Expression = "sysdef.Description"
                End With
                With .Add("DefaultValue", SqlDbType.VarChar)
                    .Expression = "sysdef.DefaultValue"
                End With
            End With

            .SqlFrom = " FROM SystemDefaults sysdef "

            .SqlWhere = " WHERE sysdef.ID = '5CE2C5B3-E483-4C62-A325-FF840F16EA0B'" '''Fixed ID from System Defaults

            pDT = .Requery

        End With

        pReturn = ""
        If (pDT.Rows.Count > 0) Then
            If (IsNumeric(pDT.Rows(0).Item("DefaultValue"))) Then
                pReturn = (Convert.ToInt32(pDT.Rows(0).Item("DefaultValue")) / 60).ToString()
            End If
        End If

        Return pReturn

    End Function

    Function getPickUpCustomerIsWaitingDefault() As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pReturn As String

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "sysdef.ID"
                    .Searchable = False
                End With
                With .Add("Description", SqlDbType.VarChar)
                    .Expression = "sysdef.Description"
                End With
                With .Add("DefaultValue", SqlDbType.VarChar)
                    .Expression = "sysdef.DefaultValue"
                End With
            End With

            .SqlFrom = " FROM SystemDefaults sysdef "

            .SqlWhere = " WHERE sysdef.ID = 'CF898F41-EC86-4CBC-B532-7109A55A9509'" '''Fixed ID from System Defaults

            pDT = .Requery

        End With

        pReturn = ""
        If (pDT.Rows.Count > 0) Then
            pReturn = pDT.Rows(0).Item("DefaultValue").ToString()
        End If

        Return pReturn

    End Function

    Function getInvoiceInfo(ByVal strID As String, vFowstID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "si.ID"
                    .Searchable = False
                End With
                With .Add("CustomerID", SqlDbType.UniqueIdentifier)
                    .Expression = "si.CustomerID"
                End With
                With .Add("CustomerName", SqlDbType.VarChar)
                    .Expression = "cust.CompanyName"
                End With
                With .Add("MobileNo", SqlDbType.VarChar)
                    .Expression = "NULL"
                End With
                With .Add("RepresentativeID", SqlDbType.UniqueIdentifier)
                    .Expression = "si.CusRepresentativeID"
                End With
                With .Add("Representative", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(custrep.Title)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(custrep.LastName)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(custrep.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(custrep.MiddleName)) + ' ',' '), '') "
                End With
                With .Add("BillingAddressID", SqlDbType.UniqueIdentifier)
                    .Expression = "si.BillingAddressID"
                    .Searchable = False
                End With
                With .Add("BillingAddress", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(custbilladd.StreetNumber)), ''), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(custbilladd.Street)),', '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(custbillcty.Name)),', '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(custbillctry.Name)),', '), '')"
                End With
                With .Add("DeliveryAddressID", SqlDbType.UniqueIdentifier)
                    .Expression = "si.DeliveryAddressID"
                    .Searchable = False
                End With
                With .Add("DeliveryAddress", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(custdeladd.StreetNumber)), ''), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(custdeladd.Street)),', '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(custdeladdcty.Name)),', '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(custdeladdctry.Name)),', '), '')"
                End With
                With .Add("PickUpLocationID", SqlDbType.VarChar)
                    .Expression = "ISNULL(si.PickUpLocationID, " & ReadyString(vFowstID) & ")"
                    .Searchable = False
                End With
                With .Add("PickUpLocation", SqlDbType.VarChar)
                    .Expression = "ISNULL(pckup.Name,(SELECT TOP 1 [Name] FROM MobizLibraryOrganizationFOWST WHERE ID = " & ReadyString(vFowstID) & "))"
                    .Searchable = False
                End With
                With .Add("DeliveryMethod", SqlDbType.VarChar)
                    .Expression = "ISNULL(si.DeliveryMethod,'PickUp')"
                End With
                With .Add("IsDownTown", SqlDbType.Bit)
                    .Expression = "si.IsDownTown"
                End With
                With .Add("IsRushDelivery", SqlDbType.Bit)
                    .Expression = "si.IsRushDelivery"
                End With
                With .Add("DeliverWithin", SqlDbType.Int)
                    .Expression = "si.DeliverWithin"
                End With
                With .Add("DeliveryType", SqlDbType.VarChar)
                    .Expression = "si.DeliveryType"
                End With
                With .Add("PickupDateMonth", SqlDbType.Int)
                    .Expression = "MONTH(ISNULL(si.PickupDate,getdate()))"
                End With
                With .Add("PickupDateDay", SqlDbType.Int)
                    .Expression = "DAY(ISNULL(si.PickupDate,getdate()))"
                End With
                With .Add("PickupDateYear", SqlDbType.Int)
                    .Expression = "YEAR(ISNULL(si.PickupDate,getdate()))"
                End With
                With .Add("PickupDateHour", SqlDbType.Int)
                    .Expression = "CASE " &
                                "     WHEN DATEPART(hour,ISNULL(si.PickupDate,getdate())) = 0 THEN 12 " &
                                "     WHEN (DATEPART(hour,ISNULL(si.PickupDate,getdate())) > 0 AND DATEPART(hour,ISNULL(si.PickupDate,getdate())) <= 12) THEN DATEPART(hour,ISNULL(si.PickupDate,getdate())) " &
                                "     WHEN DATEPART(hour,ISNULL(si.PickupDate,getdate())) > 12 THEN DATEPART(hour,ISNULL(si.PickupDate,getdate())) - 12 " &
                                "  END"
                End With
                With .Add("PickupDateMinute", SqlDbType.Int)
                    .Expression = "DATEPART(minute,ISNULL(si.PickupDate,getdate()))"
                End With
                With .Add("PickupDateAMPM", SqlDbType.Int)
                    .Expression = "CASE " &
                                "     WHEN DATEPART(hour,ISNULL(si.PickupDate,getdate())) < 12 THEN 'AM' " &
                                "     WHEN DATEPART(hour,ISNULL(si.PickupDate,getdate())) >=12 THEN 'PM' " &
                                "  END"
                End With
            End With

            .SqlFrom = " FROM TransInvoice si" & vbCrLf & _
                " INNER JOIN MobizLibraryOrganization cust ON (si.CustomerID = cust.ID) " & vbCrLf & _
                " LEFT JOIN MobizLibraryOrganizationRepresentatives custrep ON (si.CusRepresentativeID = custrep.ID)" & vbCrLf & _
                " LEFT JOIN MobizLibraryOrganizationFOWST custbilladd ON (si.BillingAddressID = custbilladd.ID)" & vbCrLf & _
                " LEFT JOIN dbo.MobizLibraryCities custbillcty ON (custbilladd.FowstCityID = custbillcty.ID)" & vbCrLf & _
                " LEFT JOIN dbo.MobizLibraryCountries custbillctry ON (custbilladd.FowstCountryID = custbillctry.ID)" & vbCrLf & _
                " LEFT JOIN MobizLibraryOrganizationFOWST custdeladd ON (si.DeliveryAddressID = custdeladd.ID)" & vbCrLf & _
                " LEFT JOIN dbo.MobizLibraryCities custdeladdcty ON (custdeladd.FowstCityID = custdeladdcty.ID)" & vbCrLf & _
                " LEFT JOIN dbo.MobizLibraryCountries custdeladdctry ON (custdeladd.FowstCountryID = custdeladdctry.ID)" & vbCrLf & _
                " LEFT JOIN MobizLibraryOrganizationFOWST pckup ON (si.PickUpLocationID = pckup.ID)" & vbCrLf

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE si.ID IS NULL", " WHERE si.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveInvoiceDeliveryInfo() As String
        Dim p As New InsertUpdate("TransInvoice")
        Dim pSql As String
        Dim strIDReturn As String

        If (CusRepresentativeID <> Nothing) Then
            p("CusRepresentativeID") = IIf(CusRepresentativeID.Length > 0, ReadyString(CusRepresentativeID), "NULL")
        End If

        p("BillingAddressID") = IIf(CustomerAddressID.Trim().Length > 0, ReadyString(CustomerAddressID), "NULL")
        p("DeliveryAddressID") = IIf(DeliverToID.Trim().Length > 0, ReadyString(DeliverToID), "NULL")

        p("PickUpLocationID") = IIf(PickUpLocationID.Trim().Length > 0, ReadyString(PickUpLocationID), "NULL")
        p("DeliveryMethod") = ReadyString(DeliveryMethod)
        p("IsDownTown") = IsDownTown
        p("IsRushDelivery") = IsRushDelivery
        p("DeliverWithin") = DeliverWithin
        p("DeliveryType") = ReadyString(DeliveryType)
        p("PickupDate") = If(PickupDate.Trim().Length > 0, ReadyString(PickupDate), "NULL")

        pSql = p.GetUpdate(" ID = " & ReadyString(ID))

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return strIDReturn

    End Function

    Function SaveInvoiceToQuotation() As String
        Dim pSql As String
        Dim strIDReturn As String

        strIDReturn = Guid.NewGuid.ToString()

        '''Insert Quotation Header
        pSql = "INSERT INTO TransSalesQuotations (" &
                "ID" &
                ",QuotationDate" &
                ",CustomerID" &
                ",CusRepresentativeID" &
                ",BillingAddressID" &
                ",DeliveryAddressID" &
                ",Remarks" &
                ",CreditLimit" &
                ",FowstID" &
                ",UserID" &
                ",LastUpdated" &
                ",RegisteredDate" &
                ",DeviceID) " &
                "(Select " &
                 ReadyString(strIDReturn) & " as ID," &
                 "getdate() as QuotationDate," &
                 "CustomerID," &
                 "CusRepresentativeID," &
                 "BillingAddressID," &
                 "DeliveryAddressID," &
                 "Remarks," &
                  "0 as CreditLimit," &
                 "FowstID," &
                 "UserID," &
                 "getdate() as LastUpdated," &
                 "getdate() as RegisteredDate," &
                 "DeviceID " &
            " FROM dbo.TransInvoice " &
            " WHERE ID= " & ReadyString(ID) & ")"

        ' '' ''WebRequest.Instance.Business.LibraryCustomers.GetAvailableCreditLimit(CustomerID) & " as CreditLimit," &

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        '''Insert Quotation Items
        pSql = "INSERT INTO TransSalesQuotationsItems (" &
             "SalesQuotationsID" &
             ",ProductID" &
             ",Quantity" &
             ",ActualQuantity" &
             ",UnitMeasure" &
             ",TermDiscount" &
             ",DiscountIfPaidWithIn" &
             ",NetDue" &
             ",UnitPrice" &
             ",SubTotal" &
             ",PriceLevelID" &
             ",Cost) " &
            "(Select " &
                ReadyString(strIDReturn) & " as SalesQuotationsID," &
                "ProductID," &
                "Quantity," &
                "ActualQuantity," &
                "UnitMeasure," &
                "TermDiscount," &
                "DiscountIfPaidWithIn," &
                "NetDue," &
                "UnitPrice," &
                "SubTotal," &
                "PriceLevelID," &
                "Cost " &
            " FROM dbo.TransInvoiceItems " &
            " WHERE InvoiceID= " & ReadyString(ID) & ")"

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        '''Delete Invoice Items
        pSql = "DELETE FROM dbo.TransInvoiceItems " &
            " WHERE InvoiceID = " & ReadyString(ID)

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        Return strIDReturn

    End Function

    Function getHeaderInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("OrganizationID", SqlDbType.UniqueIdentifier)
                    .Expression = "rsetbr.OrganizationID"
                    .Searchable = False
                End With
                With .Add("CorporateName", SqlDbType.VarChar)
                    .Expression = "rsetorg.CompanyName"
                End With
                With .Add("OrganizationBranchID", SqlDbType.UniqueIdentifier)
                    .Expression = "rsetfowst.FowstID"
                    .Searchable = False
                End With
                With .Add("BusinessTradeName", SqlDbType.VarChar)
                    .Expression = "rsettrdname.BusinessTradeName"
                End With
                With .Add("CompleteAddress", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(rsetbr.StreetNumber)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(rsetbr.Street)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(cty.Name)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(ctry.Name)) + ', ',', '), '')"
                End With
                With .Add("TelNo", SqlDbType.VarChar)
                    .Expression = "dbo.GetOrganizationFOWSTTelNos(rsetfowst.FowstID)"
                End With
                With .Add("TIN", SqlDbType.VarChar)
                    .Expression = "rsetorg.TinNumber"
                End With
                With .Add("POSPermitNo", SqlDbType.VarChar)
                    .Expression = "rsetbrposset.POSPermitNo"
                End With
                With .Add("POSAccreditationNo", SqlDbType.VarChar)
                    .Expression = WebRequest.Instance.Business.SystemDefaults.GetSystemDefaultPublic("Mobiz POS Accreditation Number")
                End With
            End With

            .SqlFrom = " FROM dbo.LibraryReceiptSettingsFowst rsetfowst " &
                    " INNER JOIN dbo.LibraryReceiptSettings rset ON (rsetfowst.ReceiptSettingsID = rset.ID) " &
                    " INNER JOIN dbo.MobizLibraryOrganizationFOWST rsetbr ON (rsetfowst.FowstID = rsetbr.ID) " &
                    " INNER JOIN dbo.MobizLibraryOrganization rsetorg ON (rsetbr.OrganizationID = rsetorg.ID) " &
                    " LEFT JOIN dbo.MobizLibraryOrganizationTradeName rsettrdname ON (rset.OrgTradeNameID = rsettrdname.ID) " &
                    " LEFT JOIN dbo.MobizLibraryCities cty ON (rsetbr.FowstCityID = cty.ID) " &
                    " LEFT JOIN dbo.MobizLibraryCountries ctry ON (rsetbr.FowstCountryID = ctry.ID) " &
                    " LEFT JOIN dbo.MobizLibraryOrganizationFOWSTPOSSettings rsetbrposset ON (rsetfowst.FowstID = rsetbrposset.FOWSTID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE rsetfowst.FowstID IS NULL", " WHERE rsetfowst.FowstID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getQuotationHeaderInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "quo.ID"
                    .Searchable = False
                End With
                With .Add("QuotationNo", SqlDbType.Int)
                    .Expression = "quo.QuotationNo"
                End With
                With .Add("QuotationDate", SqlDbType.DateTime)
                    .Expression = "quo.QuotationDate"
                End With
                With .Add("CustomerID", SqlDbType.UniqueIdentifier)
                    .Expression = "quo.CustomerID"
                End With
                With .Add("CustomerName", SqlDbType.VarChar)
                    .Expression = "cust.CompanyName"
                End With
                With .Add("RepresentativeID", SqlDbType.UniqueIdentifier)
                    .Expression = "quo.CusRepresentativeID"
                End With
                With .Add("Representative", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(custrep.Title)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(custrep.LastName)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(custrep.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(custrep.MiddleName)) + ' ',' '), '') "
                End With
                With .Add("CustomerAddress", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(cust.OfficeAddressBuildingNo)) + ' ', ' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(cust.OfficeAddressStreet)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(custcty.Name)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(custctry.Name)) + ', ',', '), '')"
                End With
                With .Add("CustomerTIN", SqlDbType.VarChar)
                    .Expression = "cust.TinNumber"
                End With
                With .Add("BillingAddressID", SqlDbType.UniqueIdentifier)
                    .Expression = "quo.BillingAddressID"
                    .Searchable = False
                End With
                With .Add("BillingAddress", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(custbilladd.StreetNumber)), ''), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(custbilladd.Street)),', '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(custbillcty.Name)),', '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(custbillctry.Name)),', '), '')"
                End With
                With .Add("DeliveryAddressID", SqlDbType.UniqueIdentifier)
                    .Expression = "quo.DeliveryAddressID"
                    .Searchable = False
                End With
                With .Add("DeliveryAddress", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(custdeladd.StreetNumber)), ''), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(custdeladd.Street)),', '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(custdeladdcty.Name)),', '), '') + " &
                                  "ISNULL(NULLIF(', ' + LTRIM(RTRIM(custdeladdctry.Name)),', '), '')"
                End With
                With .Add("CashierName", SqlDbType.VarChar)
                    .Expression = "ISNULL(NULLIF(LTRIM(RTRIM(cshr.SurName)) + ', ',', '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(cshr.FirstName)) + ' ',' '), '') + " &
                                  "ISNULL(NULLIF(LTRIM(RTRIM(cshr.MiddleName)) + ' ',' '), '')"
                End With
            End With

            .SqlFrom = " FROM dbo.TransSalesQuotations quo " &
                    " INNER JOIN dbo.MobizLibraryOrganization cust ON (quo.CustomerID = cust.ID) " &
                    " LEFT JOIN dbo.MobizLibraryCities custcty ON (cust.OfficeAddressCityID = custcty.ID) " &
                    " LEFT JOIN dbo.MobizLibraryCountries custctry ON (cust.OfficeAddressCountryID = custctry.ID) " &
                    " LEFT JOIN dbo.MobizLibraryOrganizationRepresentatives custrep ON (quo.CusRepresentativeID = custrep.ID) " &
                    " LEFT JOIN dbo.MobizLibraryOrganizationFOWST custbilladd ON (quo.BillingAddressID = custbilladd.ID) " &
                    " LEFT JOIN dbo.MobizLibraryCities custbillcty ON (custbilladd.FowstCityID = custbillcty.ID) " &
                    " LEFT JOIN dbo.MobizLibraryCountries custbillctry ON (custbilladd.FowstCountryID = custbillctry.ID) " &
                    " LEFT JOIN dbo.MobizLibraryOrganizationFOWST custdeladd ON (quo.DeliveryAddressID = custdeladd.ID) " &
                    " LEFT JOIN dbo.MobizLibraryCities custdeladdcty ON (custdeladd.FowstCityID = custdeladdcty.ID) " &
                    " LEFT JOIN dbo.MobizLibraryCountries custdeladdctry ON (custdeladd.FowstCountryID = custdeladdctry.ID) " &
                    " LEFT JOIN dbo.MobizLibraryPeople cshr ON (quo.UserID = cshr.ID) "


            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE quo.ID IS NULL", " WHERE quo.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getQuotationBodyInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "quoitem.ID"
                    .Searchable = False
                End With
                With .Add("ProductID", SqlDbType.UniqueIdentifier)
                    .Expression = "quoitem.ProductID"
                    .Searchable = False
                End With
                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "prd.ProductNo"
                End With
                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "prd.ProductName"
                End With
                With .Add("Quantity", SqlDbType.Int)
                    .Expression = "quoitem.Quantity"
                End With
                With .Add("UnitMeasure", SqlDbType.VarChar)
                    .Expression = "quoitem.UnitMeasure"
                End With
                With .Add("UnitPrice", SqlDbType.Money)
                    .Expression = "quoitem.UnitPrice"
                End With
                With .Add("SubTotal", SqlDbType.Money)
                    .Expression = "quoitem.SubTotal"
                End With
            End With

            .SqlFrom = " FROM TransSalesQuotationsItems quoitem " &
                    " INNER JOIN MobizLibraryPIMProduct prd ON (quoitem.ProductID = prd.ID) "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE quoitem.SalesQuotationsID IS NULL", " WHERE quoitem.SalesQuotationsID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getDeliveryInfoFooterAmountsInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("TotalAmountDue", SqlDbType.Money)
                    .Expression = "ISNULL(SUM(quoitem.SubTotal),0)"
                End With
            End With

            .SqlFrom = " FROM TransSalesQuotationsItems quoitem "

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE quoitem.SalesQuotationsID IS NULL", " WHERE quoitem.SalesQuotationsID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function













End Class
