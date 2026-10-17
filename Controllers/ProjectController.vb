Namespace WebProj
    Public Class ProjectController
        Inherits System.Web.Mvc.Controller

        Dim mFinder As Finder
        Dim clsProjectPO As New ProjectPurchaseOrder

        Function Index() As ActionResult
            Return View()
        End Function

        Function Receiving() As ActionResult
            Return View()
        End Function

        <HttpPost()>
        Function GetProjectFowst() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pFinder As Finder
            Dim pDT As DataTable
            Dim pDeviceSerial As String
            Dim strOrgID As String

            pFinder = New Finder

            With pFinder
                .ProData = Common.WebRequest.ProData
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
                    pDeviceSerial = Common.WebRequest.Data("DeviceSerial")
                    If CStr2(pDeviceSerial).Trim() = "" Then
                        pDeviceSerial = WebSession.Instance.DeviceSerial
                    End If
                    .SqlFrom = "FROM dbo.LibraryOrganizationEmployee INNER JOIN " &
                          "dbo.LibraryOrganizationEmployeeFOWST ON dbo.LibraryOrganizationEmployee.PeopleID = dbo.LibraryOrganizationEmployeeFOWST.PeopleID INNER JOIN " &
                          "dbo.MobizLibraryOrganizationFOWST AS fowst ON dbo.LibraryOrganizationEmployeeFOWST.FowstID = fowst.ID" & vbCrLf &
                          " JOIN LibraryDeviceFowst ON LibraryDeviceFowst.FowstID = fowst.ID" & vbCrLf &
                          " JOIN LibraryDevice ON LibraryDevice.ID = LibraryDeviceFowst.DeviceID" & vbCrLf &
                          " LEFT OUTER JOIN dbo.MobizLibraryCities AS cty ON fowst.FowstCityID = cty.ID LEFT OUTER JOIN " &
                          "dbo.MobizLibraryCountries AS ctry ON fowst.FowstCountryID = ctry.ID"
                    .SqlWhere = " WHERE fowst.OrganizationID = " & ReadyString(strOrgID) & " AND dbo.LibraryOrganizationEmployee.PeopleID = " & ReadyString(WebSession.Instance.PeopleUserID) & " AND dbo.LibraryOrganizationEmployee.Deactivated = 0" &
                        " " & vbCrLf &
                        "    AND LibraryDeviceFowst.IsActive = 1"
                Else
                    .SqlFrom = "FROM dbo.MobizLibraryOrganizationFOWST fowst " &
                    " LEFT OUTER JOIN dbo.MobizLibraryCities cty ON (fowst.FowstCityID = cty.ID) " &
                    " LEFT OUTER JOIN dbo.MobizLibraryCountries ctry ON (fowst.FowstCountryID = ctry.ID)"

                    .SqlWhere = " WHERE fowst.OrganizationID = " & ReadyString(strOrgID) & vbCrLf
                End If

                .SqlOrder = "ORDER BY FowstName, Address"

                pDT = .Requery
            End With

            Return JsonDictionary.Serialize("Results", pDT)

        End Function
    End Class
End Namespace

