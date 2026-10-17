define(['common',
    'business',
    '/Scripts3/Offline/Web.js?crc=',
    'promise!/Scripts3/Browser/common.js?crc=',
    '/Scripts/sources/SalesPayments/SalesPaymentsExport_offline.js?crc='
], function () {

    Browser.OfflineMode = true;
 
    window.SalesPayment_offline = function () {

        this.getOrgDiscountType = function () {

            pSql = "SELECT ID " +
                ",IFNULL(DiscountType,'') as DiscountType " +
                ",IFNULL(PercentDiscount,0) as PercentDiscount " +
                " FROM private.LibraryDiscountSettings " +
                " ORDER BY DiscountType ASC "

            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            return pDT;

        }

        this.getCustomDiscounts = function () {
            var availableCD = [];

            try {
                var pSql = "SELECT * FROM LibraryCustomDiscounts WHERE IsActive = 1 ORDER BY Name ASC "
                var pDT = Sql.Execute(pSql).Rows;                

                for (var i = 0; i < pDT.length; i++) {
                    if (CBool(pDT[i].IsApplyToAllFowst)) {
                        if (CStr2(pDT[i].AvailabilityDateFrom) != '' && CStr2(pDT[i].AvailabilityDateTo) != '') {
                            if (new Date(Business.ZRead.GetZReadDay()) >= new Date(pDT[i].AvailabilityDateFrom) &&
                                new Date(Business.ZRead.GetZReadDay()) <= new Date(pDT[i].AvailabilityDateTo)) {
                                availableCD.push(pDT[i]);
                            }
                        } else {
                            availableCD.push(pDT[i]);
                        }
                    } else {
                        pSql = "SELECT FowstID FROM LibraryCustomDiscountsFowst WHERE CustomDiscountID = " + ReadyString(pDT[i].ID.toString())
                        var pDTFowst = Sql.Execute(pSql).Rows;

                        for (var f = 0; f < pDTFowst.length; f++) {
                            if (sessionStorage.FowstID.toLowerCase() == pDTFowst[f].FowstID.toLowerCase()) {
                                var isExist = false;

                                $.grep(availableCD, function (e) {
                                    if (e.ID == pDT[i].ID) {
                                        isExist = true;
                                    }
                                })

                                if (!isExist) {
                                    availableCD.push(pDT[i]);
                                }
                            }
                        }
                    }
                }

                return availableCD;
            } catch (e) {
                NewMessageBox('You are using the latest Mobiz app version but your database schema is outdated. We recommend to synchronize database now. Thank you!')
                return availableCD;
            }            
        }

        this.getOrgDiscountPercentRefresh = function (strDiscountID) {

            pSql = "SELECT ID" +
                ",IFNULL(DiscountType,'') as DiscountType " +
                ",IFNULL(PercentDiscount,0) as PercentDiscount " +
                " FROM private.LibraryDiscountSettings " +
                " WHERE ID = " + ReadyString(strDiscountID);

            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            if (pDT.Rows.length == 0 && strDiscountID != '') {
                //Custom Discounts
                pSql = "SELECT ID" +
                ",CASE WHEN DiscountAmount IS NULL THEN 'percentage' ELSE 'fixamount' END as DiscountType " +
                ",DiscountPercentage as PercentDiscount, DiscountAmount " +
                " FROM private.LibraryCustomDiscounts " +
                " WHERE ID = " + ReadyString(strDiscountID);
                pDT = Sql.Execute(pSql);

                if (pDT.Rows.length > 0) {
                    if (pDT.Rows[0].DiscountType == 'fixamount') {
                        var discountedamount = CDec2($('#lblSalesPaySubTotal').text().replace(/,/g, "")) - CDec2(pDT.Rows[0].DiscountAmount)
                        var per = ((discountedamount / CDec2($('#lblSalesPaySubTotal').text().replace(/,/g, ""))) * 100) - 100;
                        pDT.Rows[0].PercentDiscount = Math.abs(per);
                    }
                }                                
            }

            return pDT;

        }

        this.ShowSalesPaymentsInfo = function (vInvoiceID) {
            var queryForTotalModifier = "(SELECT SUM(im.SubTotal) FROM TransInvoiceItemsModifier as im INNER JOIN TransInvoiceItems as ii ON ii.ID = im.InvoiceItemID WHERE ii.InvoiceID = siitems.InvoiceID)";

            pSql = "SELECT siitems.InvoiceID " +
                ",si.CustomerID ,(SELECT COUNT(DISTINCT ProductID) FROM private.TransInvoiceItems WHERE InvoiceID=si.ID) AS SkuCount" +
                ",CAST(SUM(IFNULL(siitems.Quantity,0)) as TEXT) as TotalQuantity " +
                ",CAST((SUM(IFNULL(siitems.VATableSale,SubTotal)) + SUM(IFNULL(siitems.VATExemptSale,0)) + SUM(IFNULL(siitems.ZeroRatedSale,0)) + SUM(IFNULL(siitems.NonVATSale,0))) as TEXT) as TotalAmountDue " +
                ",(SELECT SUM(IFNULL(SubTotal,0)) FROM TransInvoiceItems WHERE InvoiceID = siitems.InvoiceID AND OriginalUnitPrice = UnitPrice) as TotalAmountDueAllowedForDiscount " +
                ",CAST(SUM(IFNULL(siitems.SubTotal,0)) as TEXT) as TransInvoiceItems " +
                ",IFNULL(si.RefNum,'') as RefNum " +
                ",(SELECT IFNULL(TaxPercent, 0) FROM private.TransInvoiceItems WHERE InvoiceID = si.ID LIMIT 1) as TaxPercent " +
                ",si.DepositAmount " +
                " FROM private.TransInvoiceItems siitems " +
                " INNER JOIN private.TransInvoice si ON (siitems.InvoiceID = si.ID) " +
                " WHERE siitems.InvoiceID = '" + vInvoiceID + "'" +
                " GROUP BY siitems.InvoiceID " +
                " ,si.CustomerID " +
                " ,si.RefNum " +
                ",si.DepositAmount ";
            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            return pDT;

        };

        this.getTaxPercent = function (vInvoiceID) {
            pSql = "SELECT IFNULL(TaxPercent, 0) as TaxPercent FROM private.TransInvoiceItems WHERE InvoiceID = " + ReadyString(vInvoiceID) + " LIMIT 1"
            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            return pDT;
        }

        this.ShowDeliveryInfo = function (vInvoiceID, vFowstID) {

            pSql = "SELECT " +
                " si.ID" +
                ",si.CustomerID " +
                ",IFNULL((CASE " +
                "   WHEN cust.IsOrganization = 1 THEN cust.CompanyName " +
                "   WHEN cust.IsOrganization = 0 THEN " +
                "        IFNULL(NULLIF(TRIM(cust.PersonSurname) || ', ',', '), '') || " +
                "        IFNULL(NULLIF(TRIM(cust.PersonFirstName) || ' ',' '), '') || " +
                "        IFNULL(NULLIF(TRIM(cust.PersonMiddleName) || ' ',' '), '') || " +
                "        IFNULL(NULLIF(', ' || TRIM(cust.PersonSuffixName),', '), '') " +
                "END),'') as CustomerName" +
                ",NULL as MobileNo " +
                ",IFNULL(si.CusRepresentativeID,'') as RepresentativeID" +
                ",IFNULL(NULLIF(TRIM(custrep.Title) || ' ', ' '), '') || " +
                        "IFNULL(NULLIF(TRIM(custrep.LastName) || ', ',', '), '') || " +
                        "IFNULL(NULLIF(TRIM(custrep.FirstName) || ' ',' '), '') || " +
                        "IFNULL(NULLIF(TRIM(custrep.MiddleName) || ' ',' '), '') as Representative " +
                ",IFNULL(si.BillingAddressID,'') as BillingAddressID " +
                ",IFNULL(NULLIF(TRIM(custbilladd.StreetNumber), ''), '') || " +
                        "IFNULL(NULLIF(', ' || TRIM(custbilladd.Street),', '), '') || " +
                        "IFNULL(NULLIF(', ' || TRIM(custbillcty.Name),', '), '') || " +
                        "IFNULL(NULLIF(', ' || TRIM(custbillctry.Name),', '), '') as BillingAddress " +
                ",IFNULL(si.DeliveryAddressID,'') as DeliveryAddressID" +
                ",IFNULL(NULLIF(TRIM(custdeladd.StreetNumber), ''), '') || " +
                            "IFNULL(NULLIF(', ' || TRIM(custdeladd.Street),', '), '') || " +
                            "IFNULL(NULLIF(', ' || TRIM(custdeladdcty.Name),', '), '') || " +
                            "IFNULL(NULLIF(', ' || TRIM(custdeladdctry.Name),', '), '') as DeliveryAddress " +
                ",IFNULL(si.PickUpLocationID, '" + vFowstID + "') as PickUpLocationID " +
                ",IFNULL(pckup.Name,(SELECT [Name] FROM LibraryOrganizationFOWST WHERE ID = '" + vFowstID + "' LIMIT 1)) as PickUpLocation " +
                ",IFNULL(si.DeliveryMethod,'PickUp') as DeliveryMethod " +
                ",IFNULL(si.IsDownTown,0) as IsDownTown" +
                ",IFNULL(si.IsRushDelivery,0) as IsRushDelivery " +
                ",IFNULL(si.DeliverWithin,0) as DeliverWithin " +
                ",IFNULL(si.DeliveryType,'') as DeliveryType " +
                ",cast(strftime('%m', IFNULL(si.PickupDate,'now')) as signed) as PickupDateMonth " +
                ",cast(strftime('%d', IFNULL(si.PickupDate,'now')) as signed) as PickupDateDay " +
                ",cast(strftime('%Y', IFNULL(si.PickupDate,'now')) as signed) as PickupDateYear " +
                ",(CASE " +
                "     WHEN cast(strftime('%H', IFNULL(si.PickupDate,'now')) as signed) = 0 THEN 12 " +
                "     WHEN cast(strftime('%H', IFNULL(si.PickupDate,'now'))  as signed) > 0 AND cast(strftime('%H', IFNULL(si.PickupDate,'now'))  as signed) <= 12 THEN cast(strftime('%H', IFNULL(si.PickupDate,'now'))  as signed) " +
                "     WHEN cast(strftime('%H', IFNULL(si.PickupDate,'now'))  as signed) > 12 THEN cast(strftime('%H', IFNULL(si.PickupDate,'now')) as signed) - 12 " +
                "END) as PickupDateHour " +
                ",strftime('%M', IFNULL(si.PickupDate,'now')) as PickupDateMinute " +
                ",(CASE " +
                "     WHEN cast(strftime('%H', IFNULL(si.PickupDate,'now')) as signed) < 12 THEN 'AM' " +
                "     WHEN cast(strftime('%H', IFNULL(si.PickupDate,'now')) as signed) >=12 THEN 'PM' " +
                " END) as PickupDateAMPM " +
                " FROM private.TransInvoice si " +
                " INNER JOIN LibraryOrganization cust ON (si.CustomerID = cust.ID) " +
                " LEFT JOIN LibraryOrganizationRepresentatives custrep ON (si.CusRepresentativeID = custrep.ID) " +
                " LEFT JOIN LibraryOrganizationFOWST custbilladd ON (si.BillingAddressID = custbilladd.ID) " +
                " LEFT JOIN LibraryCities custbillcty ON (custbilladd.FowstCityID = custbillcty.ID) " +
                " LEFT JOIN LibraryCountries custbillctry ON (custbilladd.FowstCountryID = custbillctry.ID) " +
                " LEFT JOIN LibraryOrganizationFOWST custdeladd ON (si.DeliveryAddressID = custdeladd.ID) " +
                " LEFT JOIN LibraryCities custdeladdcty ON (custdeladd.FowstCityID = custdeladdcty.ID) " +
                " LEFT JOIN LibraryCountries custdeladdctry ON (custdeladd.FowstCountryID = custdeladdctry.ID) " +
                " LEFT JOIN LibraryOrganizationFOWST pckup ON (si.PickUpLocationID = pckup.ID) " +
                " WHERE si.ID = '" + vInvoiceID + "'"

            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            return pDT;

        };

        this.getOutOfTownBusDefault = function () {

            pSql = "SELECT " +
                        "sysdef.ID " +
                        ",IFNULL(sysdef.Description,'') as Description " +
                        ",IFNULL(sysdef.DefaultValue,'') as DefaultValue " +
                        " FROM private.SystemDefaults sysdef "
            " WHERE sysdef.ID = '5CE2C5B3-E483-4C62-A325-FF840F16EA0B'"

            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            return pDT;

        };

        this.getPickUpCustomerIsWaitingDefault = function () {

            pSql = "SELECT " +
                        "sysdef.ID " +
                        ",IFNULL(sysdef.Description,'') as Description " +
                        ",IFNULL(sysdef.DefaultValue,'') as DefaultValue " +
                        " FROM private.SystemDefaults sysdef " +
                        " WHERE sysdef.ID = 'CF898F41-EC86-4CBC-B532-7109A55A9509'";

            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            return pDT;

        };

        this.getOrganizationRepresentative = function (vOrganizationID) {

            pSql = "SELECT " +
                        "orgrep.ID " +
                        ",IFNULL(TRIM(orgrep.Title), '') || " +
                        "IFNULL(TRIM(orgrep.LastName), '')  || ', ' || " +
                        "IFNULL(TRIM(orgrep.FirstName),'') || ' ' || " +
                        "IFNULL(TRIM(orgrep.MiddleName), '') || ' ' || " +
                        "IFNULL(TRIM(orgrep.GenerationalSuffix), '') as RepresentativeName " +
                        ",(SELECT GROUP_CONCAT(' +' || ctry.CallingCode || '.' || areacode.CallingCode || '.' || orgreptel.ContactNo || IFNULL(NULLIF(' Ext. ' || orgreptel.ExtensionNo,' Ext. '),''),', ') FROM LibraryOrganizationRepresentativesTel orgreptel INNER JOIN LibraryCountries ctry ON (orgreptel.CountryID = ctry.ID) INNER JOIN LibraryAreaCodes areacode ON (orgreptel.AreaCodeID = areacode.ID) WHERE orgreptel.OrgRepID = orgrep.ID) as TelNo " +
                        ",(SELECT GROUP_CONCAT(' +' || ctry.CallingCode || '.' || telcopref.Prefix || '.' || orgrepmob.ContactNo,', ') FROM LibraryOrganizationRepresentativesMobile orgrepmob INNER JOIN LibraryCountries ctry ON (orgrepmob.CountryID = ctry.ID)  INNER JOIN LibraryTelcoPrefixes telcopref ON (orgrepmob.TelCoPrefixID = telcopref.ID)  WHERE orgrepmob.OrgRepID = orgrep.ID) as MobileNo " +
                        ",(SELECT GROUP_CONCAT(orgrepemail.EmailAddress,', ') FROM LibraryOrganizationRepresentativesEmail orgrepemail WHERE orgrepemail.OrgRepID = orgrep.ID) as Email " +
                        ",orgrep.IsActive as IsActive " +
                        " FROM LibraryOrganizationRepresentatives orgrep " +
                        " INNER JOIN LibraryOrganization org ON (orgrep.OrganizationID = org.ID) " +
                        " WHERE orgrep.OrganizationID = '" + vOrganizationID + "' " +
                        " ORDER BY orgrep.LastName ASC, " +
                        " orgrep.FirstName ASC," +
                        " orgrep.MiddleName ASC " +
                        " LIMIT 1"

            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            return pDT;

        };

        this.getCustomerEmails = function (vOrganizationID) {

            pSql = "SELECT custemail.EmailAddress " +
                    " FROM LibraryOrganizationContactEmail custemail " +
                    " Where custemail.OrganizationID = '" + vOrganizationID + "' " +
                    " UNION " +
                    " Select custaddemail.EmailAddress " +
                    " FROM LibraryOrganizationFOWST custfowst " +
                    " INNER JOIN LibraryOrganizationFOWSTEmail custaddemail ON (custfowst.ID = custaddemail.FOWSTID) " +
                    " Where custfowst.OrganizationID = '" + vOrganizationID + "' " +
                    " UNION " +
                    " Select custrepemail.EmailAddress " +
                    " FROM LibraryOrganizationRepresentatives custrep " +
                    " INNER JOIN LibraryOrganizationRepresentativesEmail custrepemail ON (custrep.ID = custrepemail.OrgRepID)  " +
                    " Where custrep.OrganizationID = '" + vOrganizationID + "' "

            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            return pDT;

        };
        //additional by janine
        this.SaveOrganizationInfo = function (vData) {

            var pIU;
            var pSql;
            var newOrgID = NewID();
            //get initials

            if (vData.CompanyName != null) {
                var arr = vData.CompanyName.split(' ');

                var initials = '';
                for (x = 0; x < arr.length; x++)
                    initials += arr[x].charAt(0);
            }

            //check corporate name unique

            if (CStr2(vData.OrganizationID) == '') {
                if (vData.IsOrganization == 1) {
                    pSql = "SELECT ID" +
                        " FROM LibraryOrganization" +
                        " Where CompanyName = " + ReadyString(vData.CompanyName);
                    var pDTOrg = Sql.Execute(pSql);

                    if (pDTOrg.Rows.length > 0) {
                        return 'Existing';
                    }
                } else {
                    //if person,check if unique

                    if (CStr2(vData.PersonFullName) == "") {
                        pSql = "SELECT ID" +
                        " FROM LibraryOrganization" +
                        " Where PersonSurname = " + ReadyString(vData.PersonSurname) + " AND PersonFirstName = " + ReadyString(vData.PersonFirstName) + " AND PersonMiddleName =" + ReadyString(vData.PersonMiddleName);
                        var pDTPerson = Sql.Execute(pSql);

                        if (pDTPerson.Rows.length > 0) {
                            return 'Existing';
                        }
                    } else {
                        pSql = "SELECT ID" +
                        " FROM LibraryOrganization" +
                        " Where PersonFullname = " + ReadyString(vData.PersonFullName);
                        var pDTPerson = Sql.Execute(pSql);

                        if (pDTPerson.Rows.length > 0) {
                            if (!CBool(vData.AllowDuplicate))
                                return 'Existing';
                        }
                    }
                }

            }

            pIU = new InsertUpdate('LibraryOrganization');

            pIU.Add('IsOrganization', vData.IsOrganization);

            pIU.Add('PersonTitleID', ReadyString(vData.PersonTitleID));

            if (vData.PersonFullName != null)
                pIU.Add('PersonFullname', ReadyString(vData.PersonFullName));
            if (vData.PersonSurname != null)
                pIU.Add('PersonSurname', ReadyString(vData.PersonSurname));
            if (vData.PersonFirstName != null)
                pIU.Add('PersonFirstName', ReadyString(vData.PersonFirstName));
            if (vData.PersonMiddleName != null)
                pIU.Add('PersonMiddleName', ReadyString(vData.PersonMiddleName));
            if (vData.PersonSuffixName != null)
                pIU.Add('PersonSuffixName', ReadyString(vData.PersonSuffixName));

            // additional july 16
            if (CStr2(vData.PersonSeniorCitizensIDNo) != '')
                pIU.Add('PersonSeniorCitizensIDNo', ReadyString(vData.PersonSeniorCitizensIDNo));
            else
                pIU.Add('PersonSeniorCitizensIDNo', 'NULL');

            if (CStr2(vData.PersonPWDIDNo) != '')
                pIU.Add('PersonPWDIDNo', ReadyString(vData.PersonPWDIDNo));
            else
                pIU.Add('PersonPWDIDNo', 'NULL');

            if (CStr2(vData.PersonPassportNo) != '')
                pIU.Add('PersonPassportNo', ReadyString(vData.PersonPassportNo));
            else
                pIU.Add('PersonPassportNo', 'NULL');

            if (CStr2(vData.PersonDriversLicenseNo) != '')
                pIU.Add('PersonDriversLicenseNo', ReadyString(vData.PersonDriversLicenseNo));
            else
                pIU.Add('PersonDriversLicenseNo', 'NULL');

            if (CStr2(vData.PersonSSSNo) != '')
                pIU.Add('PersonSSSNo', ReadyString(vData.PersonSSSNo));
            else
                pIU.Add('PersonSSSNo', 'NULL');

            if (CStr2(vData.PersonGSISIDNo) != '')
                pIU.Add('PersonGSISIDNo', ReadyString(vData.PersonGSISIDNo));
            else
                pIU.Add('PersonGSISIDNo', 'NULL');

            if (CStr2(vData.PersonPRCIDNo) != '')
                pIU.Add('PersonPRCIDNo', ReadyString(vData.PersonPRCIDNo));
            else
                pIU.Add('PersonPRCIDNo', 'NULL');

            if (CStr2(vData.PersonPostalIDNo) != '')
                pIU.Add('PersonPostalIDNo', ReadyString(vData.PersonPostalIDNo));
            else
                pIU.Add('PersonPostalIDNo', 'NULL');

            if (CStr2(vData.PersonVotersIDNo) != '')
                pIU.Add('PersonVotersIDNo', ReadyString(vData.PersonVotersIDNo));
            else
                pIU.Add('PersonVotersIDNo', 'NULL');

            if (CStr2(vData.PersonOtherIDType) != '')
                pIU.Add('PersonOtherIDType', ReadyString(vData.PersonOtherIDType));
            else
                pIU.Add('PersonOtherIDType', 'NULL');

            if (CStr2(vData.PersonOtherIDNo) != '')
                pIU.Add('PersonOtherIDNo', ReadyString(vData.PersonOtherIDNo));
            else
                pIU.Add('PersonOtherIDNo', 'NULL');

            // end of additional july 16

            if (vData.PersonDOB != '') {
                pIU.Add('PersonDateOfBirth', (IsDate(vData.PersonDOB) ? ReadyString(vData.PersonDOB) : ReadyString('1/1/1900')));
            }

            if (vData.CompanyName != null)
                pIU.Add('CompanyName', ReadyString(vData.CompanyName));
            if (vData.CompanyName != null)
                pIU.Add('CompanyNameInitial', ReadyString(initials));
            if (vData.TIN != null)
                pIU.Add('TINNumber', ReadyString(vData.TIN));
            if (vData.CompanyBusinessStyle != null)
                pIU.Add('BusinessIndustryText', ReadyString(vData.CompanyBusinessStyle));
            if (vData.ChildName != null)
                pIU.Add('ChildName', ReadyString(vData.ChildName));
            if (vData.ChildBirthdate != null)
                pIU.Add('ChildBirthdate', ReadyString($.format.date(new Date(vData.ChildBirthdate), 'MMM dd, yyyy')));
            if (vData.ChildAge != null)
                pIU.Add('ChildAge', ReadyString(vData.ChildAge));

            pIU.Add('IsFranchisor', vData.IsFranchisor);

            pIU.Add('OfficeAddressBuildingNo', ReadyString(vData.OfficeAddressBuildingNo));
            pIU.Add('OfficeAddressStreet', ReadyString(vData.OfficeAddressStreet));
            pIU.Add('OfficeAddressCityID', ReadyString(vData.OfficeAddressCityID));
            pIU.Add('OfficeAddressCountryID', ReadyString(vData.OfficeAddressCountryID));

            pIU.Add('BusinessAddressBuildingNo', ReadyString(vData.BusinessAddressBuildingNo));
            pIU.Add('BusinessAddressStreet', ReadyString(vData.BusinessAddressStreet));
            pIU.Add('BusinessAddressCityID', ReadyString(vData.BusinessAddressCityID));
            pIU.Add('BusinessAddressCountryID', ReadyString(vData.BusinessAddressCountryID));

            pIU.Add('LastUpdated', proj.SqliteNow);
            pIU.Add('IsFromDevice', '1'); //flag as added from device
            pIU.Add('FlushToServer', '1'); //need to flush

            if (CStr2(vData.OrganizationID) == '') {                                
                pIU.Add('ID', ReadyString(newOrgID));

                pSql = pIU.GetInsert();

            } else {
                pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.OrganizationID));

            }

            Sql.BeginTransaction();
            Sql.ExecuteSql(pSql);


            if (CStr2(vData.OrganizationID) == '') {
                // add to organization customer
                pIU = new InsertUpdate('private.LibraryOrganizationCustomer');

                pIU.Add('OrganizationID', ReadyString(newOrgID));
                pIU.Add('ActivationDate', proj.SqliteNow);
                pIU.Add('LastUpdated', proj.SqliteNow);
                pIU.Add('DeActivated', '0');
                pIU.Add('DeactivationDate', 'NULL');
                pIU.Add('DeactivationReason', ReadyString(''));
                pIU.Add('Comments', ReadyString(''));
                pIU.Add('IsBan', '0');
                pIU.Add('ID', ReadyString(NewID()));

                pSql = pIU.GetInsert();

                Sql.ExecuteSql(pSql);

                // Save Default Price Level (Wholesale, SRP and Retail Only) 05/22/2013 Temporarily Advice by Sir Robert
                //insert retail
                pIU = new InsertUpdate('private.LibraryOrganizationPriceLevel');
                pIU.Add('OrganizationID', ReadyString(newOrgID));
                pIU.Add('PriceLevelID', ReadyString('01000012-0005-0000-0000-000000000000'));
                pIU.Add('UserID', ReadyString(Web.PeopleUserID()));
                pIU.Add('LastUpdated', proj.SqliteNow);
                pIU.Add('ID', ReadyString(NewID()));
                pSql = pIU.GetInsert();
                Sql.ExecuteSql(pSql);

                // insert srp
                pIU = new InsertUpdate('private.LibraryOrganizationPriceLevel');
                pIU.Add('OrganizationID', ReadyString(newOrgID));
                pIU.Add('PriceLevelID', ReadyString('EEF716C6-B290-4B28-B2A6-0A4F52CF4989'));
                pIU.Add('UserID', ReadyString(Web.PeopleUserID()));
                pIU.Add('LastUpdated', proj.SqliteNow);
                pIU.Add('ID', ReadyString(NewID()));
                pSql = pIU.GetInsert();
                Sql.ExecuteSql(pSql);

                //insert wholesale
                pIU = new InsertUpdate('private.LibraryOrganizationPriceLevel');
                pIU.Add('OrganizationID', ReadyString(newOrgID));
                pIU.Add('PriceLevelID', ReadyString('01000012-0004-0000-0000-000000000000'));
                pIU.Add('UserID', ReadyString(Web.PeopleUserID()));
                pIU.Add('LastUpdated', proj.SqliteNow);
                pIU.Add('ID', ReadyString(NewID()));
                pSql = pIU.GetInsert();
                Sql.ExecuteSql(pSql);

            }


            Sql.CommitTransaction();


            if (CStr2(vData.OrganizationID) == '')
                return newOrgID;
            else
                return vData.OrganizationID;


        };

        this.SaveOrganizationFOWST = function (vData) {

            var pIU;
            var pSql;

            pIU = new InsertUpdate('LibraryOrganizationFOWST');

            pIU.Add('OrganizationID', ReadyString(vData.OrganizationID));
            pIU.Add('StreetNumber', ReadyString(vData.StreetNumber));
            pIU.Add('Street', ReadyString(vData.Street));
            pIU.Add('FowstCityID', ReadyString(vData.FowstCityID));
            pIU.Add('FowstCountryID', ReadyString(vData.FowstCountryID));
            pIU.Add('LastUpdated', proj.SqliteNow);
            pIU.Add('ID', ReadyString(NewID()));
            pIU.Add('IsFromDevice', '1'); //flag as added from device
            pIU.Add('FlushToServer', '1'); //need to flush
            pSql = pIU.GetInsert();
            Sql.ExecuteSql(pSql);

            return true;

        }; //end of SaveOrganizationFOWST

        //end of additional
        ///////////////////////////////////////////////////////////////////////////

        this.SaveCustomerDemographics = function (vData) {

            var pIU;
            var pSql;

            pIU = new InsertUpdate('private.TransInvoice');

            pIU.Add('DeliveryMethod', ReadyString(vData.DeliveryMethod));
            pIU.Add('OrderMethod', ReadyString(vData.OrderMethod));
            pIU.Add('CustomerDemographicGender', ReadyString(vData.Gender));
            pIU.Add('CustomerDemographicAge', ReadyString(vData.Age));

            pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));

            Sql.ExecuteSql(pSql);

            return true;

        };

        this.SaveDeliveryInfo = function (vData) {

            var pIU;
            var pSql;

            pIU = new InsertUpdate('private.TransInvoice');

            if (vData.CusRepresentativeID != "") {
                pIU.Add('CusRepresentativeID', (vData.CusRepresentativeID != "" ? ReadyString(vData.CusRepresentativeID) : "NULL"));
            }

            pIU.Add('BillingAddressID', (vData.BillToID != "" ? ReadyString(vData.BillToID) : "NULL"));
            pIU.Add('DeliveryAddressID', (vData.ShipToID != "" ? ReadyString(vData.ShipToID) : "NULL"));

            pIU.Add('PickUpLocationID', (vData.PickUpLocationID != "" ? ReadyString(vData.PickUpLocationID) : "NULL"));
            pIU.Add('DeliveryMethod', ReadyString(vData.DeliveryMethod));
            pIU.Add('IsDownTown', CDec2(vData.IsDownTown));
            pIU.Add('IsRushDelivery', CDec2(vData.IsRushDelivery));
            pIU.Add('DeliverWithin', CDec2(vData.DeliverWithin));
            pIU.Add('DeliveryType', ReadyString(vData.DeliveryType));
            pIU.Add('PickupDate', ReadyDate(vData.PickupDate));
            pIU.Add('IsEmailReceipt', CInt2(vData.IsEmailReceipt));

            if (vData.IsNoneVATSale != undefined) {
                pIU.Add('IsNoneVATSale', CInt2(vData.IsNoneVATSale));
            }

            pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));

            Sql.ExecuteSql(pSql);

            return true;

        };

        this.PreConsumeSrv = function (vDataSRV, vSuccess) {

            if ((vDataSRV == undefined)
                    || (vDataSRV.length == 0)) {

                vSuccess();
                return;
            }

            var pThis = this;
            //additional by janine feb 11 2015
            //assign guid here
            if (!IsGuid(proj.CustomerPaymentID))
                proj.CustomerPaymentID = NewID();

            $('#pageSalesPayment [name=popSrvConsume]').popup().trigger('create').show().popup('open');
            if (proj.SalesReturnFromOffline) {
                ConsumeSrvOffline();
            } else 
                AjaxSend({
                    url: '/SalesPayments/PreConsumeSrv',
                    data: {
                        SrvList: vDataSRV,
                        CustomerPaymentID: proj.CustomerPaymentID
                    },
                    timeout: 10000,
                    success: function (vData) {
                        $('#pageSalesPayment [name=popSrvConsume]').popup('close');
                        if (vData.Result == 'ok')
                            vSuccess({
                                CustomerPaymentID: proj.CustomerPaymentID
                            });
                        else if (vData.Error != undefined)
                            MessageBox(vData.Error);
                    },
                    error: function(vData) {
                        $('#pageSalesPayment [name=popSrvConsume]').popup('close');
                        Bs.Loading('hide');
                        // NewMessageBox('Unable to send SRV Data to Server. Please verify your network connection then try again. SRV payments require network connection to the server. If network connection to the server is not possible, you want to use a payment type that does not require network connection.', 'Connection Problem', 
                        NewMessageBox('Cannot process SRV payment due to internet connection problem.<br /><br />' + 
                            'We cannot complete this transaction because there is at least one (1) SRV payment. The SRV data has to be sent to Mobiz cloud before completing this transaction to prevent fraud.<br /><br />' +
                            'Please try the following:<br /><br />' + 
                            '1.) Please check and resolve your internet connection on this device then click on the button labeled “Print Receipt” or “Save Receipt” to retry completing this transaction.<br />' +
                            '2.) If internet connection cannot be restored, you can:<br />' +
                            'a.	Input a payment type that does not require an internet connection E.g. cash, cheque, credit/debit card.<br />' +
                            'b.	“Hold” this current transaction and then “Restore” it at a later time when there is an internet connection.' , 'Connection Problem', 
                            [
                                {
                                    Text: 'Try again later'
                                },
                                {
                                    Text: 'Retry to connect online',
                                    Click: function() {
                                        Bs.Loading('show', 'Saving... please wait');
                                        Android.SetAutoOfflineMode(false);
                                        setTimeout(function() {
                                            pThis.PreConsumeSrv(vDataSRV, vSuccess);
                                        }, 3000);
                                    }
                                }
                            ]
                        );
                    }
                }); // END AjaxSend({

            function ConsumeSrvOffline() {
                Sql.BeginTransaction();
                for (var pIndex = 0; pIndex <= vDataSRV.length - 1; pIndex++) {
                    var pSrv = vDataSRV[pIndex];
                    Business.TransSalesReturn.Consume(pSrv.MemoID, proj.CustomerPaymentID, pSrv.ConsumedAmount);
                }
                Sql.CommitTransaction();
                vSuccess({
                    CustomerPaymentID: proj.CustomerPaymentID
                });
            } // END function ConsumeSrvOffline() {
        },

            this.SaveSalesPayments = function (vData, rError) {

                var pIU;
                var pSql;
                var strUserID = "";
                var pCustPayID = "";
                var pDtInvoice;
                var pInvoiceVat = 0;
                var pCreditCardVat = 0;

                //Check if Invoice already saved
                pSql = "SELECT custpayinv.ID " +
                    " ,custpayinv.InvoiceID " +
                    " ,inv.InvoiceNumber " +
                    " FROM private.TransCustomerPaymentInvoices custpayinv " +
                    " INNER JOIN private.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) " +
                    " Where custpayinv.InvoiceID = '" + vData.InvoiceID + "'";
                var pDT = Sql.Execute(pSql);
                if (pDT.Rows.length > 0) {
                    return true;
                }

                if (!CBool(vData.DontVerify)) {
                    //Check if Total Amount Due has been change
                    var queryForTotalModifier = "(SELECT SUM(im.SubTotal) FROM TransInvoiceItemsModifier as im INNER JOIN TransInvoiceItems as ii ON ii.ID = im.InvoiceItemID WHERE ii.InvoiceID = siitems.InvoiceID)";

                    pSql = "SELECT CAST(SUM(IFNULL(siitems.SubTotal,0)) as TEXT) as TotalAmountDue " +
                    " FROM private.TransInvoiceItems siitems " +
                    " WHERE siitems.InvoiceID = '" + vData.InvoiceID + "' ";
                    var pDTAmountDue = Sql.Execute(pSql);

                    if (pDTAmountDue.Rows.length > 0) {
                    
                        var pSumOfItemsAmount = CDec2(Math.round(pDTAmountDue.Rows[0].TotalAmountDue / 0.05) * 0.05).toFixed(2);
                        var pInvoiceTotalAmount = CDec2(Math.round(CDec2(vData.TotalAmountDue) / 0.05) * 0.05).toFixed(2);
                        var pMustVerify;

                        if (vData.SalesPaymentDiscountList.length > 0) {
                            if (CStr2(vData.SalesPaymentDiscountList[0].ID).toUpperCase() != '055A0A33-DD98-47EC-ADD3-8B2009F3944A' && //SSD
                                CStr2(vData.SalesPaymentDiscountList[0].ID).toUpperCase() != '674AD791-188B-4238-B95C-97B5C2B9D544' && //SSD
                                CStr2(vData.SalesPaymentDiscountList[0].ID).toUpperCase() != '34A4032F-A142-4C15-A868-4AC4410434DF') { //PWD
                                pMustVerify = true;
                            
                            }
                        } else 
                            pMustVerify = true;

                        if (pMustVerify)
                            if (pSumOfItemsAmount != pInvoiceTotalAmount) {
                                if (rError)
                                    rError.Message = 'This invoice\'s total is ' +pInvoiceTotalAmount + ' while the grand total of all the line item\'s sub totals is ' + pSumOfItemsAmount + '.';
                                return false;
                            }
                    }
                } // END if (!CBool(vData.DontVerify)) {

                strUserID = Web.PeopleUserID();

                //Get Invoice Details
                pDtInvoice = Business.TransInvoice.GetByID(vData.InvoiceID);

                //Save Customer Payment
                pIU = new InsertUpdate('private.TransCustomerPayment');
                pIU.Add('ReceiptType', ReadyString("OR"));
                pIU.Add('ReceiptNumber', "(SELECT IFNULL(devfowst.POSLastORNo,0) + 1 FROM private.LibraryDeviceFowst devfowst WHERE devfowst.FowstID = " + ReadyString(sessionStorage.FowstID) + " AND devfowst.DeviceID = " + ReadyString(Web.DeviceID()) + ")");
                pIU.Add('CustomerID', (vData.CustomerID != "" ? ReadyString(vData.CustomerID) : "NULL"));

                if ((pDtInvoice == undefined)
                    || (CStr2(pDtInvoice.Rows[0].RefNum).trim() == '')) {

                    pIU.Add('PaymentDate', proj.GetZReadDateSql());

                } else {
                    pIU.Add('PaymentDate', ReadyDate(ParseDate(pDtInvoice.Rows[0].InvoiceDate))); // jsD6DC5QD48Y
                }

                if ((pDtInvoice != null)
                    && (pDtInvoice.Rows.length > 0)) 
                    pInvoiceVat = CDec2(pDtInvoice.Rows[0].VAT);
                
                var pTotalPayment = CDec2(vData.CashAmount)
                    + CDec2(vData.DebitCardAmount)
                    + CDec2(vData.CreditCardAmount)
                    + CDec2(vData.CheckPaymentAmount)
                    + CDec2(vData.SRVAmount)
                    + CDec2(vData.OtherTypeAmount);

                if (pTotalPayment > 0) {
                    pCreditCardVat = pInvoiceVat * CDec2(vData.CreditCardAmount) / pTotalPayment; // robinson: credit card vat invoice VAT proportional to the credt card amount
                }
                pIU.Add('CashAmount', CDec2(vData.CashAmount));
                pIU.Add('DebitCardAmount', CDec2(vData.DebitCardAmount));
                pIU.Add('CreditCardAmount', CDec2(vData.CreditCardAmount));
                pIU.Add('CreditCardVat', pCreditCardVat);
                pIU.Add('CheckPaymentAmount', CDec2(vData.CheckPaymentAmount));
                pIU.Add('SRVAmount', CDec2(vData.SRVAmount));
                pIU.Add('OtherTypeAmount', CDec2(vData.OtherTypeAmount));
                pIU.Add('OnCreditAmount', CDec2(vData.OnCreditAmount));
                pIU.Add('ChargeAmount', CDec2(vData.ChargeAmount));
                pIU.Add('DiscountAmount', CDec2(vData.TotalDiscountAmount));
                pIU.Add('TotalAmountDue', CDec2(vData.TotalAmountDue));
                pIU.Add('AmountChange', "0");
                pIU.Add('CustomerChangeCash', CDec2(vData.CustomerChangeCash));
                pIU.Add('UserID', ReadyString(strUserID));
                pIU.Add('OnCreditAmount', CDec2(vData.OnCreditAmount));
                pIU.Add('OnCreditRemarks', ReadyString(vData.OnCreditRemarks));
                pIU.Add('RewardsCardNum', ReadyString(vData.RewardsCardNum));
                pIU.Add('CustomerEmailAddresses', ReadyString(vData.EmailAddress));
                
                if (IsGuid(vData.CustomerPaymentID))
                    pCustPayID = vData.CustomerPaymentID;
                else
                    pCustPayID = NewID();
                pIU.Add('ID', ReadyString(pCustPayID));
                pSql = pIU.GetInsert();

                Sql.BeginTransaction();
                Sql.ExecuteSql(pSql);

                //Update Invoice 
                pIU = new InsertUpdate('private.TransInvoice');
                var pInvoiceNumber = Business.TransInvoice.GetCounter() + 1;
                // pIU.Add('InvoiceNumber', "(SELECT IFNULL(devfowst.POSLastORNo,0) + 1 FROM private.LibraryDeviceFowst devfowst WHERE devfowst.FowstID = " + ReadyString(sessionStorage.FowstID) + " AND devfowst.DeviceID = " + ReadyString(Web.DeviceID()) + ")");
                pIU.Add('InvoiceNumber', pInvoiceNumber);
                var pDeviceFowstTransactionNumber = Business.TransInvoice.GetCounter({
                    FieldName: 'DeviceFowstTransactionNumber',
                    DontUseTableCounter: true
                }) + 1;
                pIU.Add('DeviceFowstTransactionNumber', pDeviceFowstTransactionNumber);
                pIU.Add('CustomerID', (vData.CustomerID != "" ? ReadyString(vData.CustomerID) : "NULL")); // added by arnie 51017 225pm (dapat iupdate pud ang customerid)
                pIU.Add('FowstID', (sessionStorage.FowstID != "" ? ReadyString(sessionStorage.FowstID) : "NULL"));
                pIU.Add('DeviceID', (Web.DeviceID() != "" ? ReadyString(Web.DeviceID()) : "NULL"));
                pIU.Add('IsForGovReport', CDec2(vData.IsForGovReport));
                pIU.Add('DiscountAmount', CDec2(vData.TotalDiscountAmount).toFixed(2));
                pIU.Add('IsDraft', 0);

                //TODO commented by janine dec 18
                //                if ((pDtInvoice == undefined)
                //                    || (CStr2(pDtInvoice.Rows[0].RefNum).trim() == ''))
                pIU.Add('InvoiceDate', proj.GetZReadDateSql());
                pIU.Add('LastUpdated', proj.SqliteNow); // to trigger data transmission 2017_03_31__10_00_38_8741

                pIU.Add('UserID', ReadyString(strUserID));
                pIU.Add('DateImported', 'null');

                pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.InvoiceID));

                Sql.ExecuteSql(pSql);

                //Save Customer Payment Invoices
                pIU = new InsertUpdate('private.TransCustomerPaymentInvoices');
                pIU.Add('CustomerPaymentID', ReadyString(pCustPayID));
                pIU.Add('InvoiceID', ReadyString(vData.InvoiceID));
                pIU.Add('InvoiceAmount', CDec2(vData.TotalAmountDue).toFixed(2));
                pIU.Add('DiscountAmount', CDec2(vData.TotalDiscountAmount).toFixed(2));
                pIU.Add('Balance', CDec2(vData.TotalAmountDue).toFixed(2));
                pIU.Add('AmountPaid', (parseFloat(CDec2(vData.CashAmount)) + parseFloat(CDec2(vData.DebitCardAmount)) + parseFloat(CDec2(vData.CreditCardAmount)) + parseFloat(CDec2(vData.CheckPaymentAmount)) + parseFloat(CDec2(vData.SRVAmount)) - parseFloat(CDec2(vData.CustomerChangeCash))).toFixed(2));

                pIU.Add('ID', ReadyString(NewID()));
                pSql = pIU.GetInsert();

                Sql.ExecuteSql(pSql);
                proj.CustomerPaymentID = null;

                //Save Check Payments
                var vCheck = vData.SalesPaymentCheckList;
                for (var i = 0; i < vCheck.length; i++) {

                    if (vCheck[i].RecState != "Remove") {
                        pIU = new InsertUpdate('private.TransCustomerPaymentChecks');
                        pIU.Add('CustomerPaymentID', ReadyString(pCustPayID));
                        pIU.Add('BankID', (vCheck[i].BankID != "" ? ReadyString(vCheck[i].BankID) : "NULL"));
                        pIU.Add('BankName', ReadyString(vCheck[i].BankName));
                        pIU.Add('BankBranchID', (vCheck[i].BankBranchID != "" ? ReadyString(vCheck[i].BankBranchID) : "NULL"));
                        pIU.Add('BankBranchName', ReadyString(vCheck[i].BankBranchName));
                        pIU.Add('AccountName', ReadyString(vCheck[i].AccountName));
                        pIU.Add('AccountNumber', ReadyString(vCheck[i].AccountNumber));
                        pIU.Add('CheckNumber', ReadyString(vCheck[i].CheckNumber));
                        pIU.Add('CheckDate', ReadyString(vCheck[i].CheckDate));
                        pIU.Add('CheckAmount', CDec2(vCheck[i].CheckAmount).toFixed(2));
                        pIU.Add('IsBankFromOrg', 1);
                        pIU.Add('IsGiftCertificate', CStr2(vCheck[i].CheckType) == 'gc' ? 1 : 0);

                        if (vCheck[i].ID == "") {
                            pIU.Add('ID', ReadyString(NewID()));
                            pSql = pIU.GetInsert();
                        } else {
                            pSql = pIU.GetUpdate('ID = ' + ReadyString(vCheck[i].ID));
                        }

                    } else {
                        pSql = " DELETE FROM private.TransCustomerPaymentChecks WHERE ID = " + (vCheck[i].ID == "" ? "NULL" : ReadyString(vCheck[i].ID));
                    }

                    Sql.ExecuteSql(pSql);
                }

                // march 13, 2014: dapat daw, maka credit card, debit card miskan offline
                //Save DEBIT Card Payments
                var vDebit = vData.SalesPaymentDebitCardList;
                for (var i = 0; i < vDebit.length; i++) {

                    if (vDebit[i].RecState != "Remove") {
                        pIU = new InsertUpdate('TransCustomerPaymentDebitCard');
                        pIU.Add('CustomerPaymentID', ReadyString(pCustPayID));
                        pIU.Add('DebitCardType', ReadyString(vDebit[i].DebitCardType));
                        pIU.Add('DebitCardBankID', ReadyString(vDebit[i].DebitCardBankID));
                        pIU.Add('DebitCardBankName', ReadyString(vDebit[i].DebitCardBank));
                        pIU.Add('BankCountryID', ReadyString(vDebit[i].BankCountryID));
                        pIU.Add('HolderLastName', ReadyString(vDebit[i].HolderLastName));
                        pIU.Add('HolderFirstName', ReadyString(vDebit[i].HolderFirstName));
                        pIU.Add('HolderMiddleName', ReadyString(vDebit[i].HolderMiddleName));
                        pIU.Add('HolderGenerationalSuffix', ReadyString(vDebit[i].HolderGenerationalSuffix));
                        pIU.Add('DebitCardNumber', ReadyString(vDebit[i].DebitCardNumber));
                        if (vDebit[i].DebitCardIssuanceDate != '') {
                            pIU.Add('DebitCardIssuanceDate', ReadyString(vDebit[i].DebitCardIssuanceDate));
                        }
                        if (vDebit[i].DebitCardExpirationDate != '') {
                            pIU.Add('DebitCardExpirationDate', ReadyString(vDebit[i].DebitCardExpirationDate));
                        }

                        pIU.Add('ApprovalNo', ReadyString(vDebit[i].ApprovalNo));
                        pIU.Add('NoOfInstallmentPayments', CDec2(vDebit[i].NoOfInstallmentPayments));
                        pIU.Add('DebitCardAmount', CDec2(vDebit[i].DebitCardAmount).toFixed(2));
                        pIU.Add('IsBankFromOrg', 1);

                        if (vDebit[i].ID == "") {
                            pIU.Add('ID', ReadyString(NewID()));
                            pSql = pIU.GetInsert();
                        } else {
                            pSql = pIU.GetUpdate('ID = ' + ReadyString(vDebit[i].ID));
                        }

                    } else {
                        pSql = " DELETE FROM TransCustomerPaymentDebitCard WHERE ID = " + (vDebit[i].ID == "" ? "NULL" : ReadyString(vDebit[i].ID));
                    }

                    Sql.ExecuteSql(pSql);
                }

                //Save Credit Card Payments
                var vCredit = vData.SalesPaymentCreditCardList;
                for (var i = 0; i < vCredit.length; i++) {

                    if (vCredit[i].RecState != "Remove") {
                        pIU = new InsertUpdate('TransCustomerPaymentCreditCard');
                        pIU.Add('CustomerPaymentID', ReadyString(pCustPayID));
                        pIU.Add('CreditCardType', ReadyString(vCredit[i].CreditCardType));
                        pIU.Add('CreditCardBankID', ReadyString(vCredit[i].CreditCardBankID));
                        pIU.Add('CreditCardBankName', ReadyString(vCredit[i].CreditCardBank));
                        pIU.Add('BankCountryID', ReadyString(vCredit[i].BankCountryID));
                        pIU.Add('HolderLastName', ReadyString(vCredit[i].HolderLastName));
                        pIU.Add('HolderFirstName', ReadyString(vCredit[i].HolderFirstName));
                        pIU.Add('HolderMiddleName', ReadyString(vCredit[i].HolderMiddleName));
                        pIU.Add('HolderGenerationalSuffix', ReadyString(vCredit[i].HolderGenerationalSuffix));
                        pIU.Add('CreditCardNumber', ReadyString(vCredit[i].CreditCardNumber));

                        if (vCredit[i].CreditCardIssuanceDate != '') {
                            pIU.Add('CreditCardIssuanceDate', ReadyString(vCredit[i].CreditCardIssuanceDate));
                        }
                        if (vCredit[i].CreditCardExpirationDate != '') {
                            pIU.Add('CreditCardExpirationDate', ReadyString(vCredit[i].CreditCardExpirationDate));
                        }
                        pIU.Add('ApprovalNo', ReadyString(vCredit[i].ApprovalNo));
                        pIU.Add('NoOfInstallmentPayments', CDec2(vCredit[i].NoOfInstallmentPayments));
                        pIU.Add('CreditCardAmount', CDec2(vCredit[i].CreditCardAmount).toFixed(2));
                        pIU.Add('IsBankFromOrg', 1);

                        if (vCredit[i].ID == "") {
                            pIU.Add('ID', ReadyString(NewID()));
                            pSql = pIU.GetInsert();
                        } else {
                            pSql = pIU.GetUpdate('ID = ' + ReadyString(vCredit[i].ID));
                        }

                    } else {
                        pSql = " DELETE FROM TransCustomerPaymentCreditCard WHERE ID = " + (vCredit[i].ID == "" ? "NULL" : ReadyString(vCredit[i].ID));
                    }


                    Sql.ExecuteSql(pSql);
                }

                //Save SRV Payments
                var vSRV = vData.SalesPaymentSRVList;
                for (var i = 0; i < vSRV.length; i++) {

                    if (vSRV[i].RecState != "Remove") {
                        pIU = new InsertUpdate('TransCustomerPaymentSRV');
                        pIU.Add('CustomerPaymentID', ReadyString(pCustPayID));
                        pIU.Add('MemoID', ReadyString(vSRV[i].MemoID));
                        pIU.Add('ConsumedAmount', CDec2(vSRV[i].ConsumedAmount).toFixed(2));

                        if (vSRV[i].ID == "") {
                            pIU.Add('ID', ReadyString(NewID()));
                            pSql = pIU.GetInsert();
                        } else {
                            pSql = pIU.GetUpdate('ID = ' + ReadyString(vSRV[i].ID));
                        }

                    } else {
                        pSql = " DELETE FROM TransCustomerPaymentSRV WHERE ID = " + (vSRV[i].ID == "" ? "NULL" : ReadyString(vSRV[i].ID));
                    }

                    Sql.ExecuteSql(pSql);
                }

                //Save Other Payment Types

            pSql = Sql.Execute('PRAGMA table_info(transcustomerpaymentother)').Rows;

            for (var i in pSql) {
                var colname = pSql[i].name;
                if (colname == 'CardNumber' || colname == 'GCashRefNo') {                    
                    if (pSql[i].type.toLowerCase() != 'text') {
                        Sql.Execute('ALTER TABLE transcustomerpaymentother RENAME COLUMN ' + colname + ' to ' + colname + '_old');
                        Sql.Execute('ALTER TABLE transcustomerpaymentother ADD COLUMN ' + colname + ' TEXT ');
                        Sql.Execute('UPDATE transcustomerpaymentother SET ' + colname + ' = CAST(' + colname + '_old as TEXT)');
                    }
                }
            }

                var vOtherType = vData.SalesPaymentOtherTypeList;
                for (var i = 0; i < vOtherType.length; i++) {

                    if (vOtherType[i].RecState != "Remove") {
                        pIU = new InsertUpdate('TransCustomerPaymentOther');
                        pIU.Add('CustomerPaymentID', ReadyString(pCustPayID));
                        pIU.Add('PaymentType', ReadyString(vOtherType[i].PaymentType));
                        pIU.Add('Amount', ReadyString(CDec2(vOtherType[i].Amount).toFixed(2)));
                        pIU.Add('IssuingOrganization', ReadyString(vOtherType[i].IssuingOrganization));
                        pIU.Add('CardNumber', 'CAST(' + ReadyString(vOtherType[i].CardNo) + ' as TEXT)');
                        pIU.Add('CardHolderName', ReadyString(vOtherType[i].CardHolder));
                        pIU.Add('ApprovalNo', ReadyString(vOtherType[i].ApprovalNo));
                        pIU.Add('RoomNo', ReadyString(vOtherType[i].RoomNo))
                        pIU.Add('Remarks', ReadyString(vOtherType[i].Remarks))
                        pIU.Add('GCashRefNo', 'CAST(' + ReadyString(vOtherType[i].GCashRefNo) + ' as TEXT)')

                        if (vOtherType[i].IssuanceDate != '') {
                            pIU.Add('CardIssuanceDate', ReadyString(vOtherType[i].IssuanceDate));
                        }
                        if (vOtherType[i].ExpirationDate != '') {
                            pIU.Add('CardExpirationDate', ReadyString(vOtherType[i].ExpirationDate));
                        }

                        if (vOtherType[i].ID == "") {
                            pIU.Add('ID', ReadyString(NewID()));
                            pSql = pIU.GetInsert();
                        } else {
                            pSql = pIU.GetUpdate('ID = ' + ReadyString(vOtherType[i].ID));
                        }

                        if (vOtherType[i].PaymentType == "GrabFood" || vOtherType[i].PaymentType == "FoodPanda") {
                            //update delivery type
                            var pIU;
                            var pSql2

                            pIU = new InsertUpdate('private.TransInvoice');

                            pIU.Add('DeliveryMethod', ReadyString("Deliver (Downtown)"));

                            pSql2 = pIU.GetUpdate('ID = ' + ReadyString(vData.InvoiceID));

                            Sql.ExecuteSql(pSql2);

                        }

                        if (vOtherType[i].PaymentType == "SupercardVoucher") {
                            var pIU;
                            var pSql2

                            pIU = new InsertUpdate('LibraryVouchers');

                            pIU.Add('IsActive', 0);
                            pIU.Add('FowstID', ReadyString(sessionStorage.FowstID));
                            pIU.Add('CashierID', ReadyString(sessionStorage.PeopleID));
                            pIU.Add('CashierName', ReadyString(sessionStorage.CurrentUserFullName));
                            pIU.Add('TransactionID', ReadyString(vData.InvoiceID));
                            pIU.Add('TransactionDate', proj.GetZReadDateSql());
                            pIU.Add('LastUpdated', proj.SqliteNow);
                            pIU.Add('DateImported', 'NULL');

                            pSql2 = pIU.GetUpdate('ReferenceNo = ' + ReadyString(vOtherType[i].CardNo));

                            Sql.ExecuteSql(pSql2);

                        }

                    } else {
                        pSql = " DELETE FROM TransCustomerPaymentOther WHERE ID = " + (vOtherType[i].ID == "" ? "NULL" : ReadyString(vOtherType[i].ID));
                    }


                    Sql.ExecuteSql(pSql);
                }


                //Save Discounts 
                SaveBillingDiscount(vData);


                //Save Senior Citizen / PWD customer
                var vSeniorCitizenPWD = vData.SalesPaymentSeniorCitizenPWDList;

                for (var i = 0; i < vSeniorCitizenPWD.length; i++) {

                    saveSeniorCitizenPWDCustomer(vSeniorCitizenPWD[i].ID, vData.InvoiceID);
                }


                ///Update FOWST POS OR Counter
                Business.TransInvoice.UpdateCounter();
                Business.TransInvoice.UpdateCounter(pDeviceFowstTransactionNumber, {
                    FieldName: 'DeviceFowstTransactionNumber',
                    DontUseTableCounter: true
                });

                //save to inventory ledger
                var pDataInvoiceItems;

                pSql = 'Select TransInvoice.ID,  TransInvoice.FOWSTID, Transinvoiceitems.ProductID, Transinvoiceitems.Quantity, Transinvoiceitems.ActualQuantity ' +
                        'from TransInvoice inner join Transinvoiceitems on TransInvoice.ID = Transinvoiceitems.InvoiceID ' +
                        'where TransInvoice.ID = ' + ReadyString(vData.InvoiceID)
                
                pDataInvoiceItems = Sql.Execute(pSql);

                for (var i = 0; i <= pDataInvoiceItems.Rows.length - 1; i++) {
                    Business.TransInventoryLedger.Add(pDataInvoiceItems.Rows[i].ProductID, pDataInvoiceItems.Rows[i].FowstID,
                                                    pDataInvoiceItems.Rows[i].ID, 'Invoice', Web.PeopleUserID(), pDataInvoiceItems.Rows[i].Quantity * pDataInvoiceItems.Rows[i].ActualQuantity * -1,
                                                    undefined, undefined, 'Sale', undefined, undefined, undefined);

                    var mISJIT;
                    pSql = " SELECT IFNULL(Jit, 0) AS Jit " +
                            " ,Instructions " +
                            " FROM LibraryPIMProductParent " +
                            " WHERE LibraryPIMProductParent.ProductID = " + ReadyString(pDataInvoiceItems.Rows[i].ProductID) +
                            " AND LibraryPIMProductParent.OrganizationID = " + ReadyString(sessionStorage.OrgID) 

                            mIsJIT = Sql.Execute(pSql);

                            if (mIsJIT.Rows.length > 0) {
                                if (CBool(mIsJIT.Rows[0].Jit)) {
                                    Business.TransInventoryLedger.Add(pDataInvoiceItems.Rows[i].ProductID, pDataInvoiceItems.Rows[i].FowstID,
                                                    pDataInvoiceItems.Rows[i].ID, 'Invoice', Web.PeopleUserID(), pDataInvoiceItems.Rows[i].Quantity * pDataInvoiceItems.Rows[i].ActualQuantity,
                                                    undefined, undefined, undefined, 'Production', undefined, undefined, undefined);

                                    var mContents;

                                    pSql = 'SELECT LibraryPIMProductContents.ChildProductID ' +
                                    ' ,LibraryPIMProduct.ProductNo ' +
                                    ' ,LibraryPIMProduct.ProductName ' +
                                    ' ,LibraryPIMProductContents.UnitMeasureID ' +
                                    ' ,IFNULL(LibraryCountriesLanguageLabel.Label, LibraryPIMProductContents.UnitMeasure) AS UnitMeasure ' +
                                    ' ,LibraryPIMProductContents.Quantity ' +
                                    ' FROM LibraryPIMProductContents ' +
                                    ' JOIN LibraryPIMProduct ON LibraryPIMProduct.ID = LibraryPIMProductContents.ChildProductID ' +
                                    ' LEFT JOIN LibraryPIMMeasure ON LibraryPIMMeasure.ID = LibraryPIMProductContents.UnitMeasureID ' +
                                    ' LEFT JOIN LibraryCountriesLanguageLabel ON LibraryCountriesLanguageLabel.ID = LibraryPIMMeasure.MeasureNameID ' +
                                    ' WHERE LibraryPIMProductContents.ParentProductID = ' + ReadyString(pDataInvoiceItems.Rows[i].ProductID) +
                                    ' AND LibraryPIMProductContents.OrganizationID = ' + ReadyString(sessionStorage.OrgID) +
                                    ' ORDER BY LibraryPIMProduct.ProductName';

                                    mContents = Sql.Execute(pSql);

                                    for (var j = 0; j <= mContents.Rows.length - 1; j++) {
                                        Business.TransInventoryLedger.Add(mContents.Rows[j].ChildProductID, pDataInvoiceItems.Rows[i].FowstID,
                                                    pDataInvoiceItems.Rows[i].ID, 'Invoice', Web.PeopleUserID(), mContents.Rows[j].Quantity * pDataInvoiceItems.Rows[i].Quantity * -1,
                                                    undefined, undefined, undefined, undefined, undefined, undefined);
                                    }
                                }
                            }
                }

                //save rebates

                pSql = 'SELECT ModifierGroupID ' +
                        ',SUM(Quantity * ActualQuantity) AS Quantity ' +
                        ' FROM TransInvoiceItems ' +
                        ' WHERE InvoiceID = ' + ReadyString(vData.InvoiceID) +
                        ' AND ModifierGroupID IS NOT NULL' +
                        ' GROUP BY ModifierGroupID'

                var pDtInvoiceItems;
                pDtInvoiceItems = Sql.Execute(pSql);

                if (pDtInvoiceItems.Rows.length > 0) {
                    for (var ctrInvoiceItems = 0; ctrInvoiceItems <= pDtInvoiceItems.Rows.length - 1; ctrInvoiceItems++) {
                        pSql = 'Select * from TransInvoiceItemsRebates where ModifierGroupID = ' + ReadyString(pDtInvoiceItems.Rows[ctrInvoiceItems].ModifierGroupID)

                        var pDvModifers;
                        pDvModifers = Sql.Execute(pSql);

                        for (var ctrModifiers = 0; ctrModifiers <= pDvModifers.Rows.length - 1; ctrModifiers++) {
                            Business.TransInventoryLedger.Add(pDvModifers.Rows[ctrModifiers].ProductID, sessionStorage.FowstID,
                                vData.InvoiceID, 'Return', Web.PeopleUserID(), CDec2(pDtInvoiceItems.Rows[ctrInvoiceItems].Quantity) * CDec2(pDvModifers.Rows[ctrModifiers].Quantity) * CDec2(pDvModifers.Rows[ctrModifiers].UnitQuantity),
                                undefined, undefined, undefined, undefined, undefined, undefined);
                        }
                    }
                    
                }

                Sql.CommitTransaction();

                if (proj.ReconstructSalesDataMode) {
                    SalesMenu_offline().SaveRsd({
                        InvoiceID: vData.InvoiceID,
                        DateTime: $.format.date(new Date($('#txtRSDDate').val() + ' ' + $('#txtRSDTime').val()), 'yyyy-MM-dd hh:mm:ss.S'), // format for printing and remove timezone
                        RefNum: $('#txtRSDRefNum').val().trim(),
                        ReasonForReconstructingData: $('#cbReasonForReconstructingData').val()
                    })
                }

                UpdateSalesTextFile(vData.InvoiceID);
                UpdateZReadData(vData.InvoiceID);
                UpdateSalesProductCount(vData.InvoiceID);

                if (vData.SalesPaymentOtherTypeList.length > 0) {
                    UpdateOtherPaymentTypesTextFile(vData.InvoiceID, vData.SalesPaymentOtherTypeList);
                }
                
                // 6/26/2014 10:30 AM
                // remove flushing during payment
                return true;

            }; // END this.SaveSalesPayments = function (vData) {


        ////end of additional

        this.getInvoiceDetailsByFOWSTDevice = function (vInvoiceNo) {

            pSql = "SELECT inv.ID, " +
                " inv.InvoiceNumber, " +
                " inv.CustomerID, " +
                " dsc.DiscountTypeID " +
                " FROM private.TransInvoice inv " +
                " LEFT JOIN private.TransInvoiceDiscount dsc ON inv.ID = dsc.InvoiceID " +
                " Where inv.InvoiceNumber = " + vInvoiceNo +
                (sessionStorage.FowstID != "" ? " AND inv.FowstID = " + ReadyString(sessionStorage.FowstID) : "") +
                " AND inv.DeviceID = " + ReadyString(Web.DeviceID());

            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            return pDT;

        };

        this.getInvoiceDetails = function (vInvoiceNo) {

            pSql = "SELECT inv.ID, " +
                    " inv.InvoiceNumber, " +
                    " inv.CustomerID " +
                    " FROM private.TransInvoice inv " +
                    " Where inv.InvoiceNumber = " + vInvoiceNo;

            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            return pDT;

        };

        this.getOrgFOWSTPrinterIP = function () {

            pSql = "SELECT devfowstpntr.ID, " +
                    " fowstpntr.PrinterName, " +
                    " fowstpntr.PrinterIPAddress, " +
                    " fowstpntr.ProductDescriptionInReceipt " +
                    " FROM private.LibraryDeviceFowstPrinter devfowstpntr " +
                    " INNER JOIN private.LibraryDevicePrinterFowst fowstpntr ON (fowstpntr.ID = devfowstpntr.PrinterFowstID) " +
                    " Where fowstpntr.FOWSTID = '" + sessionStorage.FowstID + "' " +
                    " AND devfowstpntr.DeviceID = '" + Web.DeviceID() + "'";

            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            return pDT;

        };

        this.getOrgPrinterType = function (PrinterID) {

            pSql = "SELECT dp.PrinterType " +
                    " FROM private.LibraryDevicePrinter dp " +                    
                    " Where dp.ID = '" + PrinterID + "' ";

            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            return pDT;

        };

        this.getOrgFOWSTPrinterType = function () {

            pSql = "SELECT devfowstpntr.ID, " +
                    " fowstpntr.PrinterIPAddress, " +
                    " fowstpntr.ConnectionType, " +
                    " dp.PrinterType " +
                    " FROM private.LibraryDeviceFowstPrinter devfowstpntr " +
                    " INNER JOIN private.LibraryDevicePrinterFowst fowstpntr ON (fowstpntr.ID = devfowstpntr.PrinterFowstID) " +
                    " INNER JOIN private.LibraryDevicePrinter dp on dp.ID = fowstpntr.PrinterID " +
                    " Where fowstpntr.FOWSTID = '" + sessionStorage.FowstID + "' " +
                    " AND devfowstpntr.DeviceID = '" + Web.DeviceID() + "'";

            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            return pDT;

        };

        this.getDipatchPrinter = function () {

            pSql = "SELECT fowstpntr.PrinterIPAddress, " +
                    " fowstpntr.PrinterName, " +
                    " fowstpntr.ConnectionType, " +
                    " dp.PrinterType " +
                    " FROM private.LibraryDevicePrinterFowst fowstpntr " +
                    " INNER JOIN private.LibraryDevicePrinter dp on dp.ID = fowstpntr.PrinterID " +
                    " Where fowstpntr.FOWSTID = '" + sessionStorage.FowstID + "' " +
                    " AND IsDefaultDispatchPrinter=1";

            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            return pDT;

        };

        this.getReceiptHeaderInfo = function (vInvoiceID) {

            pSql = "SELECT rsetbr.OrganizationID " +
                    ' ,IFNULL((SELECT BusinessRegisteredName' + '\n' +
                    ' FROM LibraryDevice' + '\n' +
                    ' WHERE ID = devfowst.DeviceID), rsetorg.CompanyName) AS CorporateName \n' +
                    " ,rsetplace.FowstID as OrganizationBranchID " +
                    ' ,IFNULL((SELECT BusinessName' + '\n' +
                    ' FROM LibraryDevice' + '\n' +
                    ' WHERE ID = devfowst.DeviceID), rsettrdname.BusinessTradeName) AS BusinessTradeName \n' +
                    " ,rsettrdname.MaxAllowableDiscountableAmount " +
                    " ,IFNULL(NULLIF(TRIM(rsetbr.StreetNumber) || ' ', ' '), '') || " +
                    " IFNULL(NULLIF(TRIM(rsetbr.Street) || ', ',', '), '') || " +
                    " IFNULL(NULLIF(TRIM(cty.Name) || ', ',', '), '') || " +
                    " IFNULL(NULLIF(TRIM(ctry.Name) || ', ',', '), '') as CompleteAddress " +
                    " ,IFNULL((SELECT GROUP_CONCAT(IFNULL(cctry.CallingCode,'') || ' ' || IFNULL(careacode.CallingCode,'') || ' ' || IFNULL(corgfowsttel.ContactNo,'') || IFNULL(NULLIF('Ext. ' || corgfowsttel.ExtensionNo,'Ext. '),''),', ') " +
	                "   FROM LibraryOrganizationFOWSTTel corgfowsttel " +
	                "   INNER JOIN LibraryCountries cctry ON (corgfowsttel.CountryID = cctry.ID) " +
	                "   INNER JOIN LibraryAreaCodes careacode ON (corgfowsttel.AreaCodeID = careacode.ID) " +
	                "   WHERE corgfowsttel.FOWSTID = '" + sessionStorage.FowstID + "' " +
                    "   ORDER BY IFNULL(cctry.CallingCode,'') || ' ' || IFNULL(careacode.CallingCode,'') || ' ' + IFNULL(corgfowsttel.ContactNo,'') || IFNULL(NULLIF('Ext. ' || corgfowsttel.ExtensionNo,'Ext. '),'') ASC " +
                    "   ),'') as TelNo " +
                    " ,IFNULL((SELECT TinDisplay"+ '\n' +
                    " FROM LibraryDevice" + '\n' +
                    " WHERE ID = devfowst.DeviceID), rsetorg.TinNumber || '-00' || IFNULL(substr('000'||rsetbr.TINBranchCode, -3, 3),'')) as TIN " +
                    " ,devfowst.POSRegistrationNo as POSPermitNo " +
                    " ,devfowst.PTUDateIssued " +
                    " ,(SELECT DefaultValue FROM SystemDefaults WHERE Description = 'Mobiz POS Accreditation Number') as POSAccreditationNo " +
                    " ,devfowst.BIRMachineNo as MIN " +
                    " ,rset.IsShowSalesRep as IsShowSalesRep " +
		            " ,rsetbr.Name AS FowstName \n" +
		            " ,IFNULL(rsetbr.PrintQueueNumbers, 0) AS PrintQueueNumbers \n" +
                    " ,devfowst.IsAuthorizedForPOS " +
                    " ,devfowst.ShowCompanyDetailsInHeader " +
                    ' ,(SELECT UuidDisplay' + '\n' +
                    ' FROM LibraryDevice' + '\n' +
                    ' WHERE ID = devfowst.DeviceID) AS UuidDisplay \n' +
                    " FROM private.LibraryReceiptSettingsFowst rsetplace " +
                    " INNER JOIN private.LibraryReceiptSettings rset ON (rsetplace.ReceiptSettingsID = rset.ID)" +
                    " INNER JOIN LibraryOrganizationFOWST rsetbr ON (rsetplace.FowstID = rsetbr.ID)" +
                    " INNER JOIN LibraryOrganization rsetorg ON (rsetbr.OrganizationID = rsetorg.ID)" +
                    " LEFT JOIN LibraryOrganizationTradeName rsettrdname ON (rset.OrgTradeNameID = rsettrdname.ID)" +
                    " LEFT JOIN LibraryCities cty ON (rsetbr.FowstCityID = cty.ID)" +
                    " LEFT JOIN LibraryCountries ctry ON (rsetbr.FowstCountryID = ctry.ID)" +
                    " LEFT JOIN private.LibraryDeviceFowst devfowst ON (rsetplace.FowstID = devfowst.FOWSTID AND devfowst.DeviceID = " + ReadyString(Web.DeviceID()) + ") " +
                    " LEFT JOIN private.LibraryDevice dev ON (devfowst.DeviceID = dev.ID) " +
                    " WHERE rsetplace.FowstID = '" + sessionStorage.FowstID + "' ";

            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            return pDT;

        };

        this.getSalesPaymentInvoiceHeaderInfo = function (vInvoiceID) {

            pSql = "SELECT inv.ID " +
                    " ,subscrbr.OrganizationID " +
                    " ,inv.FowstID as OrganizationBranchID " +
                    " ,inv.InvoiceNumber " +
                    " ,inv.DeviceFowstTransactionNumber " +
                    " ,inv.InvoiceDate " +
                    " ,inv.SalesOrderNo " +
                    " ,inv.SalesOrderDate " +
                    " ,inv.CustomerID " +
                    " ,inv.DeliveryMethod " +
                    " ,inv.OrderSlipNum " +
                    " ,inv.DepositAmount " +
                    " ,(CASE inv.IsForGovReport " +
                    "     WHEN 1 THEN " +
                    "           (CASE " +
                    "               WHEN cust.IsOrganization = 1 THEN cust.CompanyName " +
                    "               WHEN cust.IsOrganization = 0 THEN " +
                    "                   IFNULL(NULLIF(TRIM(cust.PersonSurname) || ', ',', '), '') || " +
                    "                   IFNULL(NULLIF(TRIM(cust.PersonFirstName) || ' ',' '), '') || " +
                    "                   IFNULL(NULLIF(TRIM(cust.PersonMiddleName) || ' ',' '), '') || " +
                    "                   IFNULL(NULLIF(', ' || TRIM(cust.PersonSuffixName),', '), '') " +
                    "           END) " +
                    "     ELSE " +
                    "       (CASE inv.CustomerID  " + // (CASE inv.DeliveryMethod [kung kinsa ang customer gibutang dapat mao pud mogawas sa resibo] modified by arnie May5,27 10:30am
                    "       WHEN 'cd71a0f7-08a9-4601-876e-5f13c064f873' THEN '[CASH PAYMENT]' " + // WHEN 'Pickup' THEN '[CASH PAYMENT]'
                    "       ELSE  " +
                    "           (CASE " +
                    "               WHEN cust.IsOrganization = 1 THEN cust.CompanyName " +
                    "               WHEN cust.IsOrganization = 0 THEN " +
                    "                   IFNULL(NULLIF(TRIM(cust.PersonFullname) || ' ',' '), '') || " +
                    "                   IFNULL(NULLIF(TRIM(cust.PersonSurname) || ', ',', '), '') || " +
                    "                   IFNULL(NULLIF(TRIM(cust.PersonFirstName) || ' ',' '), '') || " +
                    "                   IFNULL(NULLIF(TRIM(cust.PersonMiddleName) || ' ',' '), '') || " +
                    "                   IFNULL(NULLIF(', ' || TRIM(cust.PersonSuffixName),', '), '') " +
                    "           END) " +
                    "       END) " +
                    " END) as CustomerName " +
                    " ,(CASE inv.IsForGovReport " +
                    "     WHEN 1 THEN " +
                    "       (CASE inv.DeliveryMethod  " +
                    "           WHEN 'Pickup' THEN IFNULL(NULLIF(TRIM(custdeladd.StreetNumber), ''), cust.OfficeAddressBuildingNo) || " +
                    "               IFNULL(NULLIF(', ' || TRIM(custdeladd.Street),', '), ', ' || cust.OfficeAddressStreet) || " +
                    "               IFNULL(NULLIF(', ' || TRIM(custdeladd.Name),', '), ', ' || custcty.Name) || " +
                    "               IFNULL(NULLIF(', ' || TRIM(custdeladd.Name),', '), ', ' || custctry.Name) " +
                    "           ELSE IFNULL(NULLIF(TRIM(custdeladd.StreetNumber), ''), cust.OfficeAddressBuildingNo) || " +
                    "               IFNULL(NULLIF(', ' || TRIM(custdeladd.Street),', '), ', ' || cust.OfficeAddressStreet) || " +
                    "               IFNULL(NULLIF(', ' || TRIM(custdeladd.Name),', '), ', ' || custcty.Name) || " +
                    "               IFNULL(NULLIF(', ' || TRIM(custdeladd.Name),', '), ', ' || custctry.Name) " +
                    "           END) " +
                    "     ELSE " +
                    "       (CASE inv.DeliveryMethod  " +
                    "           WHEN 'Pickup' THEN  " +
                    "               IFNULL(NULLIF(TRIM(custbilladd.StreetNumber), ''), cust.OfficeAddressBuildingNo) || " +
                    "               IFNULL(NULLIF(', ' || TRIM(custbilladd.Street),', '), ', ' || cust.OfficeAddressStreet) || " +
                    "               IFNULL(NULLIF(', ' || TRIM(custbillcty.Name),', '), ', ' || custcty.Name) || " +
                    "               IFNULL(NULLIF(', ' || TRIM(custbillctry.Name),', '), ', ' || custctry.Name) " +
                    "           ELSE IFNULL(NULLIF(TRIM(custbilladd.StreetNumber), ''), cust.OfficeAddressBuildingNo) || " +
                    "               IFNULL(NULLIF(', ' || TRIM(custbilladd.Street),', '), ', ' || cust.OfficeAddressStreet) || " +
                    "               IFNULL(NULLIF(', ' || TRIM(custbillcty.Name),', '), ', ' || custcty.Name) || " +
                    "               IFNULL(NULLIF(', ' || TRIM(custbillctry.Name),', '), ', ' || custctry.Name) " +
                    "       END) " +
                    " END) as CustomerAddress " +
                    " ,cust.TinNumber as CustomerTIN " +
                    " ,cust.BusinessIndustryText as BusinessStyle " +
                    " ,inv.BillingAddressID " +
                    " ,(CASE inv.DeliveryMethod " +
                    "     WHEN 'Pickup' THEN " +
                    "           IFNULL(NULLIF(TRIM(custbilladd.StreetNumber), ''), cust.OfficeAddressBuildingNo) || " +
                    "          IFNULL(NULLIF(', ' || TRIM(custbilladd.Street),', '), ', ' || cust.OfficeAddressStreet) || " +
                    "          IFNULL(NULLIF(', ' || TRIM(custbillcty.Name),', '), ', ' || custcty.Name) || " +
                    "          IFNULL(NULLIF(', ' || TRIM(custbillctry.Name),', '), ', ' || custctry.Name) " +
                    "     ELSE IFNULL(NULLIF(TRIM(custbilladd.StreetNumber), ''), cust.OfficeAddressBuildingNo) || " +
                    "          IFNULL(NULLIF(', ' || TRIM(custbilladd.Street),', '), ', ' || cust.OfficeAddressStreet) || " +
                    "          IFNULL(NULLIF(', ' || TRIM(custbillcty.Name),', '), ', ' || custcty.Name) || " +
                    "          IFNULL(NULLIF(', ' || TRIM(custbillctry.Name),', '), ', ' || custctry.Name) " +
                    " END) as BillingAddress " +
                    " ,inv.DeliveryAddressID " +
                    " ,(CASE inv.DeliveryMethod " +
                    "     WHEN 'Pickup' THEN '' " +
                    "     ELSE IFNULL(NULLIF(TRIM(custdeladd.StreetNumber), ''), '') || " +
                    "          IFNULL(NULLIF(', ' || TRIM(custdeladd.Street),', '), '') || " +
                    "          IFNULL(NULLIF(', ' || TRIM(custdeladdcty.Name),', '), '') || " +
                    "          IFNULL(NULLIF(', ' || TRIM(custdeladdctry.Name),', '), '') " +
                    " END) as DeliveryAddress " +
                    " ,IFNULL(NULLIF(TRIM(cshr.SurName) || ', ',', '), '') || " +
                    "  IFNULL(NULLIF(TRIM(cshr.FirstName) || ' ',' '), '') || " +
                    "  IFNULL(NULLIF(TRIM(cshr.MiddleName) || ' ',' '), '') as CashierName " +
                    " ,TRIM(IFNULL((SELECT DefaultValue FROM private.SystemDefaults WHERE Description = 'Receipt Footer Message'), 'This Serves as Your Sales Invoice.')) as ReceiptFooterMsg " +
                    " ,tableInfo.TableName as CustomerTableNo " +
                    " ,inv.Vatable TaxSales " +
                    " ,inv.VAT TaxAmount " +
                    " ,inv.VATExempt VATExemptSale " +
                    " ,0 ZeroRatedSale " +
                    " ,inv.ForegoneVAT " +
                    " ,inv.PrintCount " +
                    ' ,(SELECT UuidDisplay' + '\n' + 
                    ' FROM LibraryDevice' + '\n' + 
                    ' WHERE ID = inv .DeviceID) AS UuidDisplay\n' +
                    " FROM private.TransInvoice inv " +
                    " INNER JOIN LibraryOrganization cust ON (inv.CustomerID = cust.ID) " +
                    " LEFT JOIN LibraryCities custcty ON (cust.OfficeAddressCityID = custcty.ID) " +
                    " LEFT JOIN LibraryCountries custctry ON (cust.OfficeAddressCountryID = custctry.ID) " +
                    " LEFT JOIN LibraryOrganizationFOWST custbilladd ON (inv.BillingAddressID = custbilladd.ID) " +
                    " LEFT JOIN LibraryCities custbillcty ON (custbilladd.FowstCityID = custbillcty.ID) " +
                    " LEFT JOIN LibraryCountries custbillctry ON (custbilladd.FowstCountryID = custbillctry.ID) " +
                    " LEFT JOIN LibraryOrganizationFOWST custdeladd ON (inv.DeliveryAddressID = custdeladd.ID) " +
                    " LEFT JOIN LibraryCities custdeladdcty ON (custdeladd.FowstCityID = custdeladdcty.ID) " +
                    " LEFT JOIN LibraryCountries custdeladdctry ON (custdeladd.FowstCountryID = custdeladdctry.ID) " +
                    " LEFT JOIN LibraryOrganizationFOWST subscrbr ON (inv.FowstID = subscrbr.ID) " +
                    " LEFT JOIN LibraryPeople cshr ON (inv.UserID = cshr.ID) " +
                    " LEFT JOIN LibraryTableTableInfo as tableInfo ON tableInfo.InternalTableName = TRIM(SUBSTR(inv.CustomerTableNo, 5)) " + // added by arnie 92117
                    " WHERE inv.ID = '" + vInvoiceID + "' ";

            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            return pDT;

        };

        this.getSalesPaymentReceiptBodyInfo = function (vInvoiceID, vOptions) {

            vOptions = $.extend({
                PrintAsOrderSlip: false,
                PrintAsCancellationSlip: false,
                Category: null
            }, vOptions);

            // removed invitem.ID
            pSql = "SELECT \n" +
                " invitem.ID as ItemID " +
                ", invitem.ProductID as StockID\n" +
                ",invitem.QuantityPrinted\n" +
                " ,prd.ProductNo as StockNumber\n";
            pSql += ' ,CASE WHEN tblPimRebate.ProductName IS NULL THEN prd.ProductName' + '\n' + 
                ' ELSE prd.ProductName || \' (Rebate of PHP\' || (TransInvoiceItemsRebates.Amount * invitem.Quantity) || \' in exchange for \' || (TransInvoiceItemsRebates.Quantity * invitem.Quantity) || \' \' || TransInvoiceItemsRebates.UnitMeasure || \' \' || tblPimRebate.ProductName || \')\'' + '\n' + 
                ' END AS StockDescription\n';
            pSql += " ,invitem.Quantity AS Quantity\n" + // " ,SUM(invitem.Quantity) AS Quantity\n" //+ -> orig code as of Apr11,2017 130pm
                " ,invitem.UnitMeasure\n" +
                " ,invitem.UnitPrice\n" +
                " ,invitem.SubTotal AS SubTotal\n" + //" ,SUM(invitem.SubTotal) AS SubTotal\n" + //-> orig code as of Apr11,2017 130pm
                " ,(SELECT GROUP_CONCAT(ppl.Surname || ', ' || ppl.FirstName || ' ' || ppl.MiddleName,'; ') FROM private.TransInvoiceItemSalesReps srep INNER JOIN LibraryPeople ppl ON (srep.PeopleID = ppl.ID) WHERE srep.InvoiceID = invitem.InvoiceID AND srep.ProductID = invitem.ProductID ORDER BY (ppl.Surname || ', ' || ppl.FirstName || ' ' || ppl.MiddleName) ASC) as SalesPerson\n " +
                " , lop.IsVatExempt" +
                " , LibraryPIMProductVariableFeatures001.C0090 ReceiptName";
            if (vOptions.Category != null)
                pSql += ',LibraryPIMProductVariableFeatures001.C0020 AS Category \n';

            pSql += " FROM private.TransInvoiceItems invitem " +
                " INNER JOIN LibraryPIMProduct prd ON (invitem.ProductID = prd.ID) " +
                " INNER JOIN private.LibraryOrganizationPIM lop ON (invitem.ProductID = lop.ProductID)\n";

            pSql += ' LEFT JOIN private.TransInvoiceItemsRebates ON TransInvoiceItemsRebates.ModifierGroupID = invitem.ModifierGroupID' + '\n' + 
                ' LEFT JOIN LibraryPIMProduct tblPimRebate ON tblPimRebate.ID = TransInvoiceItemsRebates.ProductID' + '\n' + 
                ' LEFT JOIN private.LibraryItemModifierRebates ON LibraryItemModifierRebates.ID = TransInvoiceItemsRebates.RebateID' + '\n' + 
                ' LEFT JOIN private.LibraryItemModifiers ON LibraryItemModifiers.ID = LibraryItemModifierRebates.ItemModifierID\n';
            //if (vOptions.Category != null) gicomment kay kailangan ang data sa c0090 para short product description
                pSql += 'LEFT JOIN LibraryPIMProductVariableFeatures001 ON LibraryPIMProductVariableFeatures001.ProductID = invitem.ProductID\n';
            pSql += " WHERE invitem.InvoiceID = '" + vInvoiceID + "' \n";

            //if (vOptions.Category != null) {
            //    for (var i = 0; i <= vOptions.Category.length - 1; i++) {
            //        if (i == 0) {
            //            pSql += ' AND (IFNULL(LibraryPIMProductVariableFeatures001.C0020, \'\') = ' + ReadyString(vOptions.Category[i]) + '\n';
            //        } else {
            //            pSql += ' OR IFNULL(LibraryPIMProductVariableFeatures001.C0020, \'\') = ' + ReadyString(vOptions.Category[i]) + '\n';
            //        }
            //    }

            //    pSql += ' )';
            //}
            if (vOptions.Category != null)
                pSql += ' AND IFNULL(LibraryPIMProductVariableFeatures001.C0020, \'\') COLLATE NOCASE = ' + ReadyString(vOptions.Category) + '\n';

            if (vOptions.PrintAsOrderSlip) {
                if (!vOptions.IsReprintAll) {
                    pSql += ' AND IFNULL(invitem.IsOrderSlipPrinted, 0) = 0\n';
                    pSql += ' AND IFNULL(invitem.Quantity, 0) > IFNULL(invitem.QuantityPrinted, 0)\n';
                } else {
                    pSql += ' AND IFNULL(invitem.Quantity, 0) > 0 \n';
                }
            } else if (vOptions.PrintAsCancellationSlip) {
                pSql += ' AND IFNULL(invitem.Quantity, 0) < IFNULL(invitem.QuantityPrinted, 0)\n';
            } else 
                pSql += ' AND IFNULL(invitem.Quantity, 0) > 0\n';

            //pSql += ' GROUP BY invitem.ProductID' + '\n' + //-> orig code as of Apr11,2017 130pm
            //    ' ,invitem.UnitMeasure' + '\n' + 
            //    ' ,invitem.UnitPrice\n';
            pSql += " ORDER BY prd.ProductNo ASC, " +
                " invitem.ActualQuantity DESC";

            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;
            Business.TransInvoice.GetModifiersOfItem(pDT.Rows);
            return pDT;

        };

        this.getSalesPaymentReceiptBOMPrinter = function (vInvoiceID) {

            pSql = "SELECT DISTINCT " +
                    " pntr.ID " +
                    " ,pntr.PrinterType " +
                    " ,pntrfowst.PrinterName as PrinterRole " +
                    " ,pntrfowst.PrinterIPAddress " +
                    " FROM private.TransInvoiceItems invitem " +
                    " INNER JOIN private.TransInvoice inv ON (invitem.InvoiceID = inv.ID) " +
                    " LEFT JOIN LibraryPIMProductVariableFeatures001 prdfea ON (invitem.ProductID = prdfea.ProductID) " +
                    " LEFT JOIN private.LibraryDevicePrinterCategory pntrcat ON (pntrcat.CategoryName = IFNULL(prdfea.C0020,'N/A')) " +
                    " LEFT JOIN private.LibraryDevicePrinter pntr ON (pntrcat.PrinterID = pntr.ID) " +
                    " LEFT JOIN private.LibraryDevicePrinterFowst pntrfowst ON (pntrcat.PrinterID = pntrfowst.PrinterID AND inv.FowstID = pntrfowst.FOWSTID) " +
                    " WHERE invitem.InvoiceID = '" + vInvoiceID + "' ";
            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            return pDT;

        };

        this.getSalesPaymentReceiptBOMByPrinter = function (vInvoiceID, vPrinterID) {

            pSql = "SELECT invitem.ID " +
                    " ,inv.FowstID as OrganizationBranchID " +
                    " ,inv.InvoiceNumber " +
                    " ,inv.InvoiceDate " +
                    " ,inv.SalesOrderNo " +
                    " ,inv.SalesOrderDate " +
                    " ,inv.CustomerID " +
                    " ,(CASE inv.IsForGovReport " +
                    "     WHEN 1 THEN " +
                    "           (CASE " +
                    "               WHEN cust.IsOrganization = 1 THEN cust.CompanyName " +
                    "               WHEN cust.IsOrganization = 0 THEN " +
                    "                   IFNULL(NULLIF(TRIM(cust.PersonSurname) || ', ',', '), '') || " +
                    "                   IFNULL(NULLIF(TRIM(cust.PersonFirstName) || ' ',' '), '') || " +
                    "                   IFNULL(NULLIF(TRIM(cust.PersonMiddleName) || ' ',' '), '') || " +
                    "                   IFNULL(NULLIF(', ' || TRIM(cust.PersonSuffixName),', '), '') " +
                    "           END) " +
                    "     ELSE " +
                    "       (CASE inv.DeliveryMethod  " +
                    "       WHEN 'Pickup' THEN '[CASH PAYMENT]' " +
                    "       ELSE  " +
                    "           (CASE " +
                    "               WHEN cust.IsOrganization = 1 THEN cust.CompanyName " +
                    "               WHEN cust.IsOrganization = 0 THEN " +
                    "                   IFNULL(NULLIF(TRIM(cust.PersonSurname) || ', ',', '), '') || " +
                    "                   IFNULL(NULLIF(TRIM(cust.PersonFirstName) || ' ',' '), '') || " +
                    "                   IFNULL(NULLIF(TRIM(cust.PersonMiddleName) || ' ',' '), '') || " +
                    "                   IFNULL(NULLIF(', ' || TRIM(cust.PersonSuffixName),', '), '') " +
                    "           END) " +
                    "       END) " +
                    " END) as CustomerName " +
                    " ,IFNULL(NULLIF(TRIM(cshr.SurName) || ', ',', '), '') || " +
                    "  IFNULL(NULLIF(TRIM(cshr.FirstName) || ' ',' '), '') || " +
                    "  IFNULL(NULLIF(TRIM(cshr.MiddleName) || ' ',' '), '') as CashierName " +
                    " ,inv.CustomerTableNo " +
                    " ,pntr.PrinterType " +
                    " ,pntr.PrinterRole " +
                    " ,invitem.ProductID as StockID " +
                    " ,prd.ProductNo as StockNumber " +
                    " ,prd.ProductName as StockDescription " +
                    " ,invitem.OrderedQuantity " +
                    " ,invitem.Quantity " +
                    " ,invitem.UnitMeasure " +
                    " ,pntrcat.CategoryName as ProductCategory " +
                    " ,pntrfowst.PrinterIPAddress " +
                    " ,orgfowst.FOWSTNo " +
                    " ,dev.DeviceNo " +
                    " FROM private.TransInvoiceItems invitem " +
                    " INNER JOIN LibraryPIMProduct prd ON (invitem.ProductID = prd.ID) " +
                    " INNER JOIN private.TransInvoice inv ON (invitem.InvoiceID = inv.ID) " +
                    " INNER JOIN LibraryOrganization cust ON (inv.CustomerID = cust.ID) " +
                    " LEFT JOIN LibraryPeople cshr ON (inv.UserID = cshr.ID) " +
                    " LEFT JOIN LibraryPIMProductVariableFeatures001 prdfea ON (invitem.ProductID = prdfea.ProductID) " +
                    " LEFT JOIN private.LibraryDevicePrinterCategory pntrcat ON (pntrcat.CategoryName = IFNULL(prdfea.C0020,'N/A')) " +
                    " LEFT JOIN private.LibraryDevicePrinter pntr ON (pntrcat.PrinterID = pntr.ID) " +
                    " LEFT JOIN private.LibraryDevicePrinterFowst pntrfowst ON (pntrcat.PrinterID = pntrfowst.PrinterID AND inv.FowstID = pntrfowst.FOWSTID) " +
                    " LEFT JOIN LibraryDevice dev on inv.DeviceID = dev.ID " +
                    " LEFT JOIN LibraryOrganizationfowst orgfowst on orgfowst.ID = inv.FowstID " +
                    " WHERE invitem.InvoiceID = '" + vInvoiceID + "' " +
                    " AND pntr.ID = '" + vPrinterID + "' " +
                    " ORDER BY prd.ProductNo ASC, " +
                    " invitem.ActualQuantity DESC";

            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            return pDT;

        };

        this.getSalesPaymentReceiptSerial = function (vInvoiceID) {

            pSql = "SELECT invserial.InvoiceID, invserial.ProductID, invserial.SerialNumber " +
                    " FROM private.TransInvoiceItemSerials invserial " +
                    " WHERE invserial.InvoiceID = '" + vInvoiceID + "' " +
                    " ORDER BY invserial.ProductID ASC";

            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            return pDT;

        };

        this.getSalesPaymentReceiptPaymentInfo = function (vInvoiceID) {

            pSql = "SELECT CAST(SUM(IFNULL(cpay.CashAmount,0)) as TEXT) as CashPayment " +
                    " ,CAST(SUM(IFNULL(cpay.CheckPaymentAmount,0)) as TEXT) as CheckPaymentAmount " +
                    " ,CAST(SUM(IFNULL(cpay.DebitCardAmount,0)) as TEXT) as DebitCardAmount " +
                    " ,CAST(SUM(IFNULL(cpay.CreditCardAmount,0)) as TEXT) as CreditCardAmount " +
                    " ,CAST(SUM(IFNULL(cpay.SRVAmount,0)) as TEXT) as SRVAmount" +
                    " ,(SELECT GROUP_CONCAT(SrvNum, ', ') FROM TransSalesReturnLedger WHERE SourceMemoID = (SELECT TransCustomerPaymentSRV.MemoID " +
                    "   FROM TransCustomerPaymentSRV " +
                    "   JOIN TransCustomerPaymentInvoices ON TransCustomerPaymentInvoices.CustomerPaymentID = TransCustomerPaymentSRV.CustomerPaymentID  " +
                    "   WHERE TransCustomerPaymentInvoices.InvoiceID = '" + vInvoiceID + "')) as SRVNo" +
                    " ,CAST(SUM(IFNULL(cpay.OtherTypeAmount,0)) as TEXT) as OtherTypeAmount" +
                    " ,CAST(SUM(IFNULL(cpay.OnCreditAmount,0)) as TEXT) as OnCreditAmount" +
                    " ,CAST(SUM(IFNULL(cpay.CustomerChangeCash,0)) as TEXT) as CustomerChangeCash " +
                    " ,CAST(SUM(IFNULL(cpay.OnCreditAmount,0)) as TEXT) as OnCreditAmount " +
                    " ,cpay.OnCreditRemarks as OnCreditRemarks " +
                    " FROM private.TransInvoice inv " +
                    " INNER JOIN private.TransCustomerPaymentInvoices cpayinv ON (inv.ID = cpayinv.InvoiceID) " +
                    " INNER JOIN private.TransCustomerPayment cpay ON (cpayinv.CustomerPaymentID = cpay.ID) " +
                    " WHERE inv.ID = '" + vInvoiceID + "' ";

            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            pSql = " SELECT PaymentType, CardNumber, Amount \n" +
                " FROM TransCustomerPaymentInvoices as payinv \n" +
                " INNER JOIN TransCustomerPaymentOther as otherpay ON otherpay.CustomerPaymentID = payinv.CustomerPaymentID \n" +
                " WHERE InvoiceID = " + ReadyString(vInvoiceID);
            pDT = $.extend({ OtherPayments: [] }, pDT);
            pDT.OtherPayments = Sql.Execute(pSql);

            return pDT;

        };

        this.getSalesPaymentReceiptFooterAmountsInfo = function (vInvoiceID, vOptions) {

            vOptions = $.extend({
                PrintAsOrderSlip: false,
                Category: null
            }, vOptions);
            var queryForTotalModifier = "(SELECT SUM(im.SubTotal) FROM TransInvoiceItemsModifier as im INNER JOIN TransInvoiceItems as ii ON ii.ID = im.InvoiceItemID WHERE ii.InvoiceID = invitem.InvoiceID)";

            //" ,CAST(SUM(invitem.VATExemptSale) as TEXT) as VATExemptSale " +

            pSql = "SELECT COUNT(invitem.ProductID) as SKUCount " +
                " ,CAST((SUM(IFNULL(invitem.VATableSale,SubTotal) + IFNULL(invitem.VATExemptSale,0) + IFNULL(invitem.ZeroRatedSale,0) + IFNULL(invitem.NonVATSale,0))) as TEXT) as TotalAmountDue " +
                " ,CAST(SUM(IFNULL(invitem.DiscountAmount,0)) as TEXT) as DiscountAmount " +
                " ,CAST((SUM(IFNULL(invitem.VATableSale,SubTotal) + IFNULL(invitem.VATExemptSale,0) + IFNULL(invitem.ZeroRatedSale,0) + IFNULL(invitem.NonVATSale,0) - IFNULL(invitem.DiscountAmount,0))) as TEXT) as NetAmountDue " +
                " ,CAST(SUM(IFNULL(invitem.TaxSales, 0) - IFNULL(invitem.DiscountSales, 0)) as TEXT) as TaxSales" +
                " ,CAST(SUM(IFNULL(invitem.TaxAmount, 0) - IFNULL(invitem.DiscountTax, 0)) as TEXT) as TaxAmount " +
                " ,CAST(SUM(invitem.NonVATSale) as TEXT) as NonVATSale " +
                " ,(SELECT IFNULL(ForegoneVAT, 0) FROM private.TransInvoice WHERE ID = inv.ID LIMIT 1) as VATExemptTax " +
                " ,(SELECT IFNULL(VATExempt, 0) - IFNULL(inv.SCDiscount, 0) FROM private.TransInvoice WHERE ID = inv.ID LIMIT 1) as VATExemptSale " +
                " ,(SELECT IFNULL(Vatable, 0) FROM private.TransInvoice WHERE ID = inv.ID LIMIT 1) as VatablePortionOfSCDTransaction " +
                " ,(SELECT IFNULL(VAT, 0) FROM private.TransInvoice WHERE ID = inv.ID LIMIT 1) as VatAmountPortionOfSCDTransaction " +
                " ,CAST(SUM(invitem.ZeroRatedSale) as TEXT) as ZeroRatedSale " +
                " ,(SELECT IFNULL(TaxPercent, 0) FROM private.TransInvoiceItems WHERE InvoiceID = inv.ID LIMIT 1) as TaxPercent" +
                " ,CAST(SUM(invitem.SubTotal) as TEXT) as SubTotal " +
                " ,(SELECT CASE COUNT(ChargeType) WHEN 1 THEN ChargeType ELSE 'Total Charges' END FROM private.TransInvoiceCharges WHERE InvoiceID = inv.ID) as ChargeType " +
                " ,(SELECT CAST(ROUND(SUM(IFNULL(ChargeAmount, 0)), 4) as TEXT) FROM private.TransInvoiceCharges WHERE InvoiceID = inv.ID) as ChargeAmount " +
                " ,(SELECT CAST(ROUND(SUM(IFNULL(TipAmount, 0)), 4) as TEXT) FROM private.TransInvoiceCharges WHERE InvoiceID = inv.ID) as TipAmount " +
                " ,(SELECT CAST(ROUND(SUM(IFNULL(TaxAmount, 0)), 4) as TEXT) FROM private.TransInvoiceCharges WHERE InvoiceID = inv.ID) as LocalTaxAmount\n" +
                " ,tblDiscount.DiscountPercentage, tblDiscount.DiscountType\n";
            if (vOptions.Category != null) 
                pSql += ',IFNULL(LibraryPIMProductVariableFeatures001.C0020, \'\') AS Category\n';
            pSql += " FROM private.TransInvoiceItems invitem " +
                " LEFT JOIN private.TransInvoice inv ON (invitem.InvoiceID = inv.ID)\n" +
                " LEFT JOIN (SELECT InvoiceID, DiscountPercentage, DiscountType FROM TransInvoiceDiscount \n" +
                "   LEFT JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = TransInvoiceDiscount.DiscountTypeID) as tblDiscount ON tblDiscount.InvoiceID = inv.ID \n"
            if (vOptions.Category != null) 
                pSql += 'LEFT JOIN LibraryPIMProductVariableFeatures001 ON LibraryPIMProductVariableFeatures001.ProductID = invitem.ProductID\n';
            pSql += " WHERE invitem.Quantity > 0 AND invitem.InvoiceID = " + ReadyString(vInvoiceID) + '\n';
            if (vOptions.Category != null)
                pSql += ' AND IFNULL(LibraryPIMProductVariableFeatures001.C0020, \'\') COLLATE NOCASE = ' + ReadyString(vOptions.Category) + '\n';

            if (vOptions.PrintAsOrderSlip) 
                pSql += ' AND IFNULL(invitem.IsOrderSlipPrinted, 0) = 0\n';
            pSql += 'GROUP BY invitem.InvoiceID';

            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            return pDT;

        };

        this.getSalesPaymentReceiptFooterMessagesInfo = function () {

            pSql = "SELECT rsetmsg.MessageFooter as MessageFooter " +
                    " FROM private.LibraryReceiptSettingsMessage rsetmsg " +
                    " ORDER BY rsetmsg.SortOrder ASC";

            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            return pDT;

        };

        ///////////////////////////////////////////////////////////
        //additional by janine dec 17 2014 billing
        this.RemoveBillingDiscount = function (vData) {

            //reset to no discount
            pSqlDelete = "DELETE " +
                            " FROM private.TransInvoiceDiscount " +
                            " WHERE InvoiceID = " + ReadyString(vData.InvoiceID);

            Sql.ExecuteSql(pSqlDelete);

            pSqlDelete = "DELETE " +
                            " FROM private.TransInvoiceSeniorCitizenPWDCustomer " +
                            " WHERE InvoiceID = " + ReadyString(vData.InvoiceID);

            Sql.ExecuteSql(pSqlDelete);

            //Save Discount on Invoice Item
            pIU = new InsertUpdate('private.TransInvoiceItems');
            pIU.Add('DiscountAmount', "0");
            pIU.Add('DiscountSales', "0");
            pIU.Add('DiscountTax', "0");
            pIU.Add('TaxSales', "Subtotal / (cast((100 + TaxPercent) as float)/100)");
            pSql = pIU.GetUpdate('InvoiceID = ' + ReadyString(vData.InvoiceID) + ' AND OriginalUnitPrice = UnitPrice');
            Sql.ExecuteSql(pSql);

            pIU = new InsertUpdate('private.TransInvoiceItems');
            pIU.Add('TaxAmount', "Subtotal - TaxSales");
            pIU.Add('VATExemptSale', '0');
            pIU.Add('ZeroRatedSale', '0');
            pIU.Add('NonVATSale', '0');
            pIU.Add('VATableSale', '(SubTotal)');

            pSql = pIU.GetUpdate('InvoiceID = ' + ReadyString(vData.InvoiceID) + ' AND OriginalUnitPrice = UnitPrice');
            Sql.ExecuteSql(pSql);

        };

        this.SaveBillingDiscount = function (vData) {
            //Save Discounts
            var pSqlDelete;
            var vDiscount = vData.SalesPaymentDiscountList;


            for (var i = 0; i < vDiscount.length; i++) {

                pSql = "SELECT ID FROM private.TransInvoiceDiscount " +
                    " Where InvoiceID = " + ReadyString(vData.InvoiceID);

                var pDT = Sql.Execute(pSql);

                pIU = new InsertUpdate('private.TransInvoiceDiscount');
                pIU.Add('InvoiceID', ReadyString(vData.InvoiceID));
                pIU.Add('DiscountTypeID', ReadyString(vDiscount[i].ID));
                pIU.Add('DiscountPercentage', CDec2(vDiscount[i].DiscountPercent));
                pIU.Add('DiscountRemarks', ReadyString(vDiscount[i].DiscountRemarks));
                //additional by janine

                if (CDec2(vData.NoOfPeople) > 0)
                    pIU.Add('NoOfPeople', vData.NoOfPeople);
                else
                    pIU.Add('NoOfPeople', 'NULL');

                if (CDec2(vData.NoOfSeniorCitizens) > 0)
                    pIU.Add('NoOfSeniorCitizens', vData.NoOfSeniorCitizens);
                else
                    pIU.Add('NoOfSeniorCitizens', 'NULL');

                //end of additional
                //if existing , update
                if (pDT.Rows.length > 0)
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(pDT.Rows[0].ID));
                else {
                    pIU.Add('ID', ReadyString(NewID()));
                    pSql = pIU.GetInsert();
                }
                Sql.ExecuteSql(pSql);

                //Check if NOT Senior Citizen PWD, NATTEAM, SOLO PARENT, then delete in case they have records

                if (CStr2(vDiscount[i].ID).toUpperCase() != '055A0A33-DD98-47EC-ADD3-8B2009F3944A' &&
                    CStr2(vDiscount[i].ID).toUpperCase() != '34A4032F-A142-4C15-A868-4AC4410434DF' &&
                    CStr2(vDiscount[i].ID).toUpperCase() != '674AD791-188B-4238-B95C-97B5C2B9D544' &&
                    CStr2(vDiscount[i].ID).toUpperCase() != '21EBF55B-526C-4276-B3B7-0CBCA504F425' &&
                    CStr2(vDiscount[i].ID).toUpperCase() != 'E95F0B64-1EEA-47B4-BDE5-B795234B462D') {

                    pSqlDelete = "DELETE " +
                        " FROM private.TransInvoiceSeniorCitizenPWDCustomer " +
                        " WHERE InvoiceID = " + ReadyString(vData.InvoiceID);

                    Sql.ExecuteSql(pSqlDelete);
                }

                //senior, pwd, natteam, soloparent
                if (CStr2(vDiscount[i].ID).toUpperCase() == '055A0A33-DD98-47EC-ADD3-8B2009F3944A' ||
                    CStr2(vDiscount[i].ID).toUpperCase() == '674AD791-188B-4238-B95C-97B5C2B9D544' ||
                    CStr2(vDiscount[i].ID).toUpperCase() == '21EBF55B-526C-4276-B3B7-0CBCA504F425' ||
                    CStr2(vDiscount[i].ID).toUpperCase() == 'E95F0B64-1EEA-47B4-BDE5-B795234B462D') {

                    SaveInvoiceItemTaxAndDiscountSCD(vData, vDiscount[i]);

                } else if (CStr2(vDiscount[i].ID).toUpperCase() == '34A4032F-A142-4C15-A868-4AC4410434DF') {
                    SaveInvoiceItemTaxAndDiscountSCD(vData, vDiscount[i]); // old: SaveInvoiceItemTaxAndDiscountPWD
                } else {

                    //Save Discount on Invoice Item
                    pIU = new InsertUpdate('private.TransInvoiceItems');
                    pIU.Add('DiscountAmount', "Subtotal - (Subtotal * (cast((100 - " + CDec2(vDiscount[i].DiscountPercent) + ") as float) /100))");
                    pIU.Add('DiscountSales', " (Subtotal - (Subtotal * (cast((100 - " + CDec2(vDiscount[i].DiscountPercent) + ") as float) /100))" + ")/ (cast((100 + TaxPercent) as float)/100)");
                    pIU.Add('TaxSales', "Subtotal / (cast((100 + TaxPercent) as float)/100)");
                    pSql = pIU.GetUpdate('InvoiceID = ' + ReadyString(vData.InvoiceID) + ' AND OriginalUnitPrice = UnitPrice');
                    Sql.ExecuteSql(pSql);

                    pIU = new InsertUpdate('private.TransInvoiceItems');
                    pIU.Add('DiscountTax', "DiscountAmount - DiscountSales");
                    pIU.Add('TaxAmount', "Subtotal - TaxSales");
                    pIU.Add('VATExemptSale', '0');
                    pIU.Add('ZeroRatedSale', '0');
                    pIU.Add('NonVATSale', '0');
                    pIU.Add('VATableSale', '(SubTotal)');

                    pSql = pIU.GetUpdate('InvoiceID = ' + ReadyString(vData.InvoiceID) + ' AND OriginalUnitPrice = UnitPrice');
                    Sql.ExecuteSql(pSql);
                }


            }


        };

        //FOR SCD PHILIPPINE LAW
        this.SaveInvoiceItemTaxAndDiscountSCD = function (vData, vDiscountIndex) {


            var TotalDiscount;
            var SubTotalNetOfVAT;
            var NewSubTotal;

            var proportionOfSeniors = parseFloat(vData.NoOfSeniorCitizens) / parseFloat(vData.NoOfPeople);
            var GrossSales = parseFloat(vData.CashAmount) + parseFloat(vData.CheckPaymentAmount) + parseFloat(vData.CreditCardAmount) + parseFloat(vData.DebitCardAmount) + parseFloat(vData.OtherTypeAmount) + parseFloat(vData.OnCreditAmount);
            var GrossSalesToSC = proportionOfSeniors * GrossSales;

            var proportionofregular = (parseFloat(vData.NoOfPeople) - parseFloat(vData.NoOfSeniorCitizens)) / parseFloat(vData.NoOfPeople);
            var GrossSalesToNoneSC = proportionofregular * GrossSales;

            var VatExToSC = GrossSalesToSC / (1 + (CDec2(12) / 100)); //fix ang 12% kay wla pa ko query kung asa makuha ang taxpercent
            var seniorCitizenDiscountRate = parseFloat(20) * 0.01; //fix ang 20% kay wla pa ko query kung asa makuha ang taxpercent

            var seniorCitizenDiscountBasedOnVatEx = CDec2(VatExToSC * seniorCitizenDiscountRate);

            var seniorCitizenNetAmount = VatExToSC - seniorCitizenDiscountBasedOnVatEx;

            var totalBillToBePayed = seniorCitizenNetAmount + GrossSalesToNoneSC;

            var weightedSCDiscountRate = proportionOfSeniors * seniorCitizenDiscountRate;

            //((GrossSalesToSC - VatExToSC) + seniorCitizenDiscountBasedOnVatEx) = total discount applied to the whole bill
            var TotalDiscount = ((GrossSalesToSC - VatExToSC) + seniorCitizenDiscountBasedOnVatEx)
            var avePerBill = parseFloat(GrossSales / TotalDiscount);
            
            //var mSubTotal = GetSSDItemsWithVatExempt(pInvoiceID);
            //VatableAmount = CDec2(dblSubTotal) - CDec2(mSubTotal);

            //dblTotalAmount = isSeniorCitizenDiscount ? CDec2(mSubTotal) / (1 + (CDec2(taxPercent) / 100)) : CDec2(dblSubTotal);

            //var proportionOfSeniors = parseFloat($.trim($("#txtSalesPayNoOfSC").val())) / parseFloat($.trim($("#txtSalesPayNoOfDiners").val()));
            //var GrossSales = mSubTotal;
            //var GrossSalesToSC = proportionOfSeniors * GrossSales;
            //var IsApplyMaxSCDiscountableAmount = false;
            //var proportionofregular = (parseFloat($.trim($("#txtSalesPayNoOfDiners").val())) - parseFloat($.trim($("#txtSalesPayNoOfSC").val()))) / parseFloat($.trim($("#txtSalesPayNoOfDiners").val()));
            //GrossSalesToNoneSC = proportionofregular * GrossSales;

            //VatExToSC = GrossSalesToSC / (1 + (CDec2(taxPercent) / 100));
            //var seniorCitizenDiscountRate = parseFloat($('#txtDiscountPercentCeiling').val()) * 0.01;

            //var MaxSCDiscountableAmount;

            //MaxSCDiscountableAmount = getMaxSCDiscountableAmount();

            //if (MaxSCDiscountableAmount != 0) {
            //    if (MaxSCDiscountableAmount < VatExToSC) {
            //        IsApplyMaxSCDiscountableAmount = true;
            //    }
            //} else {

            //}

            //var seniorCitizenDiscountBasedOnVatEx = (IsApplyMaxSCDiscountableAmount == true ? MaxSCDiscountableAmount : VatExToSC) * seniorCitizenDiscountRate;

            ////VatExToSC should always be the Gross VAT-Ex Amount(variable VatExToSC) even if MaxSCDiscountableAmount is present
            //seniorCitizenNetAmount = VatExToSC - seniorCitizenDiscountBasedOnVatEx;

            //var totalBillToBePayed = seniorCitizenNetAmount + GrossSalesToNoneSC;

            //var weightedSCDiscountRate = proportionOfSeniors * seniorCitizenDiscountRate;

            ////compute the tax sales first
            //pIU = new InsertUpdate('private.TransInvoiceItems');

            //if (vData.CashAmount != undefined) {
            //    pIU.Add('VATExemptSale', CDec2(totalBillToBePayed + seniorCitizenDiscountBasedOnVatEx));
            //} else {
            //    pIU.Add('VATExemptSale', "(Subtotal) / (cast((100 + TaxPercent) as float)/100)");
            //}

            ////zero dpat.
            //pIU.Add('TaxSales', '0');
            //pIU.Add('TaxAmount', '0');
            //pIU.Add('ZeroRatedSale', '0');
            //pIU.Add('NonVATSale', '0');
            //pIU.Add('VATableSale', '0');

            //pSql = pIU.GetUpdate('OriginalUnitPrice = UnitPrice' +
            //    "AND ID IN(" +
            //            "Select ID " +
            //                " from TransInvoiceItems " +
            //                " inner join LibraryOrganizationPIM " +
            //                " ON LibraryOrganizationPIM.ProductID = TransInvoiceItems.ProductID " +
            //                " where InvoiceID = " + ReadyString(vData.InvoiceID) +
            //                " AND LibraryOrganizationPIM.IsVatExempt=1" +
            //            ")"
            //    );
            //Sql.ExecuteSql(pSql);

            //pIU = new InsertUpdate('private.TransInvoiceItems');
            
            //pIU.Add('DiscountTax', '0');

            //if (vData.CashAmount != undefined) {
            //    pIU.Add('DiscountAmount', seniorCitizenDiscountBasedOnVatEx); //"(SubTotal / (1+(cast((TaxPercent) as float)/100))) - (SubTotal / (1+(cast((TaxPercent) as float)/100)) * (cast((100 - " + CDec2(vDiscountIndex.DiscountPercent) + ") as float) /100))");
            //    pIU.Add('DiscountSales', '0'); //just make this 0
            //} else {
            //    pIU.Add('DiscountAmount', "VATExemptSale - (VATExemptSale * (cast((100 - " + CDec2(vDiscountIndex.DiscountPercent) + ") as float) /100))");
            //    pIU.Add('DiscountSales', '0'); //just make this 0
            //}

            //pSql = pIU.GetUpdate('InvoiceID = ' + ReadyString(vData.InvoiceID) + ' AND OriginalUnitPrice = UnitPrice');
            //Sql.ExecuteSql(pSql);

        };

        //for PWD 
        this.SaveInvoiceItemTaxAndDiscountPWD = function (vData, vDiscountIndex) {

            //compute the tax sales first
            //pIU = new InsertUpdate('private.TransInvoiceItems');
            //pIU.Add('VATExemptSale', "(Subtotal) / (cast((100 + TaxPercent) as float)/100)");
            ////zero dpat.
            //pIU.Add('TaxSales', '0');
            //pIU.Add('TaxAmount', '0');
            //pIU.Add('ZeroRatedSale', '0');
            //pIU.Add('NonVATSale', '0');
            //pIU.Add('VATableSale', '0');

            //pSql = pIU.GetUpdate('InvoiceID = ' + ReadyString(vData.InvoiceID) + ' AND OriginalUnitPrice = UnitPrice');
            //Sql.ExecuteSql(pSql);

            //pIU = new InsertUpdate('private.TransInvoiceItems');
            //pIU.Add('DiscountAmount', "VATExemptSale - (VATExemptSale * (cast((100 - " + CDec2(vDiscountIndex.DiscountPercent) + ") as float) /100))");
            //pIU.Add('DiscountSales', '0'); //just make this 0
            //pIU.Add('DiscountTax', '0');
            //pSql = pIU.GetUpdate('InvoiceID = ' + ReadyString(vData.InvoiceID) + ' AND OriginalUnitPrice = UnitPrice');
            //Sql.ExecuteSql(pSql);
            
            var TotalDiscount;
            var SubTotalNetOfVAT;
            var NewSubTotal;

            var proportionOfSeniors = parseFloat(vData.NoOfSeniorCitizens) / parseFloat(vData.NoOfPeople);
            var GrossSales = parseFloat(vData.CashAmount) + parseFloat(vData.CheckPaymentAmount) + parseFloat(vData.CreditCardAmount) + parseFloat(vData.DebitCardAmount) + parseFloat(vData.OtherTypeAmount) + parseFloat(vData.OnCreditAmount);
            var GrossSalesToSC = proportionOfSeniors * GrossSales;
            
            var proportionofregular = (parseFloat(vData.NoOfPeople) - parseFloat(vData.NoOfSeniorCitizens)) / parseFloat(vData.NoOfPeople);
            var GrossSalesToNoneSC = proportionofregular * GrossSales;

            var VatExToSC = GrossSalesToSC / (1 + (CDec2(12) / 100)); //fix ang 12% kay wla pa ko query kung asa makuha ang taxpercent
            var seniorCitizenDiscountRate = parseFloat(20) * 0.01; //fix ang 20% kay wla pa ko query kung asa makuha ang taxpercent

            var seniorCitizenDiscountBasedOnVatEx = VatExToSC * seniorCitizenDiscountRate;

            var seniorCitizenNetAmount = VatExToSC - seniorCitizenDiscountBasedOnVatEx;

            var totalBillToBePayed = seniorCitizenNetAmount + GrossSalesToNoneSC;

            var weightedSCDiscountRate = proportionOfSeniors * seniorCitizenDiscountRate;

            //((GrossSalesToSC - VatExToSC) + seniorCitizenDiscountBasedOnVatEx) = total discount applied to the whole bill
            var TotalDiscount = ((GrossSalesToSC - VatExToSC) + seniorCitizenDiscountBasedOnVatEx)
            var avePerBill = CDec2(parseFloat(GrossSales / TotalDiscount));

            //compute the tax sales first
            pIU = new InsertUpdate('private.TransInvoiceItems');
            pIU.Add('VATExemptSale', "0"); 
            
            pIU.Add('TaxSales', '(SubTotal / (1+(cast((TaxPercent) as float)/100)) * (cast((100 - ' + CDec2(vDiscountIndex.DiscountPercent) + ') as float) /100))'); // P80            
            pIU.Add('VATableSale','(SubTotal / (1+(cast((TaxPercent) as float)/100)) * (cast((100 - ' + CDec2(vDiscountIndex.DiscountPercent) + ') as float) /100))  * (1+(cast((TaxPercent) as float)/100))'); // P89.40

            pIU.Add('ZeroRatedSale', '0');
            pIU.Add('NonVATSale', '0');

            pSql = pIU.GetUpdate('InvoiceID = ' + ReadyString(vData.InvoiceID) + ' AND OriginalUnitPrice = UnitPrice');
            Sql.ExecuteSql(pSql);

            pIU = new InsertUpdate('private.TransInvoiceItems');
            pIU.Add('TaxAmount', '((SubTotal / (1+(cast((TaxPercent) as float)/100)) * (cast((100 - ' + CDec2(vDiscountIndex.DiscountPercent) + ') as float) /100))  * (1+(cast((TaxPercent) as float)/100))) - TaxSales'); // P9.40 ..placed here because you "TaxSales" need to be saved first before you can compute for this
            pIU.Add('DiscountAmount', "(SubTotal /" + avePerBill + ')'); //"(SubTotal / (1+(cast((TaxPercent) as float)/100))) - (SubTotal / (1+(cast((TaxPercent) as float)/100)) * (cast((100 - " + CDec2(vDiscountIndex.DiscountPercent) + ") as float) /100))");
            pIU.Add('DiscountSales', "(SubTotal /" + avePerBill + ')');  //"(SubTotal / (1+(cast((TaxPercent) as float)/100))) - (SubTotal / (1+(cast((TaxPercent) as float)/100)) * (cast((100 - " + CDec2(vDiscountIndex.DiscountPercent) + ") as float) /100))"); //just make this 0
            pIU.Add('DiscountTax', '0');
            pSql = pIU.GetUpdate('InvoiceID = ' + ReadyString(vData.InvoiceID) + ' AND OriginalUnitPrice = UnitPrice');
            Sql.ExecuteSql(pSql);
            
        };

        this.SaveBillingCharges = function (vData) {
            //Save Discounts

            var vCharges = vData.SalesPaymentChargeList;
            for (var i = 0; i < vCharges.length; i++) {

                pSql = "SELECT ID FROM private.TransInvoiceCharges " +
                    " Where InvoiceID = " + ReadyString(vData.InvoiceID) + " AND ChargeType = " + ReadyString(vCharges[i].ChargeType);

                var pDT = Sql.Execute(pSql);

                pIU = new InsertUpdate('private.TransInvoiceCharges');
                pIU.Add('InvoiceID', ReadyString(vData.InvoiceID));
                pIU.Add('ChargeType', ReadyString(vCharges[i].ChargeType));
                pIU.Add('ChargeAmount', CDec2(vCharges[i].ChargeAmount));
                pIU.Add('TipAmount', CDec2(vCharges[i].TipAmount));
                pIU.Add('TaxAmount', CDec2(vCharges[i].TaxAmount));
                pIU.Add('Remarks', ReadyString(vCharges[i].Remarks));
                //if existing , update

                if (pDT.Rows.length > 0)
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(pDT.Rows[0].ID));
                else {
                    pIU.Add('ID', ReadyString(NewID()));
                    pSql = pIU.GetInsert();
                }

                Sql.ExecuteSql(pSql);

            }
            return 'ok';

        };

        this.AddDefaultServiceCharge = function(vInvoiceID) {
            if (!Business.SystemDefaults.IsAutoServiceCharge())
                return;

            var pInvoiceAmount = Business.TransInvoice.GetTotalAmount(vInvoiceID);
            var pData = {
                InvoiceID: vInvoiceID,
                SalesPaymentChargeList: [
                    {
                        ChargeAmount: pInvoiceAmount * Business.SystemDefaults.ServiceCharge() / 100,
                        ChargeType: "Service Charge"
                    }
                ]
            };
            this.DeleteBillCharges({
                InvoiceID: vInvoiceID,
                ChargeType: "Service Charge"
            });
            SaveBillingCharges(pData);

        };

        this.DeleteBillCharges = function (vData) {
            try {
                var pSql = "DELETE FROM private.TransInvoiceCharges " +
                        " Where InvoiceID = " + ReadyString(vData.InvoiceID) + " AND ChargeType = " + ReadyString(vData.ChargeType);

                Sql.ExecuteSql(pSql);

                return 'ok';

            } catch (ex) {
                return ex;
            }

        };
        this.GetBillCharges = function (invID) {
            var pSql = "SELECT ID, InvoiceID, ChargeType, Remarks,ROUND(IFNULL(ChargeAmount,0), 4) as ChargeAmount , ROUND(IFNULL(TaxAmount,0), 4) as LocalTaxAmount, ROUND(IFNULL(TipAmount,0), 4) as TipAmount  FROM private.TransInvoiceCharges " +
                    " Where ChargeType <> 'Tip' AND InvoiceID = " + ReadyString(invID);

            var pDT = Sql.Execute(pSql);
            return pDT;
        };
        this.GetBillTip = function (invID) {
            var pSql = "SELECT ID, InvoiceID, ChargeType, Remarks, ROUND(IFNULL(TipAmount,0), 4) as TipAmount FROM private.TransInvoiceCharges " +
                    " Where ChargeType = 'Tip' AND InvoiceID = " + ReadyString(invID);

            var pDT = Sql.Execute(pSql);
            return pDT;
        };

        this.getCurrentDiscount = function (vInvoiceID) {

            pSql = "SELECT invDisc.DiscountTypeID , invDisc.DiscountPercentage, invDisc.DiscountRemarks," +
            " invDisc.NoOfPeople, invDisc.NoOfSeniorCitizens ,IFNULL(disc.PercentDiscount,0) as PercentDiscount" +
                    " FROM private.TransInvoiceDiscount invDisc INNER JOIN private.LibraryDiscountSettings disc ON invDisc.DiscountTypeID = disc.ID " +
                    " WHERE invDisc.InvoiceID = " + ReadyString(vInvoiceID);

            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            return pDT;

        };

        this.saveSeniorCitizenPWDCustomer = function (vCustomerID, vInvoiceID, IDImage) {
            try {
                var pSql = "SELECT ID " +
                        " FROM private.TransInvoiceSeniorCitizenPWDCustomer " +
                        " WHERE CustomerID = " + ReadyString(vCustomerID) + " AND InvoiceID = " + ReadyString(vInvoiceID);

                var pDT = Sql.Execute(pSql);

                if (pDT != undefined) {
                    if (pDT.Rows.length < 1) {
                        pIU = new InsertUpdate('private.TransInvoiceSeniorCitizenPWDCustomer');

                        if (CStr2(IDImage)!=='')
                            pIU.Add('IDImage', ReadyString(IDImage));

                        pIU.Add('CustomerID', ReadyString(vCustomerID));
                        pIU.Add('InvoiceID', ReadyString(vInvoiceID));
                        pIU.Add('ID', ReadyString(NewID()));

                        pSql = pIU.GetInsert();

                        Sql.ExecuteSql(pSql);
                    }
                }

                return 'ok';
            } catch (ex) {
                return ex.Message;

            }
        };

        this.getSeniorCitizenPWDCustomer = function (vInvoiceID) {

            pSql = "SELECT cus.ID, cus.CustomerID, title.Code as TitleName, org.PersonFullname as Fullname, org.PersonSurname as Surname, org.PersonFirstName as FirstName, org.PersonMiddleName as MiddleName, org.PersonDateOfBirth as Birthdate, org.PersonSuffixName as SuffixName, IFNULL(org.PersonSeniorCitizensIDNo, '') || '' || IFNULL(org.PersonPWDIDNo, '') || '' || IFNULL(PersonOtherIDNo, '')  as IDNo, TinNumber, ChildName, ChildBirthdate, ChildAge " +
                    " FROM private.TransInvoiceSeniorCitizenPWDCustomer cus LEFT JOIN LibraryOrganization org ON cus.CustomerID = org.ID " +
                    " LEFT JOIN LibraryPeoplePreNominalSuffix title ON org.PersonTitleID = title.ID WHERE cus.InvoiceID = " + ReadyString(vInvoiceID);


            var pDT = Sql.Execute(pSql);

            if (pDT == undefined) return null;

            return pDT;

        };

        //for senior citizen pwd getOrganizationInfoBilling
        this.getOrganizationInfoBilling = function (id) {
            var pOut = {};
            var pDT;
            var pSql;

            if (CStr2(id) != '') {
                pSql = "SELECT org.ID, IsOrganization, org.PersonTitleID, orgpresuffix.Code as PersonTitle" +
                    " ,PersonSurname, PersonFirstName,PersonMiddleName, PersonSuffixName" +
                    " ,PersonDateOfBirth, PersonSeniorCitizensIDNo, IFNULL(PersonFullname, NULLIF(TRIM(PersonFirstName) || ' ','') || NULLIF(TRIM(PersonMiddleName) || ' ','') || NULLIF(TRIM(PersonSurname) || ' ','')) as PersonFullname " +
                    " ,PersonPWDIDNo, PersonPassportNo,PersonDriversLicenseNo, PersonSSSNo" +
                    " ,PersonGSISIDNo, PersonPRCIDNo,PersonPostalIDNo, PersonVotersIDNo , PersonOtherIDType, PersonOtherIDNo" +
                    " FROM LibraryOrganization org" +
                    " LEFT JOIN private.TransInvoiceSeniorCitizenPWDCustomer cus ON org.ID = cus.CustomerID " +
                    " LEFT JOIN LibraryPeoplePreNominalSuffix orgpresuffix ON (org.PersonTitleID = orgpresuffix.ID)" +
                    " WHERE cus.ID = " + ReadyString(id);

            }

            pSql += ' ORDER BY PersonSurname, PersonFirstName,PersonMiddleName ';
            pDT = Sql.Execute(pSql);

            return { 'OrganizationData': pDT };

        }; // getOrganizationInfoBilling

        //for senior citizen pwd incomplete temp.
        this.getOrganizationInfo = function (id) {
            var pOut = {};
            var pDT;
            var pSql;

            if (CStr2(id) != '') {
                pSql = "SELECT org.ID, IsOrganization, org.PersonTitleID, orgpresuffix.Code as PersonTitle" +
                    " ,PersonFullname ,PersonSurname, PersonFirstName,PersonMiddleName, PersonSuffixName" +
                    " ,org.PersonDateOfBirth, PersonSeniorCitizensIDNo " +
                    " ,PersonPWDIDNo, PersonPassportNo,PersonDriversLicenseNo, PersonSSSNo" +
                    " ,PersonGSISIDNo, PersonPRCIDNo,PersonPostalIDNo, PersonVotersIDNo , PersonOtherIDType, PersonOtherIDNo" +
                    " FROM LibraryOrganization org" +
                    " LEFT JOIN LibraryPeoplePreNominalSuffix orgpresuffix ON (org.PersonTitleID = orgpresuffix.ID)" +
                    " WHERE org.ID = " + ReadyString(id);

            }

            pSql += ' ORDER BY PersonSurname, PersonFirstName,PersonMiddleName ';
            pDT = Sql.Execute(pSql);

            return { 'OrganizationData': pDT };

        }; // getOrganizationInfo



        this.removeSeniorCitizenPWDCustomer = function (id) {

            var pSql = "DELETE FROM private.TransInvoiceSeniorCitizenPWDCustomer " +
                    " WHERE ID = " + ReadyString(id);

            Sql.ExecuteSql(pSql);

            return 'ok';
        };

        this.removeSeniorCitizenPWDCustomerIdentification = function (id, type) {
            var vType = '';
            var pSql;
            var pIU = new InsertUpdate('LibraryOrganization');

            switch (type) {
                case "Senior Citizens' ID":
                    pIU.Add('PersonSeniorCitizensIDNo', 'NULL');
                    break;
                case "Persons With Disabilities (PWD) ID":
                    pIU.Add('PersonPWDIDNo', 'NULL');
                    break;
                case "Passport":
                    pIU.Add('PersonPassportNo', 'NULL');
                    break;
                case "Driver's License":
                    pIU.Add('PersonDriversLicenseNo', 'NULL');
                    break;
                case "SSS ID":
                    pIU.Add('PersonSSSNo', 'NULL');
                    break;
                case "GSIS ID":
                    pIU.Add('PersonGSISIDNo', 'NULL');
                    break;
                case "PRC ID":
                    pIU.Add('PersonPRCIDNo', 'NULL');
                    break;
                case "Postal ID":
                    pIU.Add('PersonPostalIDNo', 'NULL');
                    break;
                case "Voter's ID":
                    pIU.Add('PersonVotersIDNo', 'NULL');
                    break;
                case "Other":
                    pIU.Add('PersonOtherIDType', 'NULL');
                    pIU.Add('PersonOtherIDNo', 'NULL');
                    break;
            }


            pSql = pIU.GetUpdate('ID = ' + ReadyString(id));


            Sql.ExecuteSql(pSql);

            return 'ok';
        };

        this.getFowstPrinterList = function (fowstID) {

            pSql = "SELECT ID, PrinterIPAddress, PrinterName" +
                " FROM private.LibraryDevicePrinterFowst " +
                " WHERE FowstID = " + ReadyString(fowstID);

            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            return pDT;

        }
        this.SaveFOWSTPrinterIP = function (vData) {


            try {
                pIU = new InsertUpdate('private.LibraryDevicePrinterFowst');
                pIU.Add('FowstID', ReadyString(vData.FowstID));
                pIU.Add('PrinterID', ReadyString(vData.PrinterID));
                pIU.Add('PrinterIPAddress', ReadyString(vData.PrinterIPAddress));
                pIU.Add('PrinterName', ReadyString(vData.PrinterName));
                pIU.Add('FlushToServer', '1'); //need to flush
                var strReturn = NewID();
                pIU.Add('ID', ReadyString(strReturn));
                pSql = pIU.GetInsert();

                Sql.ExecuteSql(pSql);

                return strReturn;

            } catch (ex) {

                return '';
            }

        }
        this.CheckUniquePrinterIPAddress = function (vData) {

            pSql = "SELECT ID" +
                " FROM private.LibraryDevicePrinterFowst " +
                " WHERE FowstID = " + ReadyString(vData.FowstID) +
                " AND PrinterID = " + ReadyString(vData.PrinterID) +
                " AND PrinterIPAddress = " + ReadyString(vData.PrinterIPAddress);

            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            return pDT;

        }
        this.CheckUniquePrinterName = function (vData) {

            pSql = "SELECT ID" +
                " FROM private.LibraryDevicePrinterFowst " +
                " WHERE FowstID = " + ReadyString(vData.FowstID) +
                " AND PrinterName = " + ReadyString(vData.PrinterName);

            var pDT = Sql.Execute(pSql);
            if (pDT == undefined) return null;

            return pDT;

        }
        this.AssignFowstPrinterIPAsDefault = function (printerFowstID, devFowstPrinterID) {

            pIU = new InsertUpdate('private.LibraryDeviceFowstPrinter');
            pIU.Add('PrinterFowstID', ReadyString(printerFowstID));
            pIU.Add('FlushToServer', '1');

            pSql = pIU.GetUpdate('ID = ' + ReadyString(devFowstPrinterID));

            Sql.ExecuteSql(pSql);

            return 'ok';

        }
  
        this.UpdateSalesProductCount = function (vID) {
            var pSql = '';
            var vDataInvoiceItems;
            var pData;

            pSql = "Select ProductName, InvoiceDate, ProductID, Quantity, UnitMeasure, (Quantity * UnitPrice) as Price " +
                    "FROM TransInvoiceItems invitem " +
                    "inner join TransInvoice on TransInvoice.ID = invitem.InvoiceID " +
                    "inner join LibraryPIMProduct on LibraryPIMProduct.ID = invitem.ProductID " +
                    "Where invitem.InvoiceID = " + ReadyString(vID)

            vDataInvoiceItems = Sql.Execute(pSql);

            for (var i = 0; i <= vDataInvoiceItems.Rows.length - 1; i++) {

                pSql = 'Select * from TransSalesProductCount where ProductID = ' + ReadyString(vDataInvoiceItems.Rows[i].ProductID) + ' AND date(TransactionDate) =  ' + ReadyString($.format.date(new Date(vDataInvoiceItems.Rows[i].InvoiceDate), 'yyyy-MM-dd'))

                pData = Sql.Execute(pSql);

                if (pData.Rows.length == 0) {
                    pIU = new InsertUpdate('TransSalesProductCount');

                    pIU.Add('ID', ReadyString(NewID()))
                    pIU.Add('ProductID', ReadyString(vDataInvoiceItems.Rows[i].ProductID));
                    pIU.Add('ProductDescription', ReadyString(vDataInvoiceItems.Rows[i].ProductName));
                    pIU.Add('Qty', vDataInvoiceItems.Rows[i].Quantity);
                    pIU.Add('TotalSales', vDataInvoiceItems.Rows[i].Price);
                    pIU.Add('UnitMeasure', ReadyString(vDataInvoiceItems.Rows[i].UnitMeasure));
                    pIU.Add('TransactionDate', ReadyString(vDataInvoiceItems.Rows[i].InvoiceDate));

                    pSql = pIU.GetInsert();

                } else {

                    pSql = "Update TransSalesProductCount set Qty = Qty +  " + CDec2(vDataInvoiceItems.Rows[i].Quantity) +
                            ",  TotalSales = TotalSales +  " + CDec2(vDataInvoiceItems.Rows[i].Price) +
                            " WHERE ID = " + ReadyString(pData.Rows[0].ID);

                }
                Sql.ExecuteSql(pSql);
            }

            
        }

        this.UpdateOtherPaymentTypesTextFile = function (vID, vOtherType) {
            var pSql = '';
            var LastOtherPaymentTypesTotalData;

            pSql = "Select InvoiceDate from TransInvoice where ID = " + ReadyString(vID);

            mDate = new Date(Sql.ExecuteScalar(pSql));

            ReportDate = new Date(mDate);

            LastOtherPaymentTypesTotalData = Transfers.ReadFile('OtherPaymentTypes' + sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + ($.format.number(ReportDate.getFullYear(), '00').toString() + '-' + $.format.number((ReportDate.getMonth() + 1), '00').toString() + '-' + ReportDate.getDate().toString()) + '.dat');

            if (LastOtherPaymentTypesTotalData != undefined) {
                
                LastOtherPaymentTypesTotalData = JSON.parse(LastOtherPaymentTypesTotalData);

                for (var i = 0; i < vOtherType.length; i++) {

                    if (LastOtherPaymentTypesTotalData.hasOwnProperty(vOtherType[i].PaymentType)) {
                        LastOtherPaymentTypesTotalData[vOtherType[i].PaymentType] += CDec2(vOtherType[i].Amount);
                    } else {
                        LastOtherPaymentTypesTotalData[vOtherType[i].PaymentType] = vOtherType[i].Amount;
                    }

                }

            } else {

                LastOtherPaymentTypesTotalData = {};

                for (var i = 0; i < vOtherType.length; i++) {
                    LastOtherPaymentTypesTotalData[vOtherType[i].PaymentType] = CDec2(vOtherType[i].Amount);
                }
            }

            Transfers.WriteFile('OtherPaymentTypes' + sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + ($.format.number(ReportDate.getFullYear(), '00').toString() + '-' + $.format.number((ReportDate.getMonth() + 1), '00').toString() + '-' + ReportDate.getDate().toString()) + '.dat', null,
                                LastOtherPaymentTypesTotalData);

        }

        this.UpdateSalesTextFile = function (vID) {
            var dblTotalSales = 0;
            var dblTotalSalesDiscount = 0;
            var dblTotalForeGoneVAT = 0;
            var dblTotalNumberOfCustomers = 0;
            var dblAdjRefund = 0;
            var dblAdjSRV = 0;
            var dblNetSales = 0;

            var dblCash = 0;
            var dblCheck = 0;
            var dblDebitCard = 0;
            var dblCreditCard = 0;
            var dblSRV = 0;
            var dblOtherPaymentType = 0;
            var dblOnCredit = 0;
            var dblCharges = 0;
            var dblTotalPayment = 0;

            var dblVAT = 0;
            var dblVATAmount = 0;
            var dblNonVAT = 0;
            var dblVATExempt = 0;
            var dblZeroRatedVAT = 0;
            var dblTotalVAT = 0;

            var dblTotalVATSales = 0;
            var dblTotalVATAmount = 0;
            var dblTotalVATSalesSalesReturn = 0;
            var dblTotalVATAmountSalesReturn = 0;

            var dblTotalSalesMSR = 0;
            var dblTotalSalesDiscountMSR = 0;
            var dblTotalForeGoneVATMSR = 0;
            var dblAdjRefundMSR = 0;
            var dblAdjSRVMSR = 0;
            var dblNetSalesMSR = 0;

            var dblCashMSR = 0;
            var dblCheckMSR = 0;
            var dblDebitCardMSR = 0;
            var dblCreditCardMSR = 0;
            var dblSRVMSR = 0;
            var dblOtherPaymentTypeMSR = 0;
            var dblOnCreditMSR = 0;
            var dblChargesMSR = 0;
            var dblTotalPaymentMSR = 0;
            var dblOtherPaymentAmountMSR = 0;

            var dblVATMSR = 0;
            var dblVATAmountMSR = 0;
            var dblNonVATMSR = 0;
            var dblVATExemptMSR = 0;
            var dblZeroRatedVATMSR = 0;
            var dblTotalVATMSR = 0;

            var dblTotalVATSalesMSR = 0;
            var dblTotalVATAmountMSR = 0;
            var dblTotalVATSalesSalesReturnMSR = 0;
            var dblTotalVATAmountSalesReturnMSR = 0;

            var dblAlipay = 0;
            var dblComplimentary = 0;
            var dblDiscountVoucher = 0;
            var dblEPlus = 0;
            var dblFoodPanda = 0;
            var dblGCash = 0;
            var dblGoldCard = 0;
            var dblGrabFood = 0;
            var dblLocalDelivery = 0;
            var dblPayMaya = 0;
            var dblShopeeEVoucher = 0;
            var dblShopeePay = 0;
            var dblSMAC = 0;
            var dblVPlus = 0;

            var dblAlipayMSR = 0;
            var dblComplimentaryMSR = 0;
            var dblDiscountVoucherMSR = 0;
            var dblEPlusMSR = 0;
            var dblFoodPandaMSR = 0;
            var dblGCashMSR = 0;
            var dblGoldCardMSR = 0;
            var dblGrabFoodMSR = 0;
            var dblLocalDeliveryMSR = 0;
            var dblPayMayaMSR = 0;
            var dblShopeeEVoucherMSR = 0;
            var dblShopeePayMSR = 0;
            var dblSMACMSR = 0;
            var dblVPlusMSR = 0;

            var mDate; //= new Date();
            var ReportDate;

            var vDataInvoice;
            var vDataPayments;
            
            var TransactionItemsCount = 0;

            var pSql = '';
            var IsOtherPaymentType = false;

            pSql = "Select InvoiceDate from TransInvoice where ID = " + ReadyString(vID);

            mDate = new Date(Sql.ExecuteScalar(pSql));

            ReportDate = new Date(mDate);

            pSql = "Select count(*) from TransInvoiceItems where InvoiceID = " + ReadyString(vID);

            TransactionItemsCount = CInt2(Sql.ExecuteScalar(pSql))

            //ORIGINAL QUERY by sir francis h.
            //pSql = "Select SUM(case when quantity=(0) then (0) when (((ifnull(vatablesale,(0))+ifnull(vatexemptsale,(0)))+ifnull(zeroratedsale,(0)))+ifnull(nonvatsale,(0)))=(0) then ifnull(subtotal,(0)) else ((ifnull(vatablesale,(0))+ifnull(vatexemptsale,(0)))+ifnull(zeroratedsale,(0)))+ifnull(nonvatsale,(0)) end) CurGrossSales, " +
            //        "SUM(IFNULL(invitem.DiscountAmount,0)) + inv.SCDiscount CurSalesDiscount, " +
            //        "inv.ForeGoneVAT CurVATExemptTax, " +
            //        "SUM(IFNULL(invitem.TaxSales,0) - IFNULL(invitem.DiscountSales,0)) CurTotalVATSales, " +
            //        "SUM(IFNULL(invitem.TaxAmount,0)- IFNULL((case when vatexemptsale>(0) then (0) else invitem.DiscountAmount-invitem.DiscountSales end),0)) CurTotalVATAmount, " +
            //        "SUM(invitem.NonVATSale) CurNoTAXSales, " +
            //        "SUM(invitem.VATExemptSale) CurVATExemptSales, " +
            //        "SUM(invitem.ZeroRatedSale) CurZeroRatedVATSales " +
            //        "FROM TransCustomerPaymentInvoices custpayinv " +
            //        " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
            //        " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
            //        " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " +
            //        "Where inv.ID = " + ReadyString(vID)

            //NEW QUERY, modified by arnie on May 5,2021 - ang subquery sa CurTotalVATSales, CurTotalVATAmount, CurNoTAXSales, CurVATExemptSales, CurZeroRatedVATSales kay gikan sa ReportPOS/getCashierSalesTransactionSummary kay tama man daw ang naa sa online
            pSql = "Select SUM(case when quantity=(0) then (0) when (((ifnull(vatablesale,(0))+ifnull(vatexemptsale,(0)))+ifnull(zeroratedsale,(0)))+ifnull(nonvatsale,(0)))=(0) then ifnull(subtotal,(0)) else ((ifnull(vatablesale,(0))+ifnull(vatexemptsale,(0)))+ifnull(zeroratedsale,(0)))+ifnull(nonvatsale,(0)) end) CurGrossSales, \n " +
                    "SUM(IFNULL(invitem.DiscountAmount,0)) + IFNULL(inv.SCDiscount, 0) + IFNULL(inv.SoloParentDiscount, 0) CurSalesDiscount, \n " +
                    "inv.ForeGoneVAT CurVATExemptTax, \n " +
                    "inv.InvoiceNumber InvoiceNo, \n " +
                    "inv.SCDiscount CurSCDiscount, \n " +
                    "inv.SCDiscountExempt CurSCDiscountExempt, \n " +
                    "inv.SoloParentDiscount CurSoloParentDiscount, \n " +
                    "inv.PhNatTeamDiscount CurPhNatTeamDiscount, \n " +
                    "IFNULL((SELECT  \n " +
                    "   SUM(IFNULL(invitems.TaxSales,0) - IFNULL(invitems.DiscountSales,0)) \n " +
                    "   FROM --TransCustomerPaymentInvoices custpayinvs \n " +
                    "   --INNER JOIN TransCustomerPayment custpays ON (custpayinvs.CustomerPaymentID = custpays.ID) \n " +
                    "   --INNER JOIN " +
                    "   \n TransInvoice invs --ON (custpayinvs.InvoiceID = invs.ID) \n " +
                    "   \n INNER JOIN TransInvoiceItems invitems ON (invitems.InvoiceID = invs.ID) \n " +
                    "   WHERE invs.InvoiceNumber > 0 \n " +
                    "   -- AND custpays.ReceiptType <> 'RF' \n " +
                    "   \n AND IFNULL(invs.SCDiscount, 0) = 0 AND IFNULL(invs.SoloParentDiscount, 0) = 0 \n " +
                    "   AND invs.ID = inv.ID \n " +
                    "), 0) CurTotalVATSales, \n " +
                    " IFNULL((SELECT invs.Vatable " +
                    "   FROM TransInvoice invs " +
                    "   WHERE invs.InvoiceNumber > 0 AND invs.ForegoneVAT>0" +
                    "   AND invs.ID = inv.ID) " +
                    ",0) CurTotalVatablePortionOfSCDTransaction, " +
                    " IFNULL((SELECT IFNULL(invs.VAT, 0) " +
                    "   FROM TransInvoice invs " +
                    "   WHERE invs.InvoiceNumber > 0 AND invs.ForegoneVAT>0" +
                    "   AND invs.ID = inv.ID) " +
                    ",0) CurTotalVATAmountPortionOfSCDTransaction, " +
                    "IFNULL((SELECT  \n " +
                    "   SUM(IFNULL(invitems.TaxAmount,0)- IFNULL((case when invitems.vatexemptsale>(0) then (0) else invitems.DiscountAmount-invitems.DiscountSales end),0)) \n " +
                    "   FROM TransCustomerPaymentInvoices custpayinvs \n " +
                    "   INNER JOIN TransCustomerPayment custpays ON (custpayinvs.CustomerPaymentID = custpays.ID) \n " +
                    "   INNER JOIN TransInvoice invs ON (custpayinvs.InvoiceID = invs.ID) \n " +
                    "   INNER JOIN TransInvoiceItems invitems ON (invitems.InvoiceID = invs.ID) \n " +
                    "   WHERE invs.InvoiceNumber > 0 \n " +
                    "   AND custpays.ReceiptType <> 'RF' \n " +
                    "   AND IFNULL(invs.SCDiscount, 0) = 0 AND IFNULL(invs.SoloParentDiscount, 0) = 0 \n " +
                    "   AND invs.ID = inv.ID \n " +
                    "), 0) CurTotalVATAmount, \n " +
                    "IFNULL((SELECT  \n " +
                    "   SUM(invitems.NonVATSale) \n " +
                    "   FROM TransCustomerPaymentInvoices custpayinvs \n " +
                    "   INNER JOIN TransCustomerPayment custpays ON (custpayinvs.CustomerPaymentID = custpays.ID) \n " +
                    "   INNER JOIN TransInvoice invs ON (custpayinvs.InvoiceID = invs.ID) \n " +
                    "   INNER JOIN TransInvoiceItems invitems ON (invitems.InvoiceID = invs.ID) \n " +
                    "   WHERE invs.ID = inv.ID \n " +
                    "), 0) CurNoTAXSales, \n " +
                    "IFNULL((SELECT  \n " +
                    "   SUM(invs.VATExempt) \n " + // - (invs.SCDiscount + invs.SoloParentDiscount) dapat dili daw ni sila ideduct apil sa VATExempt 090125
                    "   FROM TransCustomerPaymentInvoices custpayinvs \n " +
                    "   INNER JOIN TransCustomerPayment custpays ON (custpayinvs.CustomerPaymentID = custpays.ID) \n " +
                    "   INNER JOIN TransInvoice invs ON (custpayinvs.InvoiceID = invs.ID) \n " + //"   --INNER JOIN TransInvoiceItems invitems ON (invitems.InvoiceID = invs.ID) \n " +
                    "   WHERE invs.InvoiceNumber > 0 \n " +
                    "   AND custpays.ReceiptType <> 'RF' \n " +
                    "   AND invs.ID = inv.ID \n " +
                    "), 0) CurVATExemptSales, \n " +
                    "IFNULL((SELECT  \n " +
                    "   SUM(invitems.ZeroRatedSale) \n " +
                    "   FROM TransCustomerPaymentInvoices custpayinvs \n " +
                    "   INNER JOIN TransCustomerPayment custpays ON (custpayinvs.CustomerPaymentID = custpays.ID) \n " +
                    "   INNER JOIN TransInvoice invs ON (custpayinvs.InvoiceID = invs.ID) \n " +
                    "   INNER JOIN TransInvoiceItems invitems ON (invitems.InvoiceID = invs.ID) \n " +
                    "   WHERE invs.ID = inv.ID \n " +
                    "), 0) CurZeroRatedVATSales, \n " +
                    "IFNULL((SELECT  \n " +
                    "   SUM(invitems.DiscountAmount) \n " +
                    "   FROM TransCustomerPaymentInvoices custpayinvs \n " +
                    "   INNER JOIN TransCustomerPayment custpays ON (custpayinvs.CustomerPaymentID = custpays.ID) \n " +
                    "   INNER JOIN TransInvoice invs ON (custpayinvs.InvoiceID = invs.ID) \n " +
                    "   INNER JOIN TransInvoiceItems invitems ON (invitems.InvoiceID = invs.ID) \n " +
                    "   WHERE invs.InvoiceNumber > 0 AND IFNULL(invs.SCDiscount, 0) = 0 AND IFNULL(invs.SoloParentDiscount, 0) = 0 \n " +
                    "   AND DATE(invs.InvoiceDate) BETWEEN DATE(" + proj.GetZReadDateSql() + ") AND DATE(" + proj.GetZReadDateSql() + ") \n " +
                    "   AND invs.ID = inv.ID \n " +
                    "), 0) CurTotalDiscount \n " +
                    "FROM TransCustomerPaymentInvoices custpayinv \n " +
                    " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) \n " +
                    " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  \n " +
                    " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) \n " +
                    "Where inv.ID = \n " + ReadyString(vID)

            vDataInvoice = Sql.Execute(pSql);
            console.log(vDataInvoice)
            pSql = 'Select '

            if (vDataInvoice.Rows.length > 0) {
                dblTotalSales = CDec2(vDataInvoice.Rows[0].CurGrossSales)
                dblTotalSalesDiscount = CDec2(vDataInvoice.Rows[0].CurSalesDiscount);
                dblTotalForeGoneVAT = CDec2(vDataInvoice.Rows[0].CurVATExemptTax);

                dblTotalVATSales = CDec2(vDataInvoice.Rows[0].CurTotalVATSales || 0) + CDec2(vDataInvoice.Rows[0].CurTotalVatablePortionOfSCDTransaction || 0); //- parseFloat(vDataInfo.Rows[p].CurTotalVATSalesSalesReturn || 0);
                dblTotalVATAmount = CDec2(vDataInvoice.Rows[0].CurTotalVATAmount || 0) + CDec2(vDataInvoice.Rows[0].CurTotalVATAmountPortionOfSCDTransaction || 0); // - parseFloat(vDataInfo.Rows[p].CurTotalVATAmountSalesReturn || 0);
                dblNonVAT = CDec2(vDataInvoice.Rows[0].CurNoTAXSales || 0);
                dblVATExempt = CDec2(vDataInvoice.Rows[0].CurVATExemptSales || 0);
                dblZeroRatedVAT = CDec2(vDataInvoice.Rows[0].CurZeroRatedVATSales || 0);

                dblTotalSalesMSR = CDec2(vDataInvoice.Rows[0].CurGrossSales)
                dblTotalSalesDiscountMSR = CDec2(vDataInvoice.Rows[0].CurSalesDiscount);
                dblTotalForeGoneVATMSR = CDec2(vDataInvoice.Rows[0].CurVATExemptTax);

                dblTotalVATSalesMSR = CDec2(vDataInvoice.Rows[0].CurTotalVATSales || 0) + CDec2(vDataInvoice.Rows[0].CurTotalVatablePortionOfSCDTransaction || 0); //- parseFloat(vDataInfo.Rows[p].CurTotalVATSalesSalesReturn || 0);
                dblTotalVATAmountMSR = CDec2(vDataInvoice.Rows[0].CurTotalVATAmount || 0) + CDec2(vDataInvoice.Rows[0].CurTotalVATAmountPortionOfSCDTransaction || 0); // - parseFloat(vDataInfo.Rows[p].CurTotalVATAmountSalesReturn || 0);
                dblNonVATMSR = CDec2(vDataInvoice.Rows[0].CurNoTAXSales || 0);
                dblVATExemptMSR = CDec2(vDataInvoice.Rows[0].CurVATExemptSales || 0);
                dblZeroRatedVATMSR = CDec2(vDataInvoice.Rows[0].CurZeroRatedVATSales || 0);

                dblTotalNumberOfCustomers = CDec2(sessionStorage.TotalNumberOfCustomer);
            }

            pSql = "Select SUM(IFNULL(custpay.CashAmount,0) - IFNULL(custpay.CustomerChangeCash,0)) CashAmount, " +
                    "SUM(IFNULL(custpay.DebitCardAmount,0)) DebitCardAmount, " +
                    "SUM(IFNULL(custpay.CreditCardAmount,0)) CreditCardAmount, " +
                    "SUM(IFNULL(custpay.CheckPaymentAmount,0)) CheckPaymentAmount, " +
                    "SUM(IFNULL(custpay.SRVAmount,0)) SRVAmount, " +
                    "SUM(IFNULL(custpay.OtherTypeAmount,0)) OtherTypeAmount, " +
                    "SUM(IFNULL(custpay.OnCreditAmount,0)) OnCreditAmount, " +
                    "SUM(IFNULL(custpay.ChargeAmount,0)) ChargeAmount " +
                    "FROM TransCustomerPaymentInvoices custpayinv " +
                    " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                    " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                    "Where inv.ID = " + ReadyString(vID)

            var vDataPayments = Sql.Execute(pSql);

            if (vDataPayments.Rows.length > 0) {

                dblCash = parseFloat(vDataPayments.Rows[0].CashAmount || 0);
                dblCheck = parseFloat(vDataPayments.Rows[0].CheckPaymentAmount || 0);
                dblDebitCard = parseFloat(vDataPayments.Rows[0].DebitCardAmount || 0);
                dblCreditCard = parseFloat(vDataPayments.Rows[0].CreditCardAmount || 0);
                dblSRV = parseFloat(vDataPayments.Rows[0].SRVAmount || 0);
                dblOtherPaymentType = parseFloat(vDataPayments.Rows[0].OtherTypeAmount || 0);
                dblOnCredit = parseFloat(vDataPayments.Rows[0].OnCreditAmount || 0);
                dblCharges = parseFloat(vDataPayments.Rows[0].ChargeAmount || 0);

                dblCashMSR = parseFloat(vDataPayments.Rows[0].CashAmount || 0);
                dblCheckMSR = parseFloat(vDataPayments.Rows[0].CheckPaymentAmount || 0);
                dblDebitCardMSR = parseFloat(vDataPayments.Rows[0].DebitCardAmount || 0);
                dblCreditCardMSR = parseFloat(vDataPayments.Rows[0].CreditCardAmount || 0);
                dblSRVMSR = parseFloat(vDataPayments.Rows[0].SRVAmount || 0);
                dblOtherPaymentTypeMSR = parseFloat(vDataPayments.Rows[0].OtherTypeAmount || 0);
                dblOnCreditMSR = parseFloat(vDataPayments.Rows[0].OnCreditAmount || 0);
                dblChargesMSR = parseFloat(vDataPayments.Rows[0].ChargeAmount || 0);
                                
                dblOtherPaymentAmountMSR = parseFloat(vDataPayments.Rows[0].OtherTypeAmount || 0);
            }

            pSql = "SELECT other.* FROM TransCustomerPaymentInvoices as custpayinv \n " +
                    " INNER JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpayinv.CustomerPaymentID) \n " +
                    " WHERE custpayinv.InvoiceID = " + ReadyString(vID)

            var otherpayment = Sql.Execute(pSql).Rows;
            var cacheTable = ['CacheMSR', 'CacheCSR'];

            for (var i = 0; i < otherpayment.length; i++) {
                var opdata = otherpayment[i];
                opdata.PaymentType = opdata.PaymentType.replace(/\s+/g, '');

                IsOtherPaymentType = true;

                if (opdata.PaymentType == 'AliPay') {
                    dblAlipay += CDec2(opdata.Amount);
                    dblAlipayMSR += CDec2(opdata.Amount);
                } else if (opdata.PaymentType == 'Complimentary') {
                    dblComplimentary += CDec2(opdata.Amount);
                    dblComplimentaryMSR += CDec2(opdata.Amount);
                } else if (opdata.PaymentType == 'Discount Voucher / On Line Deals') {
                    dblDiscountVoucher += CDec2(opdata.Amount);
                    dblDiscountVoucherMSR += CDec2(opdata.Amount);
                } else if (opdata.PaymentType == 'E-Plus') {
                    dblEPlus += CDec2(opdata.Amount);
                    dblEPlusMSR += CDec2(opdata.Amount);
                } else if (opdata.PaymentType == 'FoodPanda') {
                    dblFoodPanda += CDec2(opdata.Amount);
                    dblFoodPandaMSR += CDec2(opdata.Amount);
                } else if (opdata.PaymentType == 'GCash') {
                    dblGCash += CDec2(opdata.Amount);
                    dblGCashMSR += CDec2(opdata.Amount);
                } else if (opdata.PaymentType == 'GoldCard') {
                    dblGoldCard += CDec2(opdata.Amount);
                    dblGoldCardMSR += CDec2(opdata.Amount);
                } else if (opdata.PaymentType == 'GrabFood') {
                    dblGrabFood += CDec2(opdata.Amount);
                    dblGrabFoodMSR += CDec2(opdata.Amount);
                } else if (opdata.PaymentType == 'LocalDelivery') {
                    dblLocalDelivery += CDec2(opdata.Amount);
                    dblLocalDeliveryMSR += CDec2(opdata.Amount);
                } else if (opdata.PaymentType == 'PayMaya') {
                    dblPayMaya += CDec2(opdata.Amount);
                    dblPayMayaMSR += CDec2(opdata.Amount);
                } else if (opdata.PaymentType == 'ShopeeEVoucher') {
                    dblShopeeEVoucher += CDec2(opdata.Amount);
                    dblShopeeEVoucherMSR += CDec2(opdata.Amount);
                } else if (opdata.PaymentType == 'ShopeePay') {
                    dblShopeePay += CDec2(opdata.Amount);
                    dblShopeePayMSR += CDec2(opdata.Amount);
                } else if (opdata.PaymentType == 'SMAC') {
                    dblSMAC += CDec2(opdata.Amount);
                    dblSMACMSR += CDec2(opdata.Amount);
                } else if (opdata.PaymentType == 'V-Plus') {
                    dblVPlus += CDec2(opdata.Amount);
                    dblVPlusMSR += CDec2(opdata.Amount);
                }

                //check if column exist in cachemsr table                
                for (var ct in cacheTable) {
                    var vData = Sql.Execute("PRAGMA cache.table_info(" + cacheTable[ct] +")");
                    var IsExist = false;

                    for (var m = 0; m <= vData.Rows.length - 1; m++) {
                        if (vData.Rows[m].name.toLowerCase() == CStr2('OtherPayment' + opdata.PaymentType).toLowerCase()) {
                            IsExist = true;
                            break;
                        }
                    }

                    if (!IsExist) {
                        Sql.Execute("ALTER TABLE cache." + cacheTable[ct] +" ADD COLUMN OtherPayment" + opdata.PaymentType + " INTEGER");
                    }
                }
            }

            var discountCols = [];
            var discountlist = Sql.Execute('SELECT ID, replace(DiscountType, " ", "") as DiscountType FROM private.LibraryDiscountSettings UNION ALL SELECT ID, replace(Name, " ", "") as DiscountType FROM private.LibraryCustomDiscounts WHERE IsActive = 1').Rows;
            var name = '';
            for (var i in discountlist) {
                if (discountlist[i].ID.toLowerCase() == '8e2dca9c-433f-42fc-a644-04476dba056a') {
                    name = 'TotalRegularDiscountThree';                    
                } else if (discountlist[i].ID.toLowerCase() == 'c2218c86-0450-4cd3-b019-3edd43e51741') {
                    name = 'TotalGPCDiscount';
                } else if (discountlist[i].ID.toLowerCase() == '34a4032f-a142-4c15-a868-4ac4410434df') {
                    name = 'TotalPWDDiscount';
                } else if (discountlist[i].ID.toLowerCase() == 'f94329ec-9453-4061-8f72-4b95ef18bf81') {
                    name = 'TotalMaxAllowableSCDiscount';
                } else if (discountlist[i].ID.toLowerCase() == '764edacb-0131-4033-8e44-7b903368008c') {
                    name = 'TotalRegularDiscountOne';
                } else if (discountlist[i].ID.toLowerCase() == 'c4199347-6a00-45c9-be11-8934dd63ecd1') {
                    name = 'TotalRegularDiscountTwo';
                } else if (discountlist[i].ID.toLowerCase() == '674ad791-188b-4238-b95c-97b5c2b9d544') {
                    name = 'TotalSpecialSCDiscount';
                } else if (discountlist[i].ID.toLowerCase() == '35f954a0-c67c-499c-af60-f08e64477fdc') {
                    name = 'TotalVIPDiscount';
                } else if (discountlist[i].ID.toLowerCase() == '7804a35a-b531-4380-945e-fc9643e99843') {
                    name = 'TotalEmployeeDiscount';
                } else if (discountlist[i].ID.toLowerCase() == '055a0a33-dd98-47ec-add3-8b2009f3944a') {
                    name = 'TotalSCDiscount';
                } else {
                    name = 'Total' + discountlist[i].DiscountType;
                }

                discountCols.push(name);
                discountlist[i].ColumnName = name              
            }

            //iadd nalang diri ang mga new columns for sqlite
            discountCols.push('CurVoid')
            discountCols.push('CurVoidItemQty')
            discountCols.push('PrintCount')
            discountCols.push('LastDateOfReprint')

            for (var ct in cacheTable) {
                var cachemsrColumns = Sql.Execute("PRAGMA cache.table_info(" + cacheTable[ct] +")");

                for (var i in discountCols) {
                    var isExist = false;
                    for (var m in cachemsrColumns.Rows) {
                        if (cachemsrColumns.Rows[m].name == discountCols[i]) {
                            isExist = true;
                            break;
                        }
                    }

                    if (!isExist) {
                        pSql = " ALTER TABLE cache." + cacheTable[ct] +" " +
                            " ADD [" + discountCols[i] + "] money"
                        Sql.Execute(pSql);
                    }
                }
            }            

            //START OF ORIGINAL CODE - dili na sa text file ginasave ang data, sa new SQLITE db na.
            //var LastTotalData = Transfers.ReadFile(sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + Web.PeopleUserID() + ($.format.number(ReportDate.getFullYear(), '00').toString() + '-' + $.format.number((ReportDate.getMonth() + 1), '00').toString() + '-' + ReportDate.getDate().toString()) + '.dat');

            //var LastTotalDataMSR = Transfers.ReadFile(sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + ($.format.number(ReportDate.getFullYear(), '00').toString() + '-' + $.format.number((ReportDate.getMonth() + 1), '00').toString() + '-' + ReportDate.getDate().toString()) + '.dat');

            //pSql = '';

            //if (LastTotalDataMSR != undefined) {
            //    var LastTotal = LastTotalDataMSR.split('|');


            //    dblTotalSalesMSR += CDec2(LastTotal[0]);
            //    dblTotalSalesDiscountMSR += CDec2(LastTotal[1]);
            //    dblAdjRefundMSR += CDec2(LastTotal[2]);
            //    dblAdjSRVMSR += CDec2(LastTotal[3]);

            //    dblCashMSR += CDec2(LastTotal[4]);
            //    dblCheckMSR += CDec2(LastTotal[5]);
            //    dblDebitCardMSR += CDec2(LastTotal[6]);
            //    dblCreditCardMSR += CDec2(LastTotal[7]);
            //    dblSRVMSR += CDec2(LastTotal[8]);
            //    dblOtherPaymentTypeMSR += CDec2(LastTotal[18]);
            //    dblOnCreditMSR += CDec2(LastTotal[22]);
            //    dblChargesMSR += CDec2(LastTotal[19]);

            //    dblNonVATMSR += CDec2(LastTotal[11]);
            //    dblVATExemptMSR += CDec2(LastTotal[12]);
            //    dblZeroRatedVATMSR += CDec2(LastTotal[13]);

            //    //---

            //    dblTotalVATSalesMSR += CDec2(LastTotal[14]);
            //    dblTotalVATAmountMSR += CDec2(LastTotal[15]);

            //    dblTotalVATSalesSalesReturnMSR = CDec2(LastTotal[16]);
            //    dblTotalVATAmountSalesReturnMSR = CDec2(LastTotal[17]);
            //    dblTotalForeGoneVATMSR += CDec2(LastTotal[20]);
            //    dblTotalNumberOfCustomers += CDec2(LastTotal[21]);
            //    //--

            //}
             
            //if (LastTotalData != undefined) {
            //    var LastTotal = LastTotalData.split('|');

            //    dblTotalSales += CDec2(LastTotal[0]);
            //    dblTotalSalesDiscount += CDec2(LastTotal[1]);
            //    dblAdjRefund += CDec2(LastTotal[2]);
            //    dblAdjSRV += CDec2(LastTotal[3]);

            //    dblCash += CDec2(LastTotal[4]);
            //    dblCheck += CDec2(LastTotal[5]);
            //    dblDebitCard += CDec2(LastTotal[6]);
            //    dblCreditCard += CDec2(LastTotal[7]);
            //    dblSRV += CDec2(LastTotal[8]);
            //    dblOtherPaymentType += CDec2(LastTotal[18]);
            //    dblOnCredit += CDec2(LastTotal[21]);
            //    dblCharges += CDec2(LastTotal[19]);

            //    dblNonVAT += CDec2(LastTotal[11]);
            //    dblVATExempt += CDec2(LastTotal[12]);
            //    dblZeroRatedVAT += CDec2(LastTotal[13]);

            //    //---

            //    dblTotalVATSales += CDec2(LastTotal[14]);
            //    dblTotalVATAmount += CDec2(LastTotal[15]);

            //    dblTotalVATSalesSalesReturn = CDec2(LastTotal[16]);
            //    dblTotalVATAmountSalesReturn = CDec2(LastTotal[17]);

            //    dblTotalForeGoneVAT += CDec2(LastTotal[20]);
            //    //--
            //}
            //else {
            //    //this means no '.dat' file has been created yet. Its possible that its the first transaction of the day therefore we need to delete previous offline csr

            //    //var mDate = new Date();

            //    for (var i = 1; i < 16; i++) {
            //        mDate.setDate(mDate.getDate() - 1);
            //        Transfers.DeleteFile(sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + Web.PeopleUserID() + ($.format.number(mDate.getFullYear(), '00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat')
            //    }
            //}
            //END OF ORIGINAL CODE

            var LastTotalData = Sql.Execute('SELECT * FROM cache.CacheCSR WHERE OrgID = ' + ReadyString(sessionStorage.OrgID) + ' \n' +
                                        ' AND FowstID = ' + ReadyString(sessionStorage.FowstID) + ' \n' +
                                        ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' \n' +
                                        ' AND UserID = ' + ReadyString(Web.PeopleUserID()) + ' \n' +
                                        ' AND SalesDate = ' + ReadyString($.format.date(ReportDate, 'yyyy-MM-dd'))).Rows

            var LastTotalDataMSR = Sql.Execute('SELECT * FROM cache.CacheMSR WHERE OrgID = ' + ReadyString(sessionStorage.OrgID) + ' \n' +
                                        ' AND FowstID = ' + ReadyString(sessionStorage.FowstID) + ' \n' +
                                        ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' \n' +
                                        ' AND SalesDate = ' + ReadyString($.format.date(ReportDate, 'yyyy-MM-dd'))).Rows

            pSql = '';

            if (LastTotalDataMSR.length > 0) {
                var LastTotal = LastTotalDataMSR[0];

                dblTotalSalesMSR += CDec2(LastTotal.TotalSales);
                dblTotalSalesDiscountMSR += CDec2(LastTotal.TotalSalesDiscount);
                dblAdjRefundMSR += CDec2(LastTotal.AdjRefund);
                dblAdjSRVMSR += CDec2(LastTotal.AdjSRV);

                dblCashMSR += CDec2(LastTotal.Cash);
                dblCheckMSR += CDec2(LastTotal.Check);
                dblDebitCardMSR += CDec2(LastTotal.DebitCard);
                dblCreditCardMSR += CDec2(LastTotal.CreditCard);
                dblSRVMSR += CDec2(LastTotal.SRV);
                dblOtherPaymentTypeMSR += CDec2(LastTotal.TotalOtherPayment);
                dblOnCreditMSR += CDec2(LastTotal.OnCredit);
                dblChargesMSR += CDec2(LastTotal.Charges);

                dblNonVATMSR += CDec2(LastTotal.NonVAT);
                dblVATExemptMSR += CDec2(LastTotal.VATExempt);
                dblZeroRatedVATMSR += CDec2(LastTotal.ZeroRatedVAT);

                //---

                dblTotalVATSalesMSR += CDec2(LastTotal.TotalVATSales);
                dblTotalVATAmountMSR += CDec2(LastTotal.TotalVATAmount);

                dblTotalVATSalesSalesReturnMSR = CDec2(LastTotal.TotalVATSalesSalesReturn);
                dblTotalVATAmountSalesReturnMSR = CDec2(LastTotal.TotalVATAmountSalesReturn);
                dblTotalForeGoneVATMSR += CDec2(LastTotal.TotalForeGoneVAT);
                dblTotalNumberOfCustomers += CDec2(LastTotal.TotalNoOfCustomers);
                //--

                dblAlipayMSR += CDec2(LastTotal.OtherPaymentAlipay);
                dblComplimentaryMSR += CDec2(LastTotal.OtherPaymentComplimentary);
                dblDiscountVoucherMSR += CDec2(LastTotal.OtherPaymentDiscountVoucher);
                dblEPlusMSR += CDec2(LastTotal.OtherPaymentEPlus);
                dblFoodPandaMSR += CDec2(LastTotal.OtherPaymentFoodPanda);
                dblGCashMSR += CDec2(LastTotal.OtherPaymentGCash);
                dblGoldCardMSR += CDec2(LastTotal.OtherPaymentGoldCard);
                dblGrabFoodMSR += CDec2(LastTotal.OtherPaymentGrabFood);
                dblLocalDeliveryMSR += CDec2(LastTotal.OtherPaymentLocalDelivery);
                dblPayMayaMSR += CDec2(LastTotal.OtherPaymentPayMaya);
                dblSMACMSR += CDec2(LastTotal.OtherPaymentSMAC);
                dblShopeeEVoucherMSR += CDec2(LastTotal.OtherPaymentShopeeEVoucher);
                dblShopeePayMSR += CDec2(LastTotal.OtherPaymentShopeePay);
                dblVPlusMSR += CDec2(LastTotal.OtherPaymentVPLUS);

                if (IsOtherPaymentType) {
                    for (var i = 0; i < otherpayment.length; i++) {
                        var opdata = otherpayment[i];
                        opdata.PaymentType = opdata.PaymentType.replace(/\s+/g, '');
                        opdata.LastTotalCacheMSR = CDec2(LastTotal['OtherPayment' + opdata.PaymentType])
                    }
                    dblOtherPaymentAmountMSR += CDec2(LastTotal['OtherPayment' + opdata.PaymentType]);
                }

                for (var i in discountlist) {
                    discountlist[i].LastTotalCacheMSR = CDec2(LastTotal[discountlist[i].ColumnName]);
                }
            }

            if (LastTotalData.length > 0) {
                var LastTotal = LastTotalData[0];

                dblTotalSales += CDec2(LastTotal.TotalSales);
                dblTotalSalesDiscount += CDec2(LastTotal.TotalSalesDiscount);
                dblAdjRefund += CDec2(LastTotal.AdjRefund);
                dblAdjSRV += CDec2(LastTotal.AdjSRV);

                dblCash += CDec2(LastTotal.Cash);
                dblCheck += CDec2(LastTotal.Check);
                dblDebitCard += CDec2(LastTotal.DebitCard);
                dblCreditCard += CDec2(LastTotal.CreditCard);
                dblSRV += CDec2(LastTotal.SRV);
                dblOtherPaymentType += CDec2(LastTotal.TotalOtherPayment);
                dblOnCredit += CDec2(LastTotal.OnCredit);
                dblCharges += CDec2(LastTotal.Charges);

                dblNonVAT += CDec2(LastTotal.NonVAT);
                dblVATExempt += CDec2(LastTotal.VATExempt);
                dblZeroRatedVAT += CDec2(LastTotal.ZeroRatedVAT);

                //---

                dblTotalVATSales += CDec2(LastTotal.TotalVATSales);
                dblTotalVATAmount += CDec2(LastTotal.TotalVATAmount);

                dblTotalVATSalesSalesReturn = CDec2(LastTotal.TotalVATSalesSalesReturn);
                dblTotalVATAmountSalesReturn = CDec2(LastTotal.TotalVATAmountSalesReturn);

                dblTotalForeGoneVAT += CDec2(LastTotal.TotalForeGoneVAT);
                //--

                dblAlipay += CDec2(LastTotal.OtherPaymentAlipay);
                dblComplimentary += CDec2(LastTotal.OtherPaymentComplimentary);
                dblDiscountVoucher += CDec2(LastTotal.OtherPaymentDiscountVoucher);
                dblEPlus += CDec2(LastTotal.OtherPaymentEPlus);
                dblFoodPanda += CDec2(LastTotal.OtherPaymentFoodPanda);
                dblGCash += CDec2(LastTotal.OtherPaymentGCash);
                dblGoldCard += CDec2(LastTotal.OtherPaymentGoldCard);
                dblGrabFood += CDec2(LastTotal.OtherPaymentGrabFood);
                dblLocalDelivery += CDec2(LastTotal.OtherPaymentLocalDelivery);
                dblPayMaya += CDec2(LastTotal.OtherPaymentPayMaya);
                dblSMAC += CDec2(LastTotal.OtherPaymentSMAC);
                dblShopeeEVoucher += CDec2(LastTotal.OtherPaymentShopeeEVoucher);
                dblShopeePay += CDec2(LastTotal.OtherPaymentShopeePay);
                dblVPlus += CDec2(LastTotal.OtherPaymentVPLUS);      

                if (IsOtherPaymentType) {
                    for (var i = 0; i < otherpayment.length; i++) {
                        var opdata = otherpayment[i];
                        opdata.PaymentType = opdata.PaymentType.replace(/\s+/g, '');
                        opdata.LastTotalCacheCSR = CDec2(LastTotal['OtherPayment' + opdata.PaymentType])
                    }
                }

                for (var i in discountlist) {
                    discountlist[i].LastTotalCacheCSR = CDec2(LastTotal[discountlist[i].ColumnName]);
                }
            }

            mDate = new Date();

            dblVAT += CDec2(dblTotalVATSales) - CDec2(dblTotalVATSalesSalesReturn);
            dblVATAmount += CDec2(dblTotalVATAmount) - CDec2(dblTotalVATAmountSalesReturn);

            dblVATMSR += CDec2(dblTotalVATSalesMSR) - CDec2(dblTotalVATSalesSalesReturnMSR);
            dblVATAmountMSR += CDec2(dblTotalVATAmountMSR) - CDec2(dblTotalVATAmountSalesReturnMSR);            

            //Transfers.WriteFile(sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + Web.PeopleUserID() + ($.format.number(ReportDate.getFullYear(), '00').toString() + '-' + $.format.number((ReportDate.getMonth() + 1), '00').toString() + '-' + ReportDate.getDate().toString()) + '.dat', null,
            //                                dblTotalSales + '|' + dblTotalSalesDiscount + '|' + dblAdjRefund + '|' + dblAdjSRV + '|' +
            //                                    dblCash + '|' + dblCheck + '|' + dblDebitCard + '|' + dblCreditCard + '|' + dblSRV + '|' +
            //                                    dblVAT + '|' + dblVATAmount + '|' + dblNonVAT + '|' + dblVATExempt + '|' + dblZeroRatedVAT + '|' +
            //                                    dblTotalVATSales + '|' + dblTotalVATAmount + '|' + dblTotalVATSalesSalesReturn + '|' + dblTotalVATAmountSalesReturn + '|' +
            //                                    dblOtherPaymentType + '|' + dblCharges + '|' + dblTotalForeGoneVAT + '|' + dblOnCredit);

            //Transfers.WriteFile(sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + ($.format.number(ReportDate.getFullYear(), '00').toString() + '-' + $.format.number((ReportDate.getMonth() + 1), '00').toString() + '-' + ReportDate.getDate().toString()) + '.dat', null,
            //                                dblTotalSalesMSR + '|' + dblTotalSalesDiscountMSR + '|' + dblAdjRefundMSR + '|' + dblAdjSRVMSR + '|' +
            //                                    dblCashMSR + '|' + dblCheckMSR + '|' + dblDebitCardMSR + '|' + dblCreditCardMSR + '|' + dblSRVMSR + '|' +
            //                                    dblVATMSR + '|' + dblVATAmountMSR + '|' + dblNonVATMSR + '|' + dblVATExemptMSR + '|' + dblZeroRatedVATMSR + '|' +
            //                                    dblTotalVATSalesMSR + '|' + dblTotalVATAmountMSR + '|' + dblTotalVATSalesSalesReturnMSR + '|' + dblTotalVATAmountSalesReturnMSR + '|' +
            //                                    dblOtherPaymentTypeMSR + '|' + dblChargesMSR + '|' + dblTotalForeGoneVATMSR + '|' + dblTotalNumberOfCustomers + '|' + dblOnCreditMSR);

            var cachedata = {
                TotalSales: dblTotalSales,
                TotalSalesDiscount: dblTotalSalesDiscount,
                AdjRefund: dblAdjRefund,
                AdjSRV: dblAdjSRV,
                Cash: dblCash,
                Check: dblCheck,
                DebitCard: dblDebitCard,
                CreditCard: dblCreditCard,
                SRV: dblSRV,
                VAT: dblVAT,
                VATAmount: dblVATAmount,
                NonVAT: dblNonVAT,
                VATExempt: dblVATExempt,
                ZeroRatedVAT: dblZeroRatedVAT,
                TotalVATSales: dblTotalVATSales,
                TotalVATAmount: dblTotalVATAmount,
                TotalVATSalesSalesReturn: dblTotalVATSalesSalesReturn,
                TotalVATAmountSalesReturn: dblTotalVATAmountSalesReturn,
                OtherPaymentType: dblOtherPaymentType, // accumulated otherpayment
                OtherPaymentAlipay: dblAlipay,
                OtherPaymentComplimentary: dblComplimentary,
                OtherPaymentDiscountVoucher: dblDiscountVoucher,
                OtherPaymentEPlus: dblEPlus,
                OtherPaymentFoodPanda: dblFoodPanda,
                OtherPaymentGCash: dblGCash,
                OtherPaymentGoldCard: dblGoldCard,
                OtherPaymentGrabFood: dblGrabFood,
                OtherPaymentLocalDelivery: dblLocalDelivery,
                OtherPaymentPayMaya: dblPayMaya,
                OtherPaymentSMAC: dblSMAC,
                OtherPaymentShopeeEVoucher: dblShopeeEVoucher,
                OtherPaymentShopeePay: dblShopeePay,
                OtherPaymentVPLUS: dblVPlus,
                Charges: dblCharges,
                TotalForeGoneVAT: dblTotalForeGoneVAT,
                OnCredit: dblOnCredit,
                SalesDate: $.format.date(ReportDate, 'yyyy-MM-dd'),
                CashierID: Web.PeopleUserID()
            }

            SaveToCacheTable(cachedata, 'CacheCSR', otherpayment, discountlist, vID);

            cachedata = {
                TotalSales: dblTotalSalesMSR,
                TotalSalesDiscount: dblTotalSalesDiscountMSR,
                AdjRefund: dblAdjRefundMSR,
                AdjSRV: dblAdjSRVMSR,
                Cash: dblCashMSR,
                Check: dblCheckMSR,
                DebitCard: dblDebitCardMSR,
                CreditCard: dblCreditCardMSR,
                SRV: dblSRVMSR,
                VAT: dblVATMSR,
                VATAmount: dblVATAmountMSR,
                NonVAT: dblNonVATMSR,
                VATExempt: dblVATExemptMSR,
                ZeroRatedVAT: dblZeroRatedVATMSR,
                TotalVATSales: dblTotalVATSalesMSR,
                TotalVATAmount: dblTotalVATAmountMSR,
                TotalVATSalesSalesReturn: dblTotalVATSalesSalesReturnMSR,
                TotalVATAmountSalesReturn: dblTotalVATAmountSalesReturnMSR,
                OtherPaymentType: dblOtherPaymentTypeMSR, // accumulated otherpayment
                OtherPaymentAlipay: dblAlipayMSR,
                OtherPaymentComplimentary: dblComplimentaryMSR,
                OtherPaymentDiscountVoucher: dblDiscountVoucherMSR,
                OtherPaymentEPlus: dblEPlusMSR,
                OtherPaymentFoodPanda: dblFoodPandaMSR,
                OtherPaymentGCash: dblGCashMSR,
                OtherPaymentGoldCard: dblGoldCardMSR,
                OtherPaymentGrabFood: dblGrabFoodMSR,
                OtherPaymentLocalDelivery: dblLocalDeliveryMSR,
                OtherPaymentPayMaya: dblPayMayaMSR,
                OtherPaymentSMAC: dblSMACMSR,
                OtherPaymentShopeeEVoucher: dblShopeeEVoucherMSR,
                OtherPaymentShopeePay: dblShopeePayMSR,
                OtherPaymentVPLUS: dblVPlusMSR,
                Charges: dblChargesMSR,
                TotalForeGoneVAT: dblTotalForeGoneVATMSR,
                TotalNoOfCustomer: dblTotalNumberOfCustomers,
                OnCredit: dblOnCreditMSR,                
                SalesDate: $.format.date(ReportDate, 'yyyy-MM-dd')
            }

            SaveToCacheTable(cachedata, 'CacheMSR', otherpayment, discountlist, vID);

            var LastTransactionItemsCountData = Transfers.ReadFile('LastTransactionItemsCount' + sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + ($.format.number(ReportDate.getFullYear(), '00').toString() + '-' + $.format.number((ReportDate.getMonth() + 1), '00').toString() + '-' + ReportDate.getDate().toString()) + '.dat');


            var NoOfTransaction = 0;
            var NoOfItems = 0;

            if (LastTransactionItemsCountData != undefined) {
                var LastTotalTransactionCount = LastTransactionItemsCountData.split('|');
                
                NoOfTransaction = CInt2(LastTotalTransactionCount[0]) + 1;
                NoOfItems = CInt2(LastTotalTransactionCount[1]) + TransactionItemsCount;
            }
            else {
                NoOfTransaction = 1;
                NoOfItems = TransactionItemsCount;
            }

            Transfers.WriteFile('LastTransactionItemsCount' + sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + ($.format.number(ReportDate.getFullYear(), '00').toString() + '-' + $.format.number((ReportDate.getMonth() + 1), '00').toString() + '-' + ReportDate.getDate().toString()) + '.dat',null,
                                NoOfTransaction + '|' + NoOfItems)

        }

        this.SaveToCacheTable = function (data, table, otherpayment, discountlist, vID) {
            //cachedata[opdata.PaymentType] = dblOtherPaymentAmountMSR;

            if (IsGuid(vID)) {
                for (var i = 0; i < otherpayment.length; i++) {
                    var opdata = otherpayment[i];
                    opdata.PaymentType = opdata.PaymentType.replace(/\s+/g, '');
                    data['OtherPayment' + opdata.PaymentType] = CDec2(opdata.Amount);
                }

                var invdiscount = Sql.Execute(' SELECT disc.DiscountTypeID, IFNULL(libdisc.DiscountType, cusdisc.Name) as DiscountType, custpay.DiscountAmount \n' +
                    ' FROM TransCustomerPaymentInvoices custpayinv \n' +
                    ' INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) \n' +
                    ' INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID) \n' +
                    ' INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID \n' +
                    ' LEFT JOIN LibraryDiscountSettings as libdisc ON libdisc.ID = disc.DiscountTypeID \n' +
                    ' LEFT JOIN LibraryCustomDiscounts as cusdisc ON cusdisc.ID = disc.DiscountTypeID \n' +
                    ' WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> "RF" \n' +
                    ' AND inv.ID = ' + ReadyString(vID)).Rows;

                for (var i in discountlist) {
                    data[discountlist[i].ColumnName] = 0;

                    //if (LastTotalDataMSR.length > 0) {
                    //    var LastTotal = LastTotalDataMSR[0];
                    //    data[discountlist[i].ColumnName] += CDec2(LastTotal[discountlist[i].ColumnName]);
                    //}

                    for (var d in invdiscount) {
                        if (invdiscount[d].DiscountTypeID == discountlist[i].ID) {
                            data[discountlist[i].ColumnName] += CDec2(invdiscount[d].DiscountAmount);
                        }
                    }
                }
            }

            var pIU = new InsertUpdate('cache.' + table);

            pIU.Add('TotalSales', CDec2(data.TotalSales).toFixed(2));
            pIU.Add('TotalSalesDiscount', CDec2(data.TotalSalesDiscount).toFixed(2));
            pIU.Add('AdjRefund', CDec2(data.AdjRefund).toFixed(2));
            pIU.Add('AdjSRV', CDec2(data.AdjSRV).toFixed(2));
            pIU.Add('Cash', CDec2(data.Cash).toFixed(2));
            pIU.Add('[Check]', CDec2(data.Check).toFixed(2));
            pIU.Add('DebitCard', CDec2(data.DebitCard).toFixed(2));
            pIU.Add('CreditCard', CDec2(data.CreditCard).toFixed(2));
            pIU.Add('SRV', CDec2(data.SRV).toFixed(2));
            pIU.Add('VAT', CDec2(data.VAT).toFixed(2));
            pIU.Add('VATAmount', CDec2(data.VATAmount).toFixed(2));
            pIU.Add('NonVAT', CDec2(data.NonVAT).toFixed(2));
            pIU.Add('VATExempt', CDec2(data.VATExempt).toFixed(2));
            pIU.Add('ZeroRatedVAT', CDec2(data.ZeroRatedVAT).toFixed(2));
            pIU.Add('TotalVATSales', CDec2(data.TotalVATSales).toFixed(2));
            pIU.Add('TotalVATAmount', CDec2(data.TotalVATAmount).toFixed(2));
            pIU.Add('TotalVATSalesSalesReturn', CDec2(data.TotalVATSalesSalesReturn).toFixed(2));
            pIU.Add('TotalVATAmountSalesReturn', CDec2(data.TotalVATAmountSalesReturn).toFixed(2));
            pIU.Add('TotalOtherPayment', CDec2(data.OtherPaymentType).toFixed(2));
            pIU.Add('OtherPaymentAlipay', CDec2(data.OtherPaymentAlipay).toFixed(2));
            pIU.Add('OtherPaymentComplimentary', CDec2(data.OtherPaymentComplimentary).toFixed(2));
            pIU.Add('OtherPaymentDiscountVoucher', CDec2(data.OtherPaymentDiscountVoucher).toFixed(2));
            pIU.Add('OtherPaymentEPlus', CDec2(data.OtherPaymentEPlus).toFixed(2));
            pIU.Add('OtherPaymentFoodPanda', CDec2(data.OtherPaymentFoodPanda).toFixed(2));
            pIU.Add('OtherPaymentGCash', CDec2(data.OtherPaymentGCash).toFixed(2));
            pIU.Add('OtherPaymentGoldCard', CDec2(data.OtherPaymentGoldCard).toFixed(2));
            pIU.Add('OtherPaymentGrabFood', CDec2(data.OtherPaymentGrabFood).toFixed(2));
            pIU.Add('OtherPaymentLocalDelivery', CDec2(data.OtherPaymentLocalDelivery).toFixed(2));
            pIU.Add('OtherPaymentPayMaya', CDec2(data.OtherPaymentPayMaya).toFixed(2));
            pIU.Add('OtherPaymentSMAC', CDec2(data.OtherPaymentSMAC).toFixed(2));
            pIU.Add('OtherPaymentShopeeEVoucher', CDec2(data.OtherPaymentShopeeEVoucher).toFixed(2));
            pIU.Add('OtherPaymentShopeePay', CDec2(data.OtherPaymentShopeePay).toFixed(2));
            pIU.Add('OtherPaymentVPLUS', CDec2(data.OtherPaymentVPLUS).toFixed(2));
            pIU.Add('Charges', CDec2(data.Charges).toFixed(2));
            pIU.Add('TotalForeGoneVAT', CDec2(data.TotalForeGoneVAT).toFixed(2));
            pIU.Add('OnCredit', CDec2(data.OnCredit).toFixed(2));
            pIU.Add('SalesDate', ReadyString(data.SalesDate));
            pIU.Add('OrgID', ReadyString(sessionStorage.OrgID));
            pIU.Add('FowstID', ReadyString(sessionStorage.FowstID));
            pIU.Add('DeviceID', ReadyString(Web.DeviceID()));

            if (CDec2(data.CurVoid) > 0) {
                pIU.Add('CurVoid', ReadyString(data.CurVoid));
            }

            if (CDec2(data.CurVoidItemQty) > 0) {
                pIU.Add('CurVoidItemQty', ReadyString(data.CurVoidItemQty));
            }

            for (var i = 0; i < otherpayment.length; i++) {
                var opdata = otherpayment[i];
                opdata.PaymentType = opdata.PaymentType.replace(/\s+/g, '');
                pIU.Add('[OtherPayment' + opdata.PaymentType + ']', CDec2(CDec2(opdata['LastTotal' + table]).toFixed(2)) + CDec2(CDec2(data['OtherPayment' + opdata.PaymentType]).toFixed(2)));
            }            

            for (var i in discountlist) {
                pIU.Add('[' + discountlist[i].ColumnName + ']', CDec2(CDec2(discountlist[i]['LastTotal' + table]).toFixed(2)) + CDec2(CDec2(data[discountlist[i].ColumnName]).toFixed(2)));
            }

            if (table == 'CacheCSR') {
                pSql = pIU.GetUpdate(' OrgID = ' + ReadyString(sessionStorage.OrgID) + ' \n' +
                    ' AND FowstID = ' + ReadyString(sessionStorage.FowstID) + ' \n' +
                    ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' \n' +
                    ' AND UserID = ' + ReadyString(data.CashierID) + ' \n' +
                    ' AND SalesDate = ' + ReadyString(data.SalesDate));

                if (CDec2(Sql.ExecuteSql(pSql)) == 0) {
                    pIU.Add("ID", ReadyString(NewID()));
                    pIU.Add('UserID', ReadyString(data.CashierID));
                    pSql = pIU.GetInsert();
                    Sql.ExecuteSql(pSql)
                }
            } else {
                pIU.Add('TotalNoOfCustomers', CDec2(data.TotalNoOfCustomers));

                pSql = pIU.GetUpdate(' OrgID = ' + ReadyString(sessionStorage.OrgID) + ' \n' +
                    ' AND FowstID = ' + ReadyString(sessionStorage.FowstID) + ' \n' +
                    ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' \n' +
                    ' AND SalesDate = ' + ReadyString(data.SalesDate));

                if (CDec2(Sql.ExecuteSql(pSql)) == 0) {
                    pIU.Add("ID", ReadyString(NewID()));
                    pSql = pIU.GetInsert();
                    Sql.ExecuteSql(pSql)
                }
            }
        }

        this.UpdateZReadData = function (vID) {
            var dblTotalSales = 0;
            var dblTotalSalesDiscount = 0;
            var dblTotalForeGoneVAT = 0;
            var dblTotalNumberOfCustomers = 0;
            var dblAdjRefund = 0;
            var dblAdjSRV = 0;
            var dblNetSales = 0;

            var dblCash = 0;
            var dblCheck = 0;
            var dblDebitCard = 0;
            var dblCreditCard = 0;
            var dblSRV = 0;
            var dblOtherPaymentType = 0;
            var dblOnCredit = 0;
            var dblCharges = 0;
            var dblTotalPayment = 0;

            var dblVAT = 0;
            var dblVATAmount = 0;
            var dblNonVAT = 0;
            var dblVATExempt = 0;
            var dblZeroRatedVAT = 0;
            var dblTotalVAT = 0;

            var dblTotalVATSales = 0;
            var dblTotalVATAmount = 0;
            var dblTotalVATSalesSalesReturn = 0;
            var dblTotalVATAmountSalesReturn = 0;

            var dblTotalSalesMSR = 0;
            var dblTotalSalesDiscountMSR = 0;
            var dblTotalForeGoneVATMSR = 0;
            var dblAdjRefundMSR = 0;
            var dblAdjSRVMSR = 0;
            var dblNetSalesMSR = 0;

            var dblCashMSR = 0;
            var dblCheckMSR = 0;
            var dblDebitCardMSR = 0;
            var dblCreditCardMSR = 0;
            var dblSRVMSR = 0;

            var dblOnCreditMSR = 0;
            var dblChargesMSR = 0;
            var dblTotalPaymentMSR = 0;
            var dblOtherPaymentAmountMSR = 0;

            var dblVATMSR = 0;
            var dblVATAmountMSR = 0;
            var dblNonVATMSR = 0;
            var dblVATExemptMSR = 0;
            var dblZeroRatedVATMSR = 0;
            var dblTotalVATMSR = 0;

            var dblTotalVATSalesMSR = 0;
            var dblTotalVATAmountMSR = 0;
            var dblTotalVATSalesSalesReturnMSR = 0;
            var dblTotalVATAmountSalesReturnMSR = 0;

            var mDate; //= new Date();
            var ReportDate;

            var vDataInvoice;
            var vDataPayments;

            var pSql = '';
            var newCacheZreadColumns = ['CurVoid', 'CurVoidItemQty', 'PrevSRVAmount', 'PrevSCDiscount', 'PrevVATExempt', 'PrevSCDiscountExempt', 'CurSCDiscountExempt', 'PrevSoloParentDiscount', 'PrevPhNatTeamDiscount', 'CurSoloParentDiscount', 'CurPhNatTeamDiscount', 'PrintCount', 'LastDateOfReprint'];
            var cachezreadColumns = Sql.Execute("PRAGMA cache.table_info(CacheZRead)");            

            for (var i in newCacheZreadColumns) {
                var isExist = false;
                for (var m in cachezreadColumns.Rows) {
                    if (cachezreadColumns.Rows[m].name == newCacheZreadColumns[i]) {
                        isExist = true;
                        break;
                    }
                }

                if (!isExist) {
                    pSql = " ALTER TABLE cachezread " +
                        " ADD " + newCacheZreadColumns[i] + " money"
                    Sql.Execute(pSql);
                }
            }

            pSql = "Select InvoiceDate from TransInvoice where ID = " + ReadyString(vID);

            mDate = new Date(Sql.ExecuteScalar(pSql));

            ReportDate = new Date(mDate);

            //ORIGINAL QUERY
            //pSql = "Select SUM(case when quantity=(0) then (0) when (((ifnull(vatablesale,(0))+ifnull(vatexemptsale,(0)))+ifnull(zeroratedsale,(0)))+ifnull(nonvatsale,(0)))=(0) then ifnull(subtotal,(0)) else ((ifnull(vatablesale,(0))+ifnull(vatexemptsale,(0)))+ifnull(zeroratedsale,(0)))+ifnull(nonvatsale,(0)) end) CurGrossSales, " +
            //        "SUM(IFNULL(invitem.DiscountAmount,0)) + inv.SCDiscount CurSalesDiscount, " +
            //        "inv.ForeGoneVAT CurVATExemptTax, " +
            //        "inv.InvoiceNumber InvoiceNo, " +
            //        "inv.SCDiscount CurSCDiscount, " +
            //        "SUM(IFNULL(invitem.TaxSales,0) - IFNULL(invitem.DiscountSales,0)) CurTotalVATSales, " +
            //        "SUM(IFNULL(invitem.TaxAmount,0)- IFNULL((case when vatexemptsale>(0) then (0) else invitem.DiscountAmount-invitem.DiscountSales end),0)) CurTotalVATAmount, " +
            //        "SUM(invitem.NonVATSale) CurNoTAXSales, " +
            //        "(inv.VATExempt - inv.SCDiscount) CurVATExemptSales, " +
            //        "SUM(invitem.ZeroRatedSale) CurZeroRatedVATSales " +
            //        "FROM TransCustomerPaymentInvoices custpayinv " +
            //        " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
            //        " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
            //        " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " +
            //        "Where inv.ID = " + ReadyString(vID)

            pSql = "Select SUM(case when quantity=(0) then (0) when (((ifnull(vatablesale,(0))+ifnull(vatexemptsale,(0)))+ifnull(zeroratedsale,(0)))+ifnull(nonvatsale,(0)))=(0) then ifnull(subtotal,(0)) else ((ifnull(vatablesale,(0))+ifnull(vatexemptsale,(0)))+ifnull(zeroratedsale,(0)))+ifnull(nonvatsale,(0)) end) CurGrossSales, \n " +
                    "SUM(IFNULL(invitem.DiscountAmount,0)) + IFNULL(inv.SCDiscount, 0) + IFNULL(inv.SoloParentDiscount, 0) CurSalesDiscount, \n " +
                    "inv.ForeGoneVAT CurVATExemptTax, \n " +
                    "inv.InvoiceNumber InvoiceNo, \n " +
                    "inv.SCDiscount CurSCDiscount, \n " +
                    "inv.SCDiscountExempt CurSCDiscountExempt, \n " +
                    "inv.SoloParentDiscount CurSoloParentDiscount, \n " +
                    "inv.PhNatTeamDiscount CurPhNatTeamDiscount, \n " +
                    "IFNULL((SELECT  \n " +
                    "   SUM(IFNULL(invitems.TaxSales,0) - IFNULL(invitems.DiscountSales,0)) \n " +
                    "   FROM --TransCustomerPaymentInvoices custpayinvs \n " +
                    "   --INNER JOIN TransCustomerPayment custpays ON (custpayinvs.CustomerPaymentID = custpays.ID) \n " +
                    "   --INNER JOIN " +
                    "   \n TransInvoice invs --ON (custpayinvs.InvoiceID = invs.ID) \n " +
                    "   \n INNER JOIN TransInvoiceItems invitems ON (invitems.InvoiceID = invs.ID) \n " +
                    "   WHERE invs.InvoiceNumber > 0 \n " +
                    "   -- AND custpays.ReceiptType <> 'RF' \n " +
                    "   \n AND IFNULL(invs.SCDiscount, 0) = 0 AND IFNULL(invs.SoloParentDiscount, 0) = 0 \n " +
                    "   AND invs.ID = inv.ID \n " +
                    "), 0) CurTotalVATSales, \n " +
                    " IFNULL((SELECT invs.Vatable " +
                    "   FROM TransInvoice invs " +
                    "   WHERE invs.InvoiceNumber > 0 AND invs.ForegoneVAT>0" +
                    "   AND invs.ID = inv.ID) " +
                    ",0) CurTotalVatablePortionOfSCDTransaction, " +
                    " IFNULL((SELECT IFNULL(invs.VAT, 0) " +
                    "   FROM TransInvoice invs " +
                    "   WHERE invs.InvoiceNumber > 0 AND invs.ForegoneVAT>0" +
                    "   AND invs.ID = inv.ID) " +
                    ",0) CurTotalVATAmountPortionOfSCDTransaction, " +
                    "IFNULL((SELECT  \n " +
                    "   SUM(IFNULL(invitems.TaxAmount,0)- IFNULL((case when invitems.vatexemptsale>(0) then (0) else invitems.DiscountAmount-invitems.DiscountSales end),0)) \n " +
                    "   FROM TransCustomerPaymentInvoices custpayinvs \n " +
                    "   INNER JOIN TransCustomerPayment custpays ON (custpayinvs.CustomerPaymentID = custpays.ID) \n " +
                    "   INNER JOIN TransInvoice invs ON (custpayinvs.InvoiceID = invs.ID) \n " +
                    "   INNER JOIN TransInvoiceItems invitems ON (invitems.InvoiceID = invs.ID) \n " +
                    "   WHERE invs.InvoiceNumber > 0 \n " +
                    "   AND custpays.ReceiptType <> 'RF' \n " +
                    "   AND IFNULL(invs.SCDiscount, 0) = 0 AND IFNULL(invs.SoloParentDiscount, 0) = 0 \n " +
                    "   AND invs.ID = inv.ID \n " +
                    "), 0) CurTotalVATAmount, \n " +
                    "IFNULL((SELECT  \n " +
                    "   SUM(invitems.NonVATSale) \n " +
                    "   FROM TransCustomerPaymentInvoices custpayinvs \n " +
                    "   INNER JOIN TransCustomerPayment custpays ON (custpayinvs.CustomerPaymentID = custpays.ID) \n " +
                    "   INNER JOIN TransInvoice invs ON (custpayinvs.InvoiceID = invs.ID) \n " +
                    "   INNER JOIN TransInvoiceItems invitems ON (invitems.InvoiceID = invs.ID) \n " +
                    "   WHERE invs.ID = inv.ID \n " +
                    "), 0) CurNoTAXSales, \n " +
                    "IFNULL((SELECT  \n " +
                    "   SUM(invs.VATExempt) \n " + // - (invs.SCDiscount + invs.SoloParentDiscount) dapat dili daw ni sila ideduct apil sa VATExempt 090125
                    "   FROM TransCustomerPaymentInvoices custpayinvs \n " +
                    "   INNER JOIN TransCustomerPayment custpays ON (custpayinvs.CustomerPaymentID = custpays.ID) \n " +
                    "   INNER JOIN TransInvoice invs ON (custpayinvs.InvoiceID = invs.ID) \n " + //"   --INNER JOIN TransInvoiceItems invitems ON (invitems.InvoiceID = invs.ID) \n " +
                    "   WHERE invs.InvoiceNumber > 0 \n " +
                    "   AND custpays.ReceiptType <> 'RF' \n " +
                    "   AND invs.ID = inv.ID \n " +
                    "), 0) CurVATExemptSales, \n " +
                    "IFNULL((SELECT  \n " +
                    "   SUM(invitems.ZeroRatedSale) \n " +
                    "   FROM TransCustomerPaymentInvoices custpayinvs \n " +
                    "   INNER JOIN TransCustomerPayment custpays ON (custpayinvs.CustomerPaymentID = custpays.ID) \n " +
                    "   INNER JOIN TransInvoice invs ON (custpayinvs.InvoiceID = invs.ID) \n " +
                    "   INNER JOIN TransInvoiceItems invitems ON (invitems.InvoiceID = invs.ID) \n " +
                    "   WHERE invs.ID = inv.ID \n " +
                    "), 0) CurZeroRatedVATSales, \n " +
                    "IFNULL((SELECT  \n " +
                    "   SUM(invitems.DiscountAmount) \n " +
                    "   FROM TransCustomerPaymentInvoices custpayinvs \n " +
                    "   INNER JOIN TransCustomerPayment custpays ON (custpayinvs.CustomerPaymentID = custpays.ID) \n " +
                    "   INNER JOIN TransInvoice invs ON (custpayinvs.InvoiceID = invs.ID) \n " +
                    "   INNER JOIN TransInvoiceItems invitems ON (invitems.InvoiceID = invs.ID) \n " +
                    "   WHERE invs.InvoiceNumber > 0 AND IFNULL(invs.SCDiscount, 0) = 0 AND IFNULL(invs.SoloParentDiscount, 0) = 0 \n " +
                    "   AND DATE(invs.InvoiceDate) BETWEEN DATE(" + proj.GetZReadDateSql() + ") AND DATE(" + proj.GetZReadDateSql() + ") \n " +
                    "   AND invs.ID = inv.ID \n " +
                    "), 0) CurTotalDiscount \n " +
                    "FROM TransCustomerPaymentInvoices custpayinv \n " +
                    " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) \n " +
                    " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  \n " +
                    " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) \n " +
                    "Where inv.ID = \n " + ReadyString(vID)

            vDataInvoice = Sql.Execute(pSql);

            if (vDataInvoice.Rows.length > 0) {
                dblTotalSales = CDec2(vDataInvoice.Rows[0].CurGrossSales)
                dblTotalSalesDiscount = CDec2(vDataInvoice.Rows[0].CurSalesDiscount);
                dblTotalForeGoneVAT = CDec2(vDataInvoice.Rows[0].CurVATExemptTax);

                dblTotalVATSales = CDec2(vDataInvoice.Rows[0].CurTotalVATSales || 0); //- parseFloat(vDataInfo.Rows[p].CurTotalVATSalesSalesReturn || 0);
                dblTotalVATAmount = CDec2(vDataInvoice.Rows[0].CurTotalVATAmount || 0); // - parseFloat(vDataInfo.Rows[p].CurTotalVATAmountSalesReturn || 0);
                dblNonVAT = CDec2(vDataInvoice.Rows[0].CurNoTAXSales || 0);
                dblVATExempt = CDec2(vDataInvoice.Rows[0].CurVATExemptSales || 0);
                dblZeroRatedVAT = CDec2(vDataInvoice.Rows[0].CurZeroRatedVATSales || 0);

                dblTotalSalesMSR = CDec2(vDataInvoice.Rows[0].CurGrossSales)
                dblTotalSalesDiscountMSR = CDec2(vDataInvoice.Rows[0].CurSalesDiscount);
                dblTotalForeGoneVATMSR = CDec2(vDataInvoice.Rows[0].CurVATExemptTax);

                dblTotalVATSalesMSR = CDec2(vDataInvoice.Rows[0].CurTotalVATSales || 0); //- parseFloat(vDataInfo.Rows[p].CurTotalVATSalesSalesReturn || 0);
                dblTotalVATAmountMSR = CDec2(vDataInvoice.Rows[0].CurTotalVATAmount || 0); // - parseFloat(vDataInfo.Rows[p].CurTotalVATAmountSalesReturn || 0);
                dblNonVATMSR = CDec2(vDataInvoice.Rows[0].CurNoTAXSales || 0);
                dblVATExemptMSR = CDec2(vDataInvoice.Rows[0].CurVATExemptSales || 0);
                dblZeroRatedVATMSR = CDec2(vDataInvoice.Rows[0].CurZeroRatedVATSales || 0);

                dblTotalNumberOfCustomers = CDec2(sessionStorage.TotalNumberOfCustomer);
            }

            pSql = "Select SUM(IFNULL(custpay.CashAmount,0) - IFNULL(custpay.CustomerChangeCash,0)) CashAmount, " +
                    "SUM(IFNULL(custpay.DebitCardAmount,0)) DebitCardAmount, " +
                    "SUM(IFNULL(custpay.CreditCardAmount,0)) CreditCardAmount, " +
                    "SUM(IFNULL(custpay.CheckPaymentAmount,0)) CheckPaymentAmount, " +
                    "SUM(IFNULL(custpay.SRVAmount,0)) SRVAmount, " +
                    "SUM(IFNULL(custpay.OtherTypeAmount,0)) OtherTypeAmount, " +
                    "SUM(IFNULL(custpay.OnCreditAmount,0)) OnCreditAmount, " +
                    "SUM(IFNULL(custpay.ChargeAmount,0)) ChargeAmount " +
                    "FROM TransCustomerPaymentInvoices custpayinv " +
                    " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                    " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                    "Where inv.ID = " + ReadyString(vID)

            var vDataPayments = Sql.Execute(pSql);

            if (vDataPayments.Rows.length > 0) {

                dblCash = parseFloat(vDataPayments.Rows[0].CashAmount || 0);
                dblCheck = parseFloat(vDataPayments.Rows[0].CheckPaymentAmount || 0);
                dblDebitCard = parseFloat(vDataPayments.Rows[0].DebitCardAmount || 0);
                dblCreditCard = parseFloat(vDataPayments.Rows[0].CreditCardAmount || 0);
                dblSRV = parseFloat(vDataPayments.Rows[0].SRVAmount || 0);
                dblOtherPaymentType = parseFloat(vDataPayments.Rows[0].OtherTypeAmount || 0);
                dblOnCredit = parseFloat(vDataPayments.Rows[0].OnCreditAmount || 0);
                dblCharges = parseFloat(vDataPayments.Rows[0].ChargeAmount || 0);

                dblCashMSR = parseFloat(vDataPayments.Rows[0].CashAmount || 0);
                dblCheckMSR = parseFloat(vDataPayments.Rows[0].CheckPaymentAmount || 0);
                dblDebitCardMSR = parseFloat(vDataPayments.Rows[0].DebitCardAmount || 0);
                dblCreditCardMSR = parseFloat(vDataPayments.Rows[0].CreditCardAmount || 0);
                dblSRVMSR = parseFloat(vDataPayments.Rows[0].SRVAmount || 0);
                dblOtherPaymentTypeMSR = parseFloat(vDataPayments.Rows[0].OtherTypeAmount || 0);
                dblOnCreditMSR = parseFloat(vDataPayments.Rows[0].OnCreditAmount || 0);
                dblChargesMSR = parseFloat(vDataPayments.Rows[0].ChargeAmount || 0);
            }

            var dtTransCount;
            pSql = "SELECT COUNT(invitem.ProductID) CurTransInvItemCount " +
                    " FROM TransCustomerPaymentInvoices custpayinv " +
                    " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                    " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                    " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " +
                     "Where inv.ID = " + ReadyString(vID);
            dtTransCount = CInt2(Sql.ExecuteScalar(pSql));
            var dtCurrZRead;
            var dtPrevZRead

            //delete this ID (ccf76897-6e14-40b8-b227-b341857d8258)
            //this ID is reserved for ZERO ZRead sales data of the specific date.
            //temporary lang ni sya. For BIR purposes, if need nila og ZRead for that day kahit walay POS transactions makaprint pa rin ng ZRead.
            //ang INSERT data ani naa sa ZRead dashboard (ZRead.GenerateZeroSalesData() - ZRead.js)
            //by ARNIE, added on Sep 12, 2023

            var pVoid = Sql.Execute('SELECT CurVoid, CurVoidItemQty FROM cache.CacheZRead WHERE ID = ' + ReadyString('ccf76897-6e14-40b8-b227-b341857d8258')).Rows;
            var pCurVoid = 0, pCurVoidItemQty = 0;

            if (pVoid.length > 0) {
                pCurVoid = CDec2(pVoid[0].CurVoid);
                pCurVoidItemQty = CDec2(pVoid[0].CurVoidItemQty);                
            }

            Sql.Execute('DELETE FROM cache.CacheZRead WHERE ID = ' + ReadyString('ccf76897-6e14-40b8-b227-b341857d8258'))

            pSql = "Select * FROM CacheZRead " +
                " WHERE date(SalesDate) = " + ReadyString($.format.date(mDate, 'yyyy-MM-dd')) +
                " AND FOWSTID = " + ReadyString(sessionStorage.FowstID) +
                " AND DeviceID = " + ReadyString(Web.DeviceID())

            dtCurrZRead = Sql.Execute(pSql);

            var pIU = new InsertUpdate('CacheZRead');

            if (dtCurrZRead.Rows.length > 0) {

                pIU.Add('PresentationCurrGrossSales', CDec2(CDec2(dtCurrZRead.Rows[0].PresentationCurrGrossSales).toFixed(2)) + CDec2(CDec2(vDataInvoice.Rows[0].CurGrossSales).toFixed(2)));
                pIU.Add('CurGrossSales', CDec2(CDec2(dtCurrZRead.Rows[0].CurGrossSales).toFixed(2)) + CDec2(CDec2(vDataInvoice.Rows[0].CurGrossSales).toFixed(2)));
                pIU.Add('CurTotalDiscount', CDec2(CDec2(dtCurrZRead.Rows[0].CurTotalDiscount).toFixed(2)) + CDec2(CDec2(vDataInvoice.Rows[0].CurTotalDiscount).toFixed(2)));
                pIU.Add('CurSoloParentDiscount', CDec2(CDec2(dtCurrZRead.Rows[0].CurSoloParentDiscount).toFixed(2)) + CDec2(CDec2(vDataInvoice.Rows[0].CurSoloParentDiscount).toFixed(2)));
                pIU.Add('CurPhNatTeamDiscount', CDec2(CDec2(dtCurrZRead.Rows[0].CurPhNatTeamDiscount).toFixed(2)) + CDec2(CDec2(vDataInvoice.Rows[0].CurPhNatTeamDiscount).toFixed(2)));
                pIU.Add('CurSCDiscount', CDec2(CDec2(dtCurrZRead.Rows[0].CurSCDiscount).toFixed(2)) + CDec2(CDec2(vDataInvoice.Rows[0].CurSCDiscount).toFixed(2)));
                pIU.Add('CurSCDiscountExempt', CDec2(CDec2(dtCurrZRead.Rows[0].CurSCDiscountExempt).toFixed(2)) + CDec2(CDec2(vDataInvoice.Rows[0].CurSCDiscountExempt).toFixed(2)));
                pIU.Add('CurTotalVATSales', CDec2(CDec2(dtCurrZRead.Rows[0].CurTotalVATSales).toFixed(2)) + (CDec2(CDec2(vDataInvoice.Rows[0].CurTotalVatablePortionOfSCDTransaction).toFixed(2)) + CDec2(CDec2(vDataInvoice.Rows[0].CurTotalVATSales).toFixed(2))));
                pIU.Add('CurTotalVAT', CDec2(CDec2(dtCurrZRead.Rows[0].CurTotalVAT).toFixed(2)) + (CDec2(CDec2(vDataInvoice.Rows[0].CurTotalVATAmountPortionOfSCDTransaction).toFixed(2)) + CDec2(CDec2(vDataInvoice.Rows[0].CurTotalVATAmount).toFixed(2))));
                pIU.Add('CurVATExempt', CDec2(CDec2(dtCurrZRead.Rows[0].CurVATExempt).toFixed(2)) + CDec2(CDec2(vDataInvoice.Rows[0].CurVATExemptTax).toFixed(2)));
                pIU.Add('CurLocalTax', CDec2(CDec2(dtCurrZRead.Rows[0].CurLocalTax).toFixed(2)) + CDec2(CDec2(vDataInvoice.Rows[0].CurLocalTax).toFixed(2)));
                pIU.Add('CurServiceCharge', CDec2(CDec2(dtCurrZRead.Rows[0].CurServiceCharge).toFixed(2)) + CDec2(CDec2(vDataInvoice.Rows[0].CurServiceCharge).toFixed(2)));
                pIU.Add('ChargeAmount', CDec2(CDec2(dtCurrZRead.Rows[0].ChargeAmount).toFixed(2)) + CDec2(CDec2(vDataPayments.Rows[0].ChargeAmount).toFixed(2)));
                pIU.Add('CurNoTAXSales', CDec2(CDec2(dtCurrZRead.Rows[0].CurNoTAXSales).toFixed(2)) + CDec2(CDec2(vDataInvoice.Rows[0].CurNoTAXSales).toFixed(2)));
                pIU.Add('CurVATExemptSales', CDec2(CDec2(dtCurrZRead.Rows[0].CurVATExemptSales).toFixed(2)) + CDec2(CDec2(vDataInvoice.Rows[0].CurVATExemptSales).toFixed(2)));
                pIU.Add('CurZeroRatedVATSales', CDec2(CDec2(dtCurrZRead.Rows[0].CurZeroRatedVATSales).toFixed(2)) + CDec2(CDec2(vDataInvoice.Rows[0].CurZeroRatedVATSales).toFixed(2)));
                pIU.Add('CurOthers', CDec2(CDec2(dtCurrZRead.Rows[0].CurOthers).toFixed(2)) + CDec2(CDec2(vDataInvoice.Rows[0].CurOthers).toFixed(2)));
                pIU.Add('CashAmount', CDec2(CDec2(dtCurrZRead.Rows[0].CashAmount).toFixed(2)) + CDec2(CDec2(vDataPayments.Rows[0].CashAmount).toFixed(2)));
                pIU.Add('DebitCardAmount', CDec2(CDec2(dtCurrZRead.Rows[0].DebitCardAmount).toFixed(2)) + CDec2(CDec2(vDataPayments.Rows[0].DebitCardAmount).toFixed(2)));
                pIU.Add('CreditCardAmount', CDec2(CDec2(dtCurrZRead.Rows[0].CreditCardAmount).toFixed(2)) + CDec2(CDec2(vDataPayments.Rows[0].CreditCardAmount).toFixed(2)));
                pIU.Add('SMACAmount', CDec2(CDec2(dtCurrZRead.Rows[0].SMACAmount).toFixed(2)) + CDec2(CDec2(vDataPayments.Rows[0].SMACAmount).toFixed(2)));
                pIU.Add('CheckPaymentAmount', CDec2(CDec2(dtCurrZRead.Rows[0].CheckPaymentAmount).toFixed(2)) + CDec2(CDec2(vDataPayments.Rows[0].CheckPaymentAmount).toFixed(2)));
                pIU.Add('OtherTypeAmount', CDec2(CDec2(dtCurrZRead.Rows[0].OtherTypeAmount).toFixed(2)) + CDec2(CDec2(vDataPayments.Rows[0].OtherTypeAmount).toFixed(2)));
                pIU.Add('OnCreditAmount', CDec2(CDec2(dtCurrZRead.Rows[0].OnCreditAmount).toFixed(2)) + CDec2(CDec2(vDataPayments.Rows[0].OnCreditAmount).toFixed(2)));
                pIU.Add('EPlusAmount', CDec2(CDec2(dtCurrZRead.Rows[0].EPlusAmount).toFixed(2)) + CDec2(CDec2(vDataPayments.Rows[0].EPlusAmount).toFixed(2)));
                pIU.Add('VoucherAmount', CDec2(CDec2(dtCurrZRead.Rows[0].VoucherAmount).toFixed(2)) + CDec2(CDec2(vDataPayments.Rows[0].VoucherAmount).toFixed(2)));
                pIU.Add('SRVAmount', CDec2(CDec2(dtCurrZRead.Rows[0].SRVAmount).toFixed(2)) + CDec2(CDec2(vDataPayments.Rows[0].SRVAmount).toFixed(2)));
                pIU.Add('CurEndInvoiceNo', CDec2(vDataInvoice.Rows[0].InvoiceNo));
                pIU.Add('CurTransCountInv', 'IFNULL(CAST(CurTransCountInv AS INTEGER), 0) + 1');
                pIU.Add('CurTransInvItemCount', 'IFNULL(CAST(CurTransInvItemCount AS INTEGER), 0) + ' + dtTransCount);
                                
                pSql = pIU.GetUpdate("ID=" + ReadyString(dtCurrZRead.Rows[0].ID));
                pSql = Sql.Execute(pSql);
            
            } else {
                //new data

                pSql = "Select * FROM CacheZRead " +
                " WHERE date(SalesDate) < " + ReadyString($.format.date(mDate, 'yyyy-MM-dd')) +
                " AND FOWSTID = " + ReadyString(sessionStorage.FowstID) +
                " AND DeviceID = " + ReadyString(Web.DeviceID()) +
                " Order by SalesDate desc limit 1"

                dtPrevZRead = Sql.Execute(pSql);
                                
                pIU.Add('ID', ReadyString(NewID()));
                pIU.Add('DeviceID', ReadyString(Web.DeviceID()));
                pIU.Add('FOWSTID', ReadyString(sessionStorage.FowstID));                

                if (dtPrevZRead.Rows.length > 0) {
                    if (CDec2(dtPrevZRead.Rows[0].PresentationPrevGrossSales) == 0) {
                        pIU.Add('PresentationPrevGrossSales', CDec2(CDec2(dtPrevZRead.Rows[0].PrevGrossSales).toFixed(2)) + CDec2(CDec2(dtPrevZRead.Rows[0].CurGrossSales).toFixed(2)))                        
                    } else {
                        pIU.Add('PresentationPrevGrossSales', CDec2(CDec2(dtPrevZRead.Rows[0].PresentationPrevGrossSales).toFixed(2)) + CDec2(CDec2(dtPrevZRead.Rows[0].CurGrossSales).toFixed(2)))                        
                    }

                    pIU.Add('BegBal', CDec2(dtPrevZRead.Rows[0].BegBal).toFixed(2));
                    pIU.Add('PrevGrossSales', CDec2(CDec2(dtPrevZRead.Rows[0].PrevGrossSales).toFixed(2)) + CDec2(CDec2(dtPrevZRead.Rows[0].CurGrossSales).toFixed(2)))
                    pIU.Add('PrevTotalDiscount', CDec2(CDec2(dtPrevZRead.Rows[0].PrevTotalDiscount).toFixed(2)) + CDec2(CDec2(dtPrevZRead.Rows[0].CurTotalDiscount).toFixed(2)))
                    pIU.Add('PrevSalesReturnRefund', CDec2(CDec2(dtPrevZRead.Rows[0].PrevSalesReturnRefund).toFixed(2)) + CDec2(CDec2(dtPrevZRead.Rows[0].CurSalesReturnRefund).toFixed(2)))
                    pIU.Add('PrevSalesReturnExchange', CDec2(CDec2(dtPrevZRead.Rows[0].PrevSalesReturnExchange).toFixed(2)) + CDec2(CDec2(dtPrevZRead.Rows[0].CurSalesReturnExchange).toFixed(2)))
                    pIU.Add('PrevTotalVAT', CDec2(CDec2(dtPrevZRead.Rows[0].PrevTotalVAT).toFixed(2)) + CDec2(CDec2(dtPrevZRead.Rows[0].CurTotalVAT).toFixed(2)))
                    pIU.Add('PrevServiceCharge', CDec2(CDec2(dtPrevZRead.Rows[0].PrevServiceCharge).toFixed(2)) + CDec2(CDec2(dtPrevZRead.Rows[0].CurServiceCharge).toFixed(2)))
                    pIU.Add('PrevChargeAmount', CDec2(CDec2(dtPrevZRead.Rows[0].PrevChargeAmount).toFixed(2)) + CDec2(CDec2(dtPrevZRead.Rows[0].ChargeAmount).toFixed(2)))
                    pIU.Add('PrevSCDiscount', CDec2(CDec2(dtPrevZRead.Rows[0].PrevSCDiscount).toFixed(2)) + CDec2(CDec2(dtPrevZRead.Rows[0].CurSCDiscount).toFixed(2)))
                    pIU.Add('PrevSoloParentDiscount', CDec2(CDec2(dtPrevZRead.Rows[0].PrevSoloParentDiscount).toFixed(2)) + CDec2(CDec2(dtPrevZRead.Rows[0].CurSoloParentDiscount).toFixed(2)))
                    pIU.Add('PrevPhNatTeamDiscount', CDec2(CDec2(dtPrevZRead.Rows[0].PrevPhNatTeamDiscount).toFixed(2)) + CDec2(CDec2(dtPrevZRead.Rows[0].CurPhNatTeamDiscount).toFixed(2)))
                    pIU.Add('PrevSCDiscountExempt', CDec2(CDec2(dtPrevZRead.Rows[0].PrevSCDiscountExempt).toFixed(2)) + CDec2(CDec2(dtPrevZRead.Rows[0].CurSCDiscountExempt).toFixed(2)))
                    pIU.Add('PrevVATExempt', CDec2(CDec2(dtPrevZRead.Rows[0].PrevVATExempt).toFixed(2)) + CDec2(CDec2(dtPrevZRead.Rows[0].CurVATExempt).toFixed(2)))
                    pIU.Add('PrevSRVAmount', CDec2(CDec2(dtPrevZRead.Rows[0].PrevSRVAmount).toFixed(2)) + CDec2(CDec2(dtPrevZRead.Rows[0].SRVAmount).toFixed(2)));

                }
                else {
                    //new data
                    
                    pIU.Add('PresentationPrevGrossSales', 0)
                    pIU.Add('PrevGrossSales', 0);
                    pIU.Add('PrevTotalDiscount', 0);
                    pIU.Add('PrevSalesReturnRefund', 0);
                    pIU.Add('PrevSalesReturnExchange', 0);
                    pIU.Add('PrevTotalVAT', 0);
                    pIU.Add('PrevServiceCharge', 0);
                    pIU.Add('PrevChargeAmount', 0);
                    pIU.Add('PrevSCDiscount', 0);
                    pIU.Add('PrevSoloParentDiscount', 0);
                    pIU.Add('PrevPhNatTeamDiscount', 0);
                    pIU.Add('PrevSCDiscountExempt', 0);
                    pIU.Add('PrevVATExempt', 0);
                    pIU.Add('PrevSRVAmount', 0);
                }
                
                pIU.Add('SalesDate', ReadyString($.format.date(mDate, 'yyyy-MM-dd')));
                pIU.Add('PresentationCurrGrossSales', CDec2(vDataInvoice.Rows[0].CurGrossSales).toFixed(2));
                pIU.Add('CurGrossSales', CDec2(vDataInvoice.Rows[0].CurGrossSales).toFixed(2));
                pIU.Add('CurTotalDiscount', CDec2(vDataInvoice.Rows[0].CurTotalDiscount).toFixed(2));
                pIU.Add('CurSoloParentDiscount', CDec2(vDataInvoice.Rows[0].CurSoloParentDiscount).toFixed(2));
                pIU.Add('CurPhNatTeamDiscount', CDec2(vDataInvoice.Rows[0].CurPhNatTeamDiscount).toFixed(2));
                pIU.Add('CurSCDiscount', CDec2(vDataInvoice.Rows[0].CurSCDiscount).toFixed(2));
                pIU.Add('CurSCDiscountExempt', CDec2(vDataInvoice.Rows[0].CurSCDiscountExempt).toFixed(2));
                pIU.Add('CurTotalVATSales', CDec2(CDec2(vDataInvoice.Rows[0].CurTotalVatablePortionOfSCDTransaction).toFixed(2)) + CDec2(CDec2(vDataInvoice.Rows[0].CurTotalVATSales).toFixed(2)));
                pIU.Add('CurTotalVAT', (CDec2(CDec2(vDataInvoice.Rows[0].CurTotalVATAmountPortionOfSCDTransaction).toFixed(2)) + CDec2(CDec2(vDataInvoice.Rows[0].CurTotalVATAmount).toFixed(2))));
                pIU.Add('CurVATExempt', CDec2(vDataInvoice.Rows[0].CurVATExemptTax).toFixed(2));
                pIU.Add('CurLocalTax', CDec2(vDataInvoice.Rows[0].CurLocalTax).toFixed(2));
                pIU.Add('CurServiceCharge', CDec2(vDataInvoice.Rows[0].CurServiceCharge).toFixed(2));
                pIU.Add('ChargeAmount', CDec2(vDataPayments.Rows[0].ChargeAmount).toFixed(2))
                pIU.Add('CurNoTAXSales', CDec2(vDataInvoice.Rows[0].CurNoTAXSales).toFixed(2));
                pIU.Add('CurVATExemptSales', CDec2(vDataInvoice.Rows[0].CurVATExemptSales).toFixed(2));
                pIU.Add('CurZeroRatedVATSales', CDec2(vDataInvoice.Rows[0].CurZeroRatedVATSales).toFixed(2));
                pIU.Add('CurOthers', CDec2(vDataInvoice.Rows[0].CurOthers).toFixed(2));
                pIU.Add('CashAmount', CDec2(vDataPayments.Rows[0].CashAmount).toFixed(2));
                pIU.Add('DebitCardAmount', CDec2(vDataPayments.Rows[0].DebitCardAmount).toFixed(2));
                pIU.Add('CreditCardAmount', CDec2(vDataPayments.Rows[0].CreditCardAmount).toFixed(2));
                pIU.Add('SMACAmount', CDec2(vDataPayments.Rows[0].SMACAmount).toFixed(2));
                pIU.Add('CheckPaymentAmount', CDec2(vDataPayments.Rows[0].CheckPaymentAmount).toFixed(2));
                pIU.Add('OtherTypeAmount', CDec2(vDataPayments.Rows[0].OtherTypeAmount).toFixed(2));
                pIU.Add('OnCreditAmount', CDec2(vDataPayments.Rows[0].OnCreditAmount).toFixed(2));
                pIU.Add('EPlusAmount', CDec2(vDataPayments.Rows[0].EPlusAmount).toFixed(2));
                pIU.Add('VoucherAmount', CDec2(vDataPayments.Rows[0].VoucherAmount).toFixed(2));
                pIU.Add('SRVAmount', CDec2(vDataPayments.Rows[0].SRVAmount).toFixed(2));
                pIU.Add('CurBegInvoiceNo', CDec2(vDataInvoice.Rows[0].InvoiceNo));
                pIU.Add('CurEndInvoiceNo', CDec2(vDataInvoice.Rows[0].InvoiceNo));

                pIU.Add('CurTransInvItemCount', CDec2(vDataInvoice.Rows[0].CurTransInvItemCount));
                pIU.Add('CurTransCountMemoReturnExchange', 0);
                pIU.Add('CurSalesReturnExchangeItemCount', 0);
                pIU.Add('CurTransCountMemoRefund', 0);
                pIU.Add('CurSalesRefundItemCount', 0);
                pIU.Add('CurTransCountInv', 1);
                pIU.Add('CurTransInvItemCount', dtTransCount);

                pIU.Add('CurVoid', pCurVoid);
                pIU.Add('CurVoidItemQty', pCurVoidItemQty);

                pSql = pIU.GetInsert();
                pSql = Sql.Execute(pSql);

                //pIU.Add("PrevGrossSales", CDec2(dtPrevZRead.Rows[0].PrevGrossSales) + CDec2(dtPrevZRead.Rows[0].CurGrossSales))
                //pIU.Add("PrevTotalDiscount", CDec2(dtPrevZRead.Rows[0].PrevTotalDiscount) + CDec2(dtPrevZRead.Rows[0].CurTotalDiscount))
                //pIU.Add("PrevSalesReturnRefund", CDec2(dtPrevZRead.Rows[0].PrevSalesReturnRefund) + CDec2(dtPrevZRead.Rows[0].CurSalesReturnRefund))
                //pIU.Add("PrevSalesReturnExchange", CDec2(dtPrevZRead.Rows[0].PrevSalesReturnExchange) + CDec2(dtPrevZRead.Rows[0].CurSalesReturnExchange))
                //pIU.Add("PrevTotalVAT", CDec2(dtPrevZRead.Rows[0].PrevTotalVAT) + CDec2(dtPrevZRead.Rows[0].CurTotalVAT))
                //pIU.Add("PrevServiceCharge", CDec2(dtPrevZRead.Rows[0].PrevServiceCharge) + CDec2(dtPrevZRead.Rows[0].CurServiceCharge))
                //pIU.Add("PrevChargeAmount", CDec2(dtPrevZRead.Rows[0].PrevChargeAmount) + CDec2(dtPrevZRead.Rows[0].ChargeAmount))

                //pIU.Add('PresentationCurrGrossSales', CDec2(dtPOSTransaction.Rows[0].CurGrossSales));
                //pIU.Add('CurGrossSales', CDec2(dtPOSTransaction.Rows[0].CurGrossSales));
                //pIU.Add('CurTotalDiscount', CDec2(dtPOSTransaction.Rows[0].CurTotalDiscount));
                //pIU.Add('CurTotalVATSales', CDec2(dtPOSTransaction.Rows[0].CurTotalVATSales));
                //pIU.Add('CurTotalVAT', CDec2(dtPOSTransaction.Rows[0].CurTotalVAT));
                //pIU.Add('CurLocalTax', CDec2(dtPOSTransaction.Rows[0].CurLocalTax));
                //pIU.Add('CurServiceCharge', CDec2(dtPOSTransaction.Rows[0].CurServiceCharge));
                //pIU.Add('CurNoTAXSales', CDec2(dtPOSTransaction.Rows[0].CurNoTAXSales));
                //pIU.Add('CurVATExemptSales', CDec2(dtPOSTransaction.Rows[0].CurVATExemptSales));
                //pIU.Add('CurZeroRatedVATSales', CDec2(dtPOSTransaction.Rows[0].CurZeroRatedVATSales));
                //pIU.Add('CurOthers', CDec2(dtPOSTransaction.Rows[0].CurOthers));
                //pIU.Add('CashAmount', CDec2(dtPOSTransaction.Rows[0].CashAmount));
                //pIU.Add('DebitCardAmount', CDec2(dtPOSTransaction.Rows[0].DebitCardAmount));
                //pIU.Add('CreditCardAmount', CDec2(dtPOSTransaction.Rows[0].CreditCardAmount));
                //pIU.Add('SMACAmount', CDec2(dtPOSTransaction.Rows[0].SMACAmount));
                //pIU.Add('CheckPaymentAmount', CDec2(dtPOSTransaction.Rows[0].CheckPaymentAmount));
                //pIU.Add('OtherTypeAmount', CDec2(dtPOSTransaction.Rows[0].OtherTypeAmount));
                //pIU.Add('EPlusAmount', CDec2(dtPOSTransaction.Rows[0].EPlusAmount));
                //pIU.Add('VoucherAmount', CDec2(dtPOSTransaction.Rows[0].VoucherAmount));
                //pIU.Add('CurBegInvoiceNo', CDec2(dtPrevZRead.Rows[0].CurEndInvoiceNo) + 1);
                //pIU.Add('CurEndInvoiceNo', CDec2(dtPrevZRead.Rows[0].CurEndInvoiceNo) + 1);
                //pIU.Add('CurTransCountInv', 1);
                //pIU.Add('CurTransInvItemCount', CDec2(dtPOSTransaction.Rows[0].CurTransInvItemCount));
                //pIU.Add('CurTransCountMemoReturnExchange', 0);
                //pIU.Add('CurSalesReturnExchangeItemCount', 0);
                //pIU.Add('CurTransCountMemoRefund', 0);
                //pIU.Add('CurSalesRefundItemCount', 0);
                //pIU.Add('CurTermNum', CDec2(dtPrevZRead.Rows[0].CurTermNum));
                //pIU.Add('FowstNo', CDec2(dtPrevZRead.Rows[0].FowstNo));
            }

            if (proj.ReconstructSalesDataMode) {
                mDate = proj.GetZReadDate();
                var rsDate = new Date($('#txtRSDDate').val());
                var diffdays = Math.ceil(Math.abs(rsDate - mDate) / (1000 * 60 * 60 * 24));

                for (var i = 0; i < diffdays; i++) {
                    rsDate = new Date(new Date().setDate(rsDate.getDate() + 1)); // get next zread date
                    pSql = "Select * FROM CacheZRead " +
                            " WHERE date(SalesDate) = " + ReadyString($.format.date(rsDate, 'yyyy-MM-dd')) +
                            " AND FOWSTID = " + ReadyString(sessionStorage.FowstID) +
                            " AND DeviceID = " + ReadyString(Web.DeviceID()) +
                            " Order by SalesDate desc limit 1"
                    var dtNextZreadDate = Sql.Execute(pSql);

                    if (dtNextZreadDate.Rows.length > 0) {
                        pIU = new InsertUpdate('CacheZRead');

                        if (CDec2(dtNextZreadDate.Rows[0].PresentationPrevGrossSales) == 0) {
                            pIU.Add('PresentationPrevGrossSales', CDec2(dtNextZreadDate.Rows[0].PrevGrossSales) + CDec2(vDataInvoice.Rows[0].CurGrossSales))
                        } else {
                            pIU.Add('PresentationPrevGrossSales', CDec2(dtNextZreadDate.Rows[0].PresentationPrevGrossSales) + CDec2(vDataInvoice.Rows[0].CurGrossSales))
                        }

                        pIU.Add('PrevGrossSales', CDec2(dtNextZreadDate.Rows[0].PrevGrossSales) + CDec2(vDataInvoice.Rows[0].CurGrossSales))
                        pIU.Add('PrevTotalDiscount', CDec2(dtNextZreadDate.Rows[0].PrevTotalDiscount) + CDec2(vDataInvoice.Rows[0].CurTotalDiscount))
                        //pIU.Add('PrevSalesReturnRefund', CDec2(dtNextZreadDate.Rows[0].PrevSalesReturnRefund) + CDec2(vDataInvoice.Rows[0].CurSalesReturnRefund))
                        //pIU.Add('PrevSalesReturnExchange', CDec2(dtNextZreadDate.Rows[0].PrevSalesReturnExchange) + CDec2(vDataInvoice.Rows[0].CurSalesReturnExchange))
                        pIU.Add('PrevTotalVAT', CDec2(dtNextZreadDate.Rows[0].PrevTotalVAT) + (CDec2(vDataInvoice.Rows[0].CurSCDiscount) > 0 ? 0 : CDec2(vDataInvoice.Rows[0].CurTotalVATAmount)))
                        pIU.Add('PrevServiceCharge', CDec2(dtNextZreadDate.Rows[0].PrevServiceCharge) + CDec2(vDataInvoice.Rows[0].CurServiceCharge))
                        pIU.Add('PrevChargeAmount', CDec2(dtNextZreadDate.Rows[0].PrevChargeAmount) + CDec2(vDataInvoice.Rows[0].ChargeAmount))
                        pIU.Add('PrevSoloParentDiscount', CDec2(dtNextZreadDate.Rows[0].PrevSoloParentDiscount) + CDec2(vDataInvoice.Rows[0].CurSoloParentDiscount))
                        pIU.Add('PrevPhNatTeamDiscount', CDec2(dtNextZreadDate.Rows[0].PrevPhNatTeamDiscount) + CDec2(vDataInvoice.Rows[0].CurPhNatTeamDiscount))
                        pIU.Add('PrevSCDiscount', CDec2(dtNextZreadDate.Rows[0].PrevSCDiscount) + CDec2(vDataInvoice.Rows[0].CurSCDiscount))
                        pIU.Add('PrevSCDiscountExempt', CDec2(dtNextZreadDate.Rows[0].PrevSCDiscountExempt) + CDec2(vDataInvoice.Rows[0].CurSCDiscountExempt))
                        pIU.Add('PrevVATExempt', CDec2(dtNextZreadDate.Rows[0].PrevVATExempt) + CDec2(vDataInvoice.Rows[0].CurVATExempt))

                        Sql.Execute(pIU.GetUpdate(" ID = " + ReadyString(dtNextZreadDate.Rows[0].ID)));
                    }
                }
            }
        }
        ////
        //////////////////////////////////////////////////

        this.GetSalesReps = function(vInvoiceID) {
            var pSql = "SELECT DISTINCT TransInvoiceItemSalesReps.PeopleID " +
                ",IFNULL(LibraryPeople.Surname, \'\') " +
                " || \',\' || IFNULL(LibraryPeople.FirstName, \'\') " +
                " || \',\' || IFNULL(LibraryPeople.MiddleName, \'\') as PeopleName " +
                " FROM private.TransInvoiceItemSalesReps " +
                " JOIN LibraryPeople LibraryPeople ON TransInvoiceItemSalesReps.PeopleID = LibraryPeople.ID" +
                " WHERE TransInvoiceItemSalesReps.InvoiceID = " + ReadyString(vInvoiceID);

            var pOut = Sql.Execute(pSql);

            return pOut;
        };

        this.IsRequiredSalesRep = function (vInvoiceID) {
            var pSql = 'SELECT DefaultValue from private.SystemDefaults where Description = \'RequireSalesRep\''

            var pOut = Sql.ExecuteScalar(pSql);

            return CBool(pOut);
        };

        return this;

    }; // SalesPayment_offline

});