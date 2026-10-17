Public Class SecurityAccess

    Public Const SysAdminRoleID As String = "474F9F49-2876-4391-A005-107EB18F2762"

    Private Shared clsInstance As SecurityAccess

    Public Shared ReadOnly Property Instance As SecurityAccess
        Get
            If (clsInstance Is Nothing) Then
                clsInstance = New SecurityAccess()
            End If
            Return clsInstance
        End Get
    End Property

    Public Enum enmSecurityModules
        enmSecurityRoles
        enmCompanyProfile
        enmCompanyPlaces
        enmCompanyEmployee
        enmCompanyCustomer
        enmCompanySupplier
        enmShipper
        enmPOSReceiptSettingsConfiguration
        enmPOSReceiptSettingsReprint
        enmSalesReturn
    End Enum

    Public Function getSecurityModule(ByVal vSecurityModule As enmSecurityModules) As String

        Dim strReturn As String = ""

        If (vSecurityModule = enmSecurityModules.enmSecurityRoles) Then
            strReturn = "Security Roles"
        ElseIf (vSecurityModule = enmSecurityModules.enmCompanyProfile) Then
            strReturn = "Company Profile"
        ElseIf (vSecurityModule = enmSecurityModules.enmCompanyPlaces) Then
            strReturn = "Company Places"
        ElseIf (vSecurityModule = enmSecurityModules.enmCompanyEmployee) Then
            strReturn = "Company Employee"
        ElseIf (vSecurityModule = enmSecurityModules.enmCompanyCustomer) Then
            strReturn = "Company Customer"
        ElseIf (vSecurityModule = enmSecurityModules.enmCompanySupplier) Then
            strReturn = "Company Supplier"
        ElseIf (vSecurityModule = enmSecurityModules.enmShipper) Then
            strReturn = "Shipper"
        ElseIf (vSecurityModule = enmSecurityModules.enmPOSReceiptSettingsConfiguration) Then
            strReturn = "POS Receipt Settings Configuration"
        ElseIf (vSecurityModule = enmSecurityModules.enmPOSReceiptSettingsReprint) Then
            strReturn = "POS Receipt Settings Reprint"
        ElseIf (vSecurityModule = enmSecurityModules.enmSalesReturn) Then
            strReturn = "Sales Return"
        End If

        Return strReturn

    End Function

    Function CheckAccessRights(ByVal vUserAccessID As String, ByVal vSecurityModule As enmSecurityModules, ByVal IsRedirectToLogIn As Boolean) As Boolean
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim blnRet As Boolean = False

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("IsHasAccess", SqlDbType.Bit)
                    .Expression = "secrolemod.IsHasAccess"
                End With
            End With
            .SqlFrom = " FROM LibrarySecurityRoleModules secrolemod " &
                " INNER JOIN LibrarySecurityModule secmod ON (secrolemod.ModuleID = secmod.ID) " &
                " INNER JOIN LibrarySecurityRole secrole ON (secrolemod.RoleID = secrole.ID) " &
                " INNER JOIN LibrarySecurityRoleUser secroleusr ON (secrole.ID = secroleusr.RoleID) "

            .SqlWhere = "WHERE secrole.IsActive = 1 " &
                " AND secrolemod.IsHasAccess = 1 " &
                " AND secroleusr.UserAccountID = " & ReadyString(vUserAccessID) &
                " AND secmod.ModuleName = " & ReadyString(getSecurityModule(vSecurityModule))

            pDT = .Requery

        End With

        If (pDT.Rows.Count > 0) Then
            blnRet = True
        Else
            If (IsRedirectToLogIn) Then
                WebRequest.Instance.RedirectToLogin(True)
            End If
        End If

        Return blnRet

    End Function

    Function CheckAccessRights(ByVal vUserAccessID As String, ByVal vSecurityModule As String, ByVal IsRedirectToLogIn As Boolean) As Boolean
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim blnRet As Boolean = False

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("IsHasAccess", SqlDbType.Bit)
                    .Expression = "secrolemod.IsHasAccess"
                End With
            End With
            .SqlFrom = " FROM LibrarySecurityRoleModules secrolemod " &
                " INNER JOIN LibrarySecurityModule secmod ON (secrolemod.ModuleID = secmod.ID) " &
                " INNER JOIN LibrarySecurityRole secrole ON (secrolemod.RoleID = secrole.ID) " &
                " INNER JOIN LibrarySecurityRoleUser secroleusr ON (secrole.ID = secroleusr.RoleID) "

            .SqlWhere = "WHERE secrole.IsActive = 1 " &
                " AND secrolemod.IsHasAccess = 1 " &
                " AND secroleusr.PeopleID = " & ReadyString(vUserAccessID) &
                " AND secmod.ModuleName = " & ReadyString(vSecurityModule)

            pDT = .Requery

        End With

        If (pDT.Rows.Count > 0) Then
            blnRet = True
        Else
            If (IsRedirectToLogIn) Then
                WebRequest.Instance.RedirectToLogin(True)
            End If
        End If

        Return blnRet

    End Function

    Public Function AddSecurityModules(vProData As ProData)

        Dim pSecurityModuleMain As SecurityModuleMain
        Dim pSecurityModule As SecurityModule
        Dim pProjectPermission As ProjectPermission

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "A02F86C9-8CE6-4977-96D8-0203211C9F48"
            .Name = "Requests and Approvals"
            .ProData = vProData
            With .Modules
                .Add("D91C51A2-4BC5-42BF-9256-F439D6C71C61", "View all requests")
                .Add("52A0CC9D-8963-4690-B155-F576593A9D1A", "View and Approve all requests")
                .Add("4FF1D40D-D31C-40D8-AF2A-DC894B068F50", "View group requests")
                .Add("7D2841E6-B2DD-43F4-890A-657B224A53D1", "View and Approve group requests")
                .Add("53C891EA-98F2-43A1-AB04-5CA7ADFC8B9B", "View branch requests only")
                .Add("CE255CD1-76F1-498F-BFA2-3F7D20C6A69C", "View and Approve branch requests")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "81BA8822-203E-4DE0-9B0D-A6B2AAB8ABA3"
            .Name = "Sales Returns"
            .ProData = vProData
            With .Modules
                .Add("49557584-41E9-4BB9-B4AB-D7849AEDEFE8", "Ask for Approval")
                .Add("0E67B8DE-6609-4E70-A01D-F48D7F614897", "Allow Merchandise Exchange")
                .Add("F774A78C-FCC1-4172-9993-3D970D08F6B5", "Allow Sales Refund")
                .Add("43E0E3D2-E5A3-4FD2-A702-CDD3211D0E03", "Allow Void Sales")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "ED33178B-1C61-4404-A28F-73288B97B9E4"
            .Name = "POS"
            .ProData = vProData
            With .Modules
                .Add("", "Point Of Sales (POS)")
                .Add("FC600EF0-42A4-4BCA-921B-A53A93FE7864", "Allow removal of cart items")
                .Add("A410EEFC-FBC2-44B7-9CEA-1B3CF7415C27", "Show Credit Term Payment Option")
                .Add("13E14C6B-9429-4D73-B5DD-9C6342BF96D2", "Can go to payments page")
                .Add("67493BDE-6D4F-4DE4-9326-4264E41E0D35", "Recall cart")
                .Add("9275E2D0-3CA0-4655-9304-D70B31A5EB76", "Table view: add sales rep w/o PIN")
                .Add("", "Sales Order Taker")
                .Add("", "Sales Return")
                .Add("28c059e9-9ef5-47b9-81bd-af6d66b83c2c", "Add New Customer")
                .Add("c61159e1-3e20-46ef-9936-83e3e44c4e30", "Adjust Z-Read Date")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "3596A1E1-14DA-4A81-8C62-C231AB8314C7"
            .Name = "Dashboard and Reports"
            .ProData = vProData
            With .Modules
                .Add("a646b683-1524-4bbb-9b26-07e1bcc24e9b", "On the Dashbooards and Reports Main Page, user is allowed to view all main page information?", "Set this to NO if user is limited only to sales reports of groups that are assigned to him or her", 1)
                .Add("d8ca81ce-7252-4817-87ff-785e73e96de9", "Input of Inventory of Payments is Required Prior to Showing Dashboards and On Print Reports?")
                .Add("2d0084be-302c-4c63-9fb9-75abb216e136", "POS Reports")
                .Add("8d0e6a32-dc93-4b4c-850f-977f42c304f2", "Inventory Balance")
                .Add("4af16e8c-cabf-4e67-9aa8-3404763927c5", "Inventory Ledger")
                .Add("82d75463-b05d-4122-b18e-86b835f96bf9", "Stock Receiving and Transfers")
                .Add("B6F73596-335F-48B2-AA56-5A1CE6FBC9B8", "Allow Monthly Sales Report")
                .Add("ca231b83-7b4d-4301-b869-cbdbe7dd16b2", "Allow Special Discount Report")
                .Add("ca231b83-7b4d-4301-b869-cbdbe7dd16bb", "Allow Summarized Sales Report")
                .Add("ca231b83-7b4d-4301-b869-cbdbe7dd16be", "Allow Inventory Movement Report")
                .Add("ca231b83-7b4d-4301-b869-cbdbe7dd16bc", "Allow Hourly Sales Report")
                .Add("b6f38aec-c892-48e6-bcc0-50d1ae80fe2b", "Hide Company (All) option in dashboards filtering")
                .Add("39a3ecfe-7b6f-4897-b79d-c5846a86460a", "Hide By Group/Region option Filtering in dashboards filtering")
                .Add("a1ea93b6-9d18-4c82-a2f9-81256680c623", "Restrict access to the consolidated sales data in the main dashboard")
                .Add("2e040d85-7bf3-46f6-9fc6-b1ee7beb5dbc", "Show Main Sales Dashboard Data Filtering")
                .Add("2eb865d9-57d7-4aca-94d1-03b4fecf6f4e", "Show Management Report as main dashboard report") 'Customize report from WOW
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        pSecurityModule = New SecurityModule
        With pSecurityModule
            .ID = "cc2befbb-6d74-4b40-92df-affd5dd16d82"
            .IsHeader = True
            .Name = "Dashboard Menu"
            .Parent = pSecurityModuleMain
            .Parent.ProData = vProData
            .Parent.ID = "3596A1E1-14DA-4A81-8C62-C231AB8314C7"
            .Commit()
        End With

        Dim accessrights As New List(Of String())

        accessrights.Add(New String() {"5fbbc110-8ba8-419d-8069-66401853cb4d", "Hide Summarized Sales By Store"})
        accessrights.Add(New String() {"b53fe93c-7b2d-42a1-8ec7-eff71de8d83a", "Hide Hourly Sales"})
        accessrights.Add(New String() {"a41ed516-04b1-4a66-a7f4-222943741434", "Hide Monthly Sales"})
        accessrights.Add(New String() {"f3f5078e-e85f-4830-b9fb-d931337398d0", "Hide Sales By SKU"})
        accessrights.Add(New String() {"11b381e3-f914-4b50-bacd-c953f6a2a1d6", "Hide Offline Transaction History"})
        accessrights.Add(New String() {"1b846b0f-7464-446c-94e3-4a3990975c15", "Hide Cashier's Sales Report"})
        accessrights.Add(New String() {"cec2dad9-a1f1-4a5f-a4c0-2964dd5e64e3", "Hide Manager's Sales Report"})
        accessrights.Add(New String() {"8709e903-89ab-4d84-90a8-0168e6b194b5", "Hide Z-Read Report"})
        accessrights.Add(New String() {"d28f7c56-e6d7-478f-bf83-05273d84536c", "Hide Monies Accountability"})
        accessrights.Add(New String() {"ccea36e3-7e56-4688-b3df-7d3a973158dd", "Hide Store Replenishment Processing and Analytics"})
        accessrights.Add(New String() {"e663929c-8d6a-4da4-aa0d-1fb6ae8b912c", "Hide Sales Order Processing and Analytics"})
        accessrights.Add(New String() {"f826ccdc-52ab-4578-9845-d65fcfe17774", "Hide Request and Approvals"})
        accessrights.Add(New String() {"80421c8d-0122-4372-8d96-2405b9ea434a", "Hide GPS Location"})
        accessrights.Add(New String() {"966ca410-c176-42cf-95a1-42cd15ace195", "Hide Inventory Count History"})
        accessrights.Add(New String() {"e9f0dfc2-6f2c-4346-91e8-627d45e282e9", "Hide Inventory Balance"})
        accessrights.Add(New String() {"6b86eaec-d7bc-45e9-a8b6-d04bdb2a0d6e", "Hide Inventory Ledger"})
        accessrights.Add(New String() {"370da412-f423-435f-981e-96d055c33fca", "Hide Inventory Movement"})
        accessrights.Add(New String() {"a2b31bca-4eb4-4ae8-93ad-3f63feae4337", "Hide Physical Inventory and Variance Report"})
        accessrights.Add(New String() {"714eb1bf-d544-4477-ab9d-b7a9f01f576b", "Hide Stock Inventory Levels"})
        accessrights.Add(New String() {"96a90071-15dd-4f5f-afe8-928d45aae9ff", "Hide Stock Receiving and Transfers"})
        accessrights.Add(New String() {"e962cae4-be2a-4544-bb12-a9ec322ab5bc", "Hide Stock Receiving (Delivery Receipt) Report"})
        accessrights.Add(New String() {"d436c19e-5c32-402c-a73b-aa56eef704c3", "Hide Audit Trail"})
        accessrights.Add(New String() {"88cc6898-95c6-4bc4-9651-369944186d31", "Hide Franchise Requests"})
        accessrights.Add(New String() {"add7233f-bceb-42bc-95f3-d7b0228b06ba", "Hide Inventory Form Report"})
        accessrights.Add(New String() {"23d6c2ce-cf26-46b9-a47e-08c12f5c158c", "Hide Report To Principal"})
        accessrights.Add(New String() {"add0cc4a-b7db-45e9-a752-b36966ee7fc2", "Hide SCD and PWD Report"})
        accessrights.Add(New String() {"61561882-e5b7-4af8-90b7-c200a8ac8aaf", "Hide ShopperTrak"})
        accessrights.Add(New String() {"63612dc1-5428-4c28-85f0-87df5da3c027", "Hide Special Discount Report"})
        accessrights.Add(New String() {"2e9f4160-61da-4f2a-a75b-040f0239b2f8", "Hide Special Promo Report"})
        accessrights.Add(New String() {"996ebee3-54b9-4144-879b-05cd7f303dbb", "Hide Sales Return/Refund History"})
        accessrights.Add(New String() {"3e312330-b886-422a-8e59-38327a0129b6", "Hide Ayala Malls Tenant Monitoring Report"})
        accessrights.Add(New String() {"20dc9711-5cd0-480b-bf7a-aac6ca62ff48", "Hide SM Coin Report Tenant Monitoring Report"})
        accessrights.Add(New String() {"e39ec10a-a10a-4638-9763-5f0063f2d900", "Hide Device Monitoring"})
        accessrights.Add(New String() {"662304de-9d38-4684-8e03-424a6b8ecefb", "Hide Product Performance Tracker"})
        accessrights.Add(New String() {"3ff5b132-712e-4ee6-9998-962793a64329", "Hide Consumption Report"})
        accessrights.Add(New String() {"e9cee2de-97bc-4e1f-81f4-895de7c4bf87", "Hide Daily Sales per SKU"})
        accessrights.Add(New String() {"bc6245ad-9a21-4503-9592-a3fa825aeef8", "Hide Daily Sales per Store"})
        accessrights.Add(New String() {"300a1818-dd58-4a83-95bf-714a2b0bf79c", "Hide Consolidated Usage per Day"})
        accessrights.Add(New String() {"f472e23a-0d19-4d82-a49f-ae087a70e656", "Hide Consolidated Ending Balance per Day"})
        accessrights.Add(New String() {"b60b4b6c-0a61-4462-b473-f5e01056a4ef", "Hide Consolidated SCD Report"})
        accessrights.Add(New String() {"d8beccb1-b4b3-4ff9-8381-aa7bf9753501", "Hide Daily Hourly Sales per Store"})
        accessrights.Add(New String() {"157f7df0-e2b4-498a-9687-4ad77151da17", "Hide Consolidated Food Aggregators Report per Store"})
        accessrights.Add(New String() {"b2651564-7f66-4dbc-97c2-69b004b6060a", "Hide Consolidated Promo Report"})
        accessrights.Add(New String() {"febc6aa8-3dd7-4a04-a347-b745c77002b7", "Hide Daily Sales Summary Report"})
        accessrights.Add(New String() {"a5810002-e1d5-43fa-bfab-98d44b28fbef", "Hide Sales and Transaction Count Mix"})
        accessrights.Add(New String() {"02e5f259-c0de-4ab0-bf92-5bfa5a282bfc", "Hide ADS and ATC per Store"})
        accessrights.Add(New String() {"ce81039e-76dd-4696-86b7-8ddfc23aabda", "Hide Hourly Sales By Week Category Report"})
        accessrights.Add(New String() {"094699c4-1a2f-4db5-a04d-b3c0e527a270", "Hide Sales Payment Information"})
        accessrights.Add(New String() {"b4bf51bf-ede9-4654-b97c-0e64cfecc9e6", "Hide E-Sales Report"})
        accessrights.Add(New String() {"d9864e34-1369-45cf-8298-48c7fd99a229", "Hide Invoice List"})
        accessrights.Add(New String() {"eb232510-2e28-4454-9523-a70da1107596", "Hide Crew on Duty"})
        accessrights.Add(New String() {"b29ba623-5981-4294-8113-343443b1954d", "Hide Tenant Monitoring Reports"})
        accessrights.Add(New String() {"61077cfb-51e3-485d-b6a7-02ce5c3a1836", "Hide Product Mix Report"})
        accessrights.Add(New String() {"bb5e0b22-291e-4cf4-a823-d269ca6aa8ae", "Hide Product Mix per Week Report"})
        accessrights.Add(New String() {"a7f41444-b16b-46aa-8ae0-82b43cce70b6", "Hide Flavor Mix Report"})
        accessrights.Add(New String() {"909934b4-453b-4595-b936-f246f0c7985c", "Hide Inventory Report Crew vs AM"})
        accessrights.Add(New String() {"75acad95-2961-4eda-bf63-f9a97ce2e0d8", "Hide Daily Product Mix Report"})
        accessrights.Add(New String() {"4887873e-5553-4810-ab91-1f831e5f0680", "Hide Product Monitoring Monthly Summary"})
        accessrights.Add(New String() {"4887873e-5553-4810-ab91-1f831e5f0681", "Hide Product Monitoring Per Store"})
        accessrights.Add(New String() {"4887873e-5553-4810-ab91-1f831e5f0682", "Hide Product Monitoring Per Store-Product"})
        accessrights.Add(New String() {"184309b2-f302-48cd-8667-0dedcb4acc06", "Hide Weekly Sales Report"})
        accessrights.Add(New String() {"3eba31bf-11ff-44f0-a67f-9fd7bf1f725f", "Hide Combos Product Mix"})
        accessrights.Add(New String() {"53446ad8-e355-45be-a36f-04123dfac268", "Hide Weekly Average Daily Sales Report"})
        accessrights.Add(New String() {"cebe06e4-b78f-4369-abe2-5cae64195d3c", "Hide Weekly Transaction Count Report"})
        accessrights.Add(New String() {"56f319f7-6f0d-4436-b552-3a3300663912", "Hide Weekly Average Check Report"})
        accessrights.Add(New String() {"d8b4aa81-1478-46f1-99cb-488bb5604257", "Hide Daily Sales by Hour Report"})
        accessrights.Add(New String() {"f6289863-0e90-4400-b6d4-29c71d313bb5", "Hide Management Monthly Sales Report"})
        accessrights.Add(New String() {"63090f04-e3ae-4f00-bc3e-850cdf607f3d", "Hide MPOS Utilization"})
        accessrights.Add(New String() {"c66f5071-b5b9-4f63-9199-0bdd582ffb90", "Hide Vouchers Report"})
        accessrights.Add(New String() {"0357caf7-d6e0-4788-9a07-4852c2d36dcb", "Hide Flavor Monitoring per Store"})
        accessrights.Add(New String() {"154ce173-bcca-4265-a82a-ed610373ba37", "Hide E-Journal Report"})
        accessrights.Add(New String() {"9e7d15c0-c728-4d69-9f4c-bc15f4d76fb7", "Hide Void Items Report"})
        accessrights.Add(New String() {"60a1a23f-6450-4cea-8987-5227aa2d29cd", "Show MPOS Sales Dashboard"})

        For i As Integer = 0 To accessrights.Count - 1
            pSecurityModuleMain = New SecurityModuleMain
            pSecurityModule = New SecurityModule
            With pSecurityModule
                .ID = accessrights(i)(0)
                .IsHeader = False
                .SortOrder = i + 1
                .Name = accessrights(i)(1)
                .Parent = pSecurityModuleMain
                .Parent.ProData = vProData
                .Parent.ID = "3596A1E1-14DA-4A81-8C62-C231AB8314C7" 'Dashboard and Reports
                .Header = New SecurityModule
                .Header.ID = "cc2befbb-6d74-4b40-92df-affd5dd16d82" 'Dashboard's Menu
                .Commit()
            End With
        Next

        pSecurityModuleMain = New SecurityModuleMain
        pSecurityModule = New SecurityModule
        With pSecurityModule
            .ID = "c14d332d-9103-4fe0-b6b9-cc0c404637ed"
            .IsHeader = True
            .Name = "Device Monitoring"
            .Parent = pSecurityModuleMain
            .Parent.ProData = vProData
            .Parent.ID = "3596A1E1-14DA-4A81-8C62-C231AB8314C7"
            .Commit()
        End With

        accessrights = New List(Of String())

        accessrights.Add(New String() {"54c1e49f-8d2e-4056-910d-9317d59479f6", "Hide Device Available Report option"})
        accessrights.Add(New String() {"7ee46429-fe37-4330-9bb7-17dd05685e9c", "Hide Device Flushing Status Report option"})
        accessrights.Add(New String() {"5c4c0462-2cff-4cc1-8cc7-efa0688e57ca", "Hide Flushing Queue History Report option"})
        accessrights.Add(New String() {"732f604d-94ca-42c2-bc33-93aa6da0c8d1", "Hide Current Flushing Queue option"})
        accessrights.Add(New String() {"287ef43c-e635-44a4-bf59-66abb357e217", "Show only the assigned FOWST/Group to the user"})

        For i As Integer = 0 To accessrights.Count - 1
            pSecurityModuleMain = New SecurityModuleMain
            pSecurityModule = New SecurityModule
            With pSecurityModule
                .ID = accessrights(i)(0)
                .IsHeader = False
                .SortOrder = i + 1
                .Name = accessrights(i)(1)
                .Parent = pSecurityModuleMain
                .Parent.ProData = vProData
                .Parent.ID = "3596A1E1-14DA-4A81-8C62-C231AB8314C7" 'Dashboard and Reports
                .Header = New SecurityModule
                .Header.ID = "c14d332d-9103-4fe0-b6b9-cc0c404637ed" 'Device Monitoring
                .Commit()
            End With
        Next

        pSecurityModuleMain = New SecurityModuleMain
        pSecurityModule = New SecurityModule
        With pSecurityModule
            .ID = "2d3a3b5f-0bfc-493a-ac2a-d4abc90994db"
            .IsHeader = True
            .Name = "Inventory Movement"
            .Parent = pSecurityModuleMain
            .Parent.ProData = vProData
            .Parent.ID = "3596A1E1-14DA-4A81-8C62-C231AB8314C7"
            .Commit()
        End With

        accessrights = New List(Of String())

        accessrights.Add(New String() {"359561ee-177f-44d5-9126-c109e15c0054", "Hide Column Cost"})
        accessrights.Add(New String() {"972b1d00-e59d-4653-b6e9-5c6ce39fa505", "Hide Column Shortlanded"})

        For i As Integer = 0 To accessrights.Count - 1
            pSecurityModuleMain = New SecurityModuleMain
            pSecurityModule = New SecurityModule
            With pSecurityModule
                .ID = accessrights(i)(0)
                .IsHeader = False
                .SortOrder = i + 1
                .Name = accessrights(i)(1)
                .Parent = pSecurityModuleMain
                .Parent.ProData = vProData
                .Parent.ID = "3596A1E1-14DA-4A81-8C62-C231AB8314C7" 'Dashboard and Reports
                .Header = New SecurityModule
                .Header.ID = "2d3a3b5f-0bfc-493a-ac2a-d4abc90994db" 'Inventory Movement
                .Commit()
            End With
        Next

        pSecurityModuleMain = New SecurityModuleMain
        pSecurityModule = New SecurityModule
        With pSecurityModule
            .ID = "a165c8af-ecc4-4357-ba2e-a8daf0268297"
            .IsHeader = True
            .Name = "Authorized Devices"
            .Parent = pSecurityModuleMain
            .Parent.ProData = vProData
            .Parent.ID = "bcb686e3-185f-45ce-a3cf-fe787a86d532"
            .Commit()
        End With

        accessrights = New List(Of String())

        accessrights.Add(New String() {"dbb70a2a-64de-4b19-8140-27f6d47dfb23", "Allow access to the list of devices"})
        accessrights.Add(New String() {"5b8da79a-c0eb-4601-8a79-aceab947e9df", "Allow adding a new device"})
        accessrights.Add(New String() {"c860d7fe-2f2f-4b30-affb-5bb7f7e85d27", "Allow editing of existing device information"})

        For i As Integer = 0 To accessrights.Count - 1
            pSecurityModuleMain = New SecurityModuleMain
            pSecurityModule = New SecurityModule
            With pSecurityModule
                .ID = accessrights(i)(0)
                .IsHeader = False
                .SortOrder = i + 1
                .Name = accessrights(i)(1)
                .Parent = pSecurityModuleMain
                .Parent.ProData = vProData
                .Parent.ID = "bcb686e3-185f-45ce-a3cf-fe787a86d532" 'System Administration or Settings
                .Header = New SecurityModule
                .Header.ID = "a165c8af-ecc4-4357-ba2e-a8daf0268297" 'Authorize Devices
                .Commit()
            End With

            SetDefaultAccessToAllSecRole(0, accessrights(i)(0), vProData)
        Next

        pSecurityModuleMain = New SecurityModuleMain
        pSecurityModule = New SecurityModule
        With pSecurityModule
            .ID = "c323e92c-ac8e-4956-adb0-33f36b8b50ba"
            .IsHeader = True
            .Name = "Product Information Management (PIM)"
            .Parent = pSecurityModuleMain
            .Parent.ProData = vProData
            .Parent.ID = "bcb686e3-185f-45ce-a3cf-fe787a86d532"
            .Commit()
        End With

        accessrights = New List(Of String())

        accessrights.Add(New String() {"b91bbfbe-0743-446d-b33d-2cf990aa6557", "PIM Settings"})
        accessrights.Add(New String() {"ebe9eb3e-97cf-4ae5-aa05-65e24b68183f", "Company-Wide Product List"})
        accessrights.Add(New String() {"7d6613ea-e9f6-4c27-ba57-2d34e8c7eb3c", "Grant access to modify PIM details"})

        For i As Integer = 0 To accessrights.Count - 1
            pSecurityModuleMain = New SecurityModuleMain
            pSecurityModule = New SecurityModule
            With pSecurityModule
                .ID = accessrights(i)(0)
                .IsHeader = False
                .SortOrder = i + 1
                .Name = accessrights(i)(1)
                .Parent = pSecurityModuleMain
                .Parent.ProData = vProData
                .Parent.ID = "bcb686e3-185f-45ce-a3cf-fe787a86d532" 'System Administration or Settings
                .Header = New SecurityModule
                .Header.ID = "c323e92c-ac8e-4956-adb0-33f36b8b50ba" 'PIM
                .Commit()
            End With

            SetDefaultAccessToAllSecRole(1, accessrights(i)(0), vProData)
        Next

        If vProData.Execute("SELECT ID FROM LibrarySecurityRoleModules WHERE ModuleID = 'a646b683-1524-4bbb-9b26-07e1bcc24e9b' AND RoleID = '474f9f49-2876-4391-a005-107eb18f2762'").Rows.Count = 0 Then
            'SET TO YES BY DEFAULT
            Dim p As New InsertUpdate("LibrarySecurityRoleModules")

            p("ID") = ReadyString(Guid.NewGuid.ToString())
            p("RoleID") = ReadyString("474f9f49-2876-4391-a005-107eb18f2762") 'Super Admin
            p("ModuleID") = ReadyString("a646b683-1524-4bbb-9b26-07e1bcc24e9b") 'On the Dashbooards and Reports Main Page, user is allowed to view all main page information?
            p("IsHasAccess") = 1
            p("LastUpdated") = "getdate()"

            vProData.Execute(p.GetInsert())
        End If

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "81ba8822-203e-4de0-9b0d-a6b2aab8aba4"
            .Name = "PO"
            .ProData = vProData
            With .Modules
                .Add("", "P.O. (Purchase Order)")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "48EA33E8-9991-42A8-8C83-98B71EFB4C05"
            .Name = "Troubleshooting and Maintenance Tools"
            .ProData = vProData
            With .Modules
                .Add("9219C6AF-5B44-413B-9B73-9EA6DF94A64A", "Order Taker Setup")
                .Add("15424a2d-b00b-411f-b19f-62c9191e89c6", "Allow users to view the ZRead report online")
                .Add("bcc4e0b5-3f61-4008-87d3-509be101177c", "Allow users to update data in the cached database")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "BAA58006-EFBF-45E8-8160-D899EE5DF718"
            .Name = "Enterprise Application System (EAS)"
            .ProData = vProData
            With .Modules
                ' .Add("68589FDB-E1D4-4251-B411-A68E8CF1D077", "Sales Quotation (EAS)") ' 2019_05_28__09_10_24_4763 transfered to Revenue > Quotation
                ' .Add("53E6EEC8-FE3E-42F1-AA99-8A2377D7CC4B", "Allow to Free Type Price in Sales Quotation (EAS)") ' 2019_05_28__09_10_24_4763 transfered to Revenue > Quotation
                
                .Add("4C65E7AE-EFA1-476B-904F-E42C364693EF", "Purchasing (EAS)")
                .Add("8A2F35E9-C59D-4F90-BC93-057651BE6016", "Cross Tab Report")
                .Add("026544D6-50B7-4409-BB9F-42347BF51BD8", "Gross Profitability Validation")
                .Add("674fa19c-ec58-4af7-a4c6-61215be32833", "Access Basic Sales Info's tab in Universal Product Information")
                .Add("f7a5cd33-a8bc-4013-91a8-1e9c62b4fe24", "Access Item Requisition's tab in Universal Product Information")
                .Add("8d5bd5c3-2d8e-4643-a368-ed3779528c09", "Access Price History's tab in Universal Product Information")
                .Add("4f915eec-2007-4527-8f03-b896ba5ee2f4", "Access Inventory Ledger's tab in Universal Product Information")
                .Add("ad1551cc-a2a6-4640-a28e-8e60608c372f", "Access Price Matrix's tab in Universal Product Information")
                .Add("0339a7ed-3389-4218-88eb-d957abcb95f5", "Access PO History's tab in Universal Product Information")
                .Add("c7c1bf99-3588-4043-8815-36a12610b3a3", "Access Stock In Transit's tab in Universal Product Information")
                .Add("f3bd2b1b-5a4c-4189-a787-c240cb9fa636", "Access Sales Representatives' tab in Universal Product Information")
                .Add("52ee734e-651a-4115-84c0-e5bbffa2acdb", "Access Vendor Prices' tab in Universal Product Information")
                .Add("3E8A19BB-E7FA-497C-9659-3B78968E417C", "Access Factory, Freight, and Landed Cost in Universal Product Information")
                .Add("e87df838-b909-4142-8be9-c36f769f05d5", "Access Set Beginning Balance's tab in Universal Product Information")
                .Add("420da3f0-1dbe-4d13-a891-3a74d43cc9fd", "Access Inventory Count's tab in Universal Product Information")
                .Add("442b00f7-a7b9-4f3f-8c95-18a672a48ae6", "Access Inventory Count Approval and Disapproval's tab in Universal Product Information")
                .Add("d26f4328-dec6-4ab9-a97a-ff8a6dda8019", "Access Inventory Count Approval and Disapproval History's tab in Universal Product Information")
                .Add("90753dd2-7028-4f32-899c-24632eb23e88", "Access Company Wide Inventory Balance's tab in Universal Product Information")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "505AB19B-236B-4F46-86B2-BCA1002F222A"
            .Name = "Revenue"
            .ProData = vProData
            With .Modules
                .Add("1CAA8D6B-A4F3-4970-97FD-19667C96BD1E", "Main Revenue Dashboards")
                .Add("04D48CEE-D5FB-44EA-8DE0-54DFFE4FAB89", "Main Revenue Settings")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "8AAA458C-D73C-41A4-BC28-592A594097A0"
            .Name = "Quotation"
            .ProData = vProData
            With .Modules
                .Add("68589FDB-E1D4-4251-B411-A68E8CF1D077", "Sales Quotation (EAS)")
                .Add("DF5AD8EA-C5D0-4943-8600-FE8FF2DD8E74", "Access Sales Quotaion Settings*")
                .Add("53E6EEC8-FE3E-42F1-AA99-8A2377D7CC4B", "Allow to Free Type Price in Sales Quotation (EAS)")
                .Add("79CB4055-EFB8-4231-8AA6-0CD37C19865E", "View Profitability Details in Sales Quotation")
                '.Add("", "")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "4CEA31B8-CFDE-41BF-8EA0-304682909516"
            .Name = "Order Processing"
            .ProData = vProData
            With .Modules
                .Add("F130719F-3E34-4457-B1B0-74BD3323C82C", "Sales Order (EAS)")
                .Add("A8DDB45D-F62B-4D94-86E2-54366B8D9D92", "Access Sales Order Settings*")
                .Add("7F0C8B74-30C1-45C0-B909-A1EC26532C68", "Allow to Free Type Price in Sales Order (EAS)")
                .Add("FE4BF3C8-CC35-489D-935B-4BEBAD47B2EB", "Ante Date Sales Order")
                .Add("754CD285-D956-4391-82B0-B6590236F218", "Post Date Sales Order")
                .Add("18516DB5-D917-4DD6-9AD4-8EB075DAB75B", "View Profitability Details in Sales Order")
                .Add("975E2CB0-472E-487A-AFA8-B9756DCA48AE", "Allow Edit of Sales Rep Field")
                '.Add("", "")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "5E0CC3D6-589D-436D-A12B-0C0C3A4F8F5A"
            .Name = "Order Fulfillment"
            .ProData = vProData
            With .Modules
                .Add("102E1F93-5711-4923-9386-840785420954", "Access Order Fulfillment Module*")
                .Add("D7EF5C94-30DE-46B6-A096-6DF1B310B8CF", "Access Order Fulfillment Settings*")
                .Add("A67634CC-F77D-4FFD-BCFB-A8855DD0968B", "Reprint Pick List")
                .Add("4073E001-776B-46A0-98B2-80F7D72A29F7", "Reprint Withdrawal")
                '.Add("", "")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "1C44E443-FF5C-4C4D-89B0-DACD5E800A75"
            .Name = "Invoicing"
            .ProData = vProData
            With .Modules
                .Add("173EEC31-C9B4-4872-8036-C241A57BF2D3", "Sales Invoice (EAS)")
                .Add("22624337-1A50-454D-B7C0-D345882F8B30", "Invoice Settings (EAS)")
                .Add("ECE4F2F7-C112-4D34-81FA-38DD82927B7E", "Set all customers to have unlimited credit limit")
                .Add("1479239D-A5CE-40A3-9B3E-1D752B2AE5F7", "Allow to Free Type Price in Sales Invoice (EAS)")
                .Add("7FD56A89-CF1B-4B1B-9926-F48838ABEB60", "Ante Date Invoice")
                .Add("503F58E2-AB09-4BF4-AE26-B1B016AF0287", "Post Date Invoice")
                .Add("396F32BF-7A44-432E-9346-627AF986EFBA", "Reprint Invoice")
                .Add("3E4911EA-9A1C-46D3-AD78-E90EAA7D7492", "Cancel Invoice (EAS)")
                .Add("E9BB39B2-F22E-4739-B32E-238997586A79", "Modify Finalized Invoice")
                .Add("D4116DB7-B197-4DAF-9D9B-54507D206C77", "View Profitability Details in Sales Invoice")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "4CBBB0F4-1EF9-41E3-8573-A7FD20B0BB5E"
            .Name = "Delivery Tracking"
            .ProData = vProData
            With .Modules
                .Add("7EF4CAFB-9C1B-476C-AD39-23644DDAE93F", "Access Delivery Tracking Module*")
                .Add("78F703BB-9459-4FD6-B11C-2FC91786C111", "Access Delivery Tracking Settings*")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "5B6230B0-339E-411B-AE90-B852A4FAD1A5"
            .Name = "Credit and Collection"
            .ProData = vProData
            With .Modules
                .Add("D00375A2-419B-48C6-9AF8-03E74BB6C68C", "Credit and Collection (EAS)")
                .Add("FA756C66-7477-4F94-A7F6-873710374BAB", "Access Credit and Collection Settings*")
                .Add("E5477BB5-A717-43E0-8D57-E0E0AD9FD375", "Credit and Collection Settings > Grant Credit")
                .Add("C0E3A4EE-7661-4427-BB66-A00C2F2A9017", "Accept Sales Return Without Invoice")
                .Add("418338D1-9BA9-4741-869E-CEAEAC3FE3C4", "Payments (EAS)", vLabel:="Customer Payments (EAS) Read & Write Access")
                .Add("9B3000DE-BE4B-4458-B378-44DE173D8B0C", "Customer Payments (EAS) Read-Only", vLabel:="Customer Payments (EAS) Read-Only Access")
                .Add("9404A381-E057-44FE-A6B5-833D8993DBD5", "Customer Payments (EAS) Cancellation")
                .Add("79E14933-D508-46B9-9AE3-CAEB9A407D47", "Add Customer Financial Details", "I.e. Bank Account, Checking Account, Online Account")
                .Add("9EBD37D1-71AD-4A14-94E0-8733EE5F802C", "Deactivate Customer Financial Details", "I.e. Bank Account, Checking Account, Online Account")
                .Add("6E251B49-5AE6-43C4-B396-5DCEB244ED4E", "Adjustment (EAS)", vLabel:="Issue Debit And Credit Memos to Open And Closed Invoices")
                .Add("E385B5DB-9225-49F6-A56F-1F7FCD394D8D", "Adjustment to Invoices With Payments (EAS)", vLabel:="Issue Debit And Credit Memos to Closed Invoices")
                With .Add("1DCCBE43-B1DD-42DE-81CE-78AB3C475F11", "Reports (EAS)")
                    .DeleteMode = True
                End With
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "23F85CC9-211A-4D7E-A3E1-430F4EB134CC"
            .Name = "Deposits"
            .ProData = vProData
            With .Modules
                .Add("1E6C2B74-E422-4FC0-BBE3-0E861FA06E5C", "Deposits (EAS)")
                .Add("053F82E7-CF86-4532-A576-0D33AAD6C392", "Access Deposits Settings*")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "07254C9C-4926-4F4F-9310-E5229D48F464"
            .Name = "Competitor Price Canvassing"
            .ProData = vProData
            With .Modules
                .Add("9F1447F9-C5F6-4683-9B99-F11FB892CD7B", "Access Competitor Price Canvassing Module*")
                .Add("BFBDD1FD-1B4E-4AD5-98A0-A504A43DF988", "Access Competitor Price Canvassing Settings*")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "A3F37487-1B2E-4206-8F21-9E719F0C088D"
            .Name = "Purchasing"
            .ProData = vProData
            With .Modules
                .Add("915BD439-C3AC-49DE-9A2B-C50E7401E20F", "Access Purchasing (EAS) Module*")
                .Add("5B468E47-3B97-4B7A-B1BB-210455D91500", "Purchasing (EAS) Settings")
                .Add("F51AF06F-A6C6-48CC-B62E-DE7BA7D0FD54", "Allow to Free Type Price in Purchasing (EAS)")
                .Add("4B74ECBF-754A-4098-9CB3-A8F30A200F70", "Allow Approval of Purchase Orders (EAS)")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "18595245-9A1B-4951-A727-7A38C5754C2A"
            .Name = "Job Order"
            .ProData = vProData
            With .Modules
                .Add("3099C86F-8CE8-4836-A7CA-0C9A8C2D44BA", "Access Job Order Module*")
                .Add("6B523E18-968B-488B-B5F2-7B092F58D864", "Access Job Order Settings*")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "BE0A1CCC-5239-4BC1-9FB2-207C1336898B"
            .Name = "Stock Receiving"
            .ProData = vProData
            With .Modules
                .Add("F3E1BC43-59CF-42E2-8BDA-8EBC989C8EC8", "Stock Receiving (EAS)")
                .Add("0CF01143-5CF1-4574-8343-8A0484BE761A", "Access Stock Receiving (EAS) Settings*")
                .Add("195F5240-F118-4B5C-A22A-4F367AB23D83", "Cancel Stock Receiving (EAS)")
                .Add("5C4C111B-5D7A-49CE-AF53-43440F51AAF9", "Don't limit to ordered quantity") ' stock receiving
                .Add("52AB8165-1BFE-47B4-90C7-E89962ECC7CC", "Show cost details in stock receiving") ' stock receiving
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "25207625-AE7D-406E-8D63-439AE038295E"
            .Name = "Accounts Payable Verification"
            .ProData = vProData
            With .Modules
                .Add("D7BAA46D-BD15-4855-9EBB-3258AF3ACCBA", "Accounts Payable (EAS)")
                .Add("ABB6CAEA-2C95-48EC-AB6D-E72E9DF44427", "Accounts Payable (EAS) Settings*")
                .Add("EB072148-7F00-42EC-B33D-21936BD19C7B", "Accounts Payable Verification > Access to Payments", vLabel:="Accounts Payable Verification > Access to Payments Read & Write Access")
                .Add("6296CED0-F444-470C-9611-B6E65427BD7D", "Accounts Payable Verification > Access to Payments Read-Only", vLabel:="Accounts Payable Verification > Access to Payments Read-Only Access")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "CB82BAE0-EFCC-41A5-B46E-72047D765950"
            .Name = "Disbursements"
            .ProData = vProData
            With .Modules
                .Add("884b54d5-104c-47de-b31e-ff5f0e7add27", "Main Disbursement Dashboards")
                .Add("CD9909F5-D7EB-4D16-A6A9-091C6FEC72B4", "Main Disbursement Modules")
                .Add("52C905AA-BC35-497C-84C4-7317A39330B4", "Main Disbursement Settings")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "B91E3B31-ACD8-416D-ABD6-042C1E2E45A9"
            .Name = "Check Clearing"
            .ProData = vProData
            With .Modules
                .Add("8B8A2616-ACD7-4E06-B0F4-B356CAE72DAB", "Access Check Clearing Module*")
                .Add("7ECEB063-6BCC-4092-8119-BC61F7C12934", "Access Check Clearing Settings*")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "5A234152-B1B4-4C94-AA9D-8ABFC44E5B55"
            .Name = "Inventory Handling and Controls"
            .ProData = vProData
            With .Modules
                .Add("8BA508CA-AC0C-4ACD-8735-FD29A62FA6C7", "Inventory Handling and Controls*")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "F4910DDE-0674-44CA-B5D6-86B6439B0FA2"
            .Name = "DTR and HR"
            .ProData = vProData
            With .Modules
                .Add("61D695D6-DBFC-4CE4-9539-20C7C69C14DA", "Access DTR and HR Modules*")
                .Add("F0F8782B-A1F5-44B1-B77A-D6375741EBAC", "Access DTR and HR Settings*")
                .Add("847DCB64-AFBB-4DB0-8027-4526209DBF94", "Allow to view DTR Dashboard")
                .Add("847DCB64-AFBB-4DB0-8027-4526209DBF95", "Edit Employee List")
                .Add("847DCB64-AFBB-4DB0-8027-4526209DBF96", "Edit Company Profile")
                .Add("847DCB64-AFBB-4DB0-8027-4526209DBF97", "Add, Edit, Delete Authorized Devices")
                .Add("847DCB64-AFBB-4DB0-8027-4526209DBF98", "Security Roles")
                .Add("847DCB64-AFBB-4DB0-8027-4526209DBF99", "Company Places (FOWST)")
                .Add("7073889C-A80A-4D16-8FB4-A20018F115A0", "Allow to change DTR FOWST")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "6A76BBFC-B69C-431A-AC1A-CE095F62CEC6"
            .Name = "Project Management"
            .ProData = vProData
            With .Modules
                .Add("0D4D181C-7D59-460C-8E92-B99607395867", "Access Project Management Module*")
                .Add("8CFDE47A-5D64-43D4-B996-867541AA3AD3", "Access Project Management Settings*")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "611E251E-0AB0-4A8C-94F5-1355B33A74C5"
            .Name = "CRM"
            .ProData = vProData
            With .Modules
                .Add("F4DE9853-304D-4529-81ED-5419C0584BBB", "Access CRM Module*")
                .Add("F438A4E9-5152-4EE6-A75B-C24B257FFF58", "Access CRM Settings*")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "50e3c106-cefe-4a62-afb0-a8dbb64cb5ae"
            .Name = "Vendor Price Canvassing"
            .ProData = vProData
            With .Modules
                .Add("72D5AB37-05F1-4670-8666-D29EE3B0338C", "Access Vendor Price Canvass  Module*")
                .Add("2704ea36-3e4a-43c8-b705-342f5e8ab36d", "Vendor Price Canvass Settings")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "e1e68f63-e681-4889-9a00-dfc34a5eabf2"
            .Name = "Assets Management"
            .ProData = vProData
            With .Modules
                .Add("448C55A5-B402-4576-A69B-D2A798DC91D1", "Access Assets Management Module*")
                .Add("94859150-3229-4b01-af31-5dbf7ba29aa8", "Assets Management Settings")
                .Add("53314e26-e73b-4b8e-add1-8e79b93066a4", "Edit checked-in/ used asset details")
                .Add("1aaca3e8-0623-4ad2-9bbb-5f83d99135d3", "Allow removal of guest(s)/ lessee(s) assigned to an asset", "At least one guest(s)/ lessee(s) should remain assigned to a rented/leased out asset before checkout.")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "82fff94a-1e11-4e08-a73e-4b29c09ba948"
            .Name = "Expense Tracker"
            .ProData = vProData
            With .Modules
                .Add("5e29449d-f3ec-473b-8c22-303097985a2a", "Access to Expense Tracker Module")
                .Add("5ac8305b-4863-4009-b556-c164a4088414", "Access to Expense Tracker Settings")
                .Add("4d40cd9f-662a-4bbf-968a-581afed62b69", "Allow to edit expenses made by other people")
                .Add("e2ef1910-042d-4758-8ad4-69b90c9f72ec", "Allow to add/edit/delete Employee's Route Schedule")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "511afa68-dae9-4657-8dd2-3f355449285b"
            .Name = "Gate Pass"
            .ProData = vProData
            With .Modules
                .Add("32d8c476-cc77-41b8-b77a-eddf71ef79c5", "Access to Gate Pass Settings")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "B30C9241-FF3E-4D8E-9484-A229C85BBADF"
            .Name = "Fleet Tracking and Management"
            .ProData = vProData
            With .Modules
                .Add("A3858BB8-0DD1-4FB0-BEFE-1730862629E0", "Access Fleet Tracking and Management Module*")
                .Add("7C9BA7B0-1951-49E9-84DC-B8FDD104734B", "Access Fleet Tracking and Management Settings*")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "559D79E4-EDD1-41A7-84F2-E21E2F5EC188"
            .Name = "Financial Statements"
            .ProData = vProData
            With .Modules
                .Add("F641C62A-D28C-4146-B93B-FC09E4B341B4", "Access Financial Statement Module*")
                .Add("CEC9DF4D-E74D-4965-9CDA-4A0A5529C46A", "BIR - Sales")
                .Add("AE5A5410-F82E-4E84-ADF6-FBBAE8E7F6DE", "BIR - Purchases")
                .Add("8300BAA2-A597-4822-85D8-E720A9A9B1FA", "Access Financial Statement Settings*")
                .Add("8B86AD2F-DFA0-4BCA-B75B-5E413376B26C", "E-Sales Report")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "8d112700-24b7-4f07-a961-274aee4155ff"
            .Name = "Compliance Reminders"
            .ProData = vProData
            With .Modules
                .Add("d793c73c-a4b1-4122-ba90-1b21c4436f50", "Access Compliance Reminders Module*")
                .Add("5df9d10c-b1ff-489d-9fa2-dbbf505b3f5b", "Add/Edit Compliance Reminders")
            End With
            .Commit()
        End With

        pSecurityModuleMain = New SecurityModuleMain
        With pSecurityModuleMain
            .ID = "BCB686E3-185F-45CE-A3CF-FE787A86D532"
            .Name = "Settings"
            .ProData = vProData
            With .Modules
                .Add("D6C25DA8-14A1-43A5-81E8-F26F5AAC3582", "System Settings").SortOrder = 1
                .Add("26D56D3C-F3B2-4A64-B4A2-A872227EBF3A", "Merge Duplicate Customers")
                .Add("40C6BB92-D49F-4048-8B53-06DDF6F59002", "Merge Duplicate Suppliers")
                .Add("8CE95DDC-90E3-4C9B-B75F-F7836A249072", "Merge Duplicate Brands")
                .Add("573685E7-59CF-4638-9C7B-F77158AED077", "Delete Column Mappings")
                .Add("7F650669-F263-4D8F-9D0E-795DF9BB062E", "Zero out / clear transaction history for this Device+FOWST location", "You may only clear out transaction history once for this Device+FOWST. Clearing out transaction history for this Device+FOWST means that we will retag all old/test transactions with a different Device+FOWST ID. This is to prevent accidental permamnent deletion to data and preserve suitability and data integrity.")
                .Add("bf7bd33f-b33a-4521-abe5-156bb7b34968", "Allow approval of contact person's request to access Vendor Portal?")
                .Add("d41ee1fd-746c-481e-a603-bb7a37e5ebe1", "Allow approval of contact person's request to access Online Store?")
                .Add("340331f8-3de8-440b-b74a-e3da589bcc3e", "Allow to reset employee Mobiz's account")
                .Add("d45cc339-1be7-48dd-8630-8fc22beedf51", "Grant access to edit an employee's profile")
                .Add("be6d8d9f-796a-4cc2-b123-08372945954d", "Allow to sync data from franchisor database")
                .Add("2f7fd44c-f746-4b4b-b0fc-cc48d6233851", "Allow users to add, edit, or remove announcements")
                .Add("c4080625-8299-4cdd-918d-58cf5e94fae5", "Brands Library")
                .Add("838f8ca8-fe81-479d-b0f5-1609e4d2b83a", "Product Categories")
                .Add("a6abb918-5bf5-4fe0-a759-dfcc6b31b89e", "Printers")
                .Add("6b7abfe8-e414-4868-81cb-3a25746f890f", "General Settings")
                .Add("f18b7f81-c26f-4063-89f5-25bc94f095f3", "Decimal in Quantity Settings")
                .Add("e6e92bae-750a-4f81-8a5b-208cd391106e", "POS Settings")
                .Add("9b77cdf0-3a88-4af6-ae2c-7b138519c145", "Sales Order Settings")
                .Add("999bce14-98d0-439e-bf07-416631ebde10", "Receiving and Transfer-Out Settings")
                .Add("4d1450dc-cd11-4e57-bbc8-8cf7cb73cc27", "Inventory Count Settings")
                .Add("53173f79-3111-4596-b374-9bc8d0ba7f43", "Purchase Order Settings")
                .Add("48e6dc29-c23f-49d3-bd39-98647bd0a7c6", "Daily Beginning Balance Settings")
                .Add("fc05fcb9-7162-4f3f-a8b2-f10eb2e5b750", "Product Maker Settings")
                .Add("caf54f02-65e7-4aef-9909-0baf8fd5da8d", "Print Settings")
                .Add("76d66f51-f563-44e6-afac-4cd0a547e6c9", "GPS Settings")
                .Add("a37a3d50-3c16-45ec-80ea-39034b05d38a", "Regional Group Settings")
                .Add("3f5d27bb-13d6-466b-91a2-cbbe8a3e5dca", "Sales Team")
                .Add("c77bd7b7-af00-4da1-942f-ffa316d6f38c", "Item Modifiers")
                .Add("064d9a1d-49cb-4462-a101-4a4e1a12593c", "Payment Terms")
                .Add("ebb0ce78-76a3-44a4-9259-0c2f66ff0989", "Account Adjustments")
                .Add("489084bd-1296-40bb-bd9c-33a014c2eec9", "Price Levels")
                .Add("e83ffe62-24e3-4c7c-869c-311c1f36de3e", "Sales Representatives")
                .Add("6b86f518-a0c8-4270-84a8-35a430134e2f", "Proprietary Barcode Types")
                .Add("998a0053-2196-4a04-8010-cac444f99656", "Vouchers")
                .Add("40001f04-013d-4ae6-9aab-a7c509d1c5dc", "Revenue Centers")
                .Add("905aef48-afbc-4bfb-8dcd-631766816c54", "Cost Centers")
                .Add("372a204d-f20c-406b-9a7e-c15a57097ead", "Chart of Accounts")
                .Add("7e857eda-0576-4292-83ac-eb1d53ea29c8", "Flushing Report")
                .Add("e6d1983f-e19d-4374-adb0-b6c3dd911830", "Auto Data Archiving")
            End With
            .Commit()
        End With

        Dim listpermission As New List(Of Permission)()

        listpermission.Add(New Permission() With { _
            .ID = "71d865e5-77eb-41f2-ab97-e4abe94cb1de",
            .Description = "Add/Edit Labor Costing"
        })

        listpermission.Add(New Permission() With { _
            .ID = "352da83a-7667-44a9-8188-83900966ddcd",
            .Description = "Add/Edit Materials Costing"
        })

        listpermission.Add(New Permission() With { _
            .ID = "2049dbac-dee3-4f69-94dc-45b944e1d0f8",
            .Description = "Add/Edit Timeline"
        })

        listpermission.Add(New Permission() With { _
            .ID = "68ef326f-9ec7-41dc-aed5-3b3290eb586c",
            .Description = "Approve Cash Advances/Disbursements"
        })

        listpermission.Add(New Permission() With { _
            .ID = "8f656260-8b95-4427-87d3-376604d63fc8",
            .Description = "Approve Materials Requisition"
        })

        listpermission.Add(New Permission() With { _
            .ID = "670cc1f0-1b67-431f-93de-e3d027a87702",
            .Description = "Edit Line Items In Project"
        })

        listpermission.Add(New Permission() With { _
            .ID = "bb19590a-2899-48e5-b770-37c6767b0b56",
            .Description = "Make Cash Advances/Disbursements"
        })

        listpermission.Add(New Permission() With { _
            .ID = "7149b0ab-de2c-47c9-a143-3ad8bc142092",
            .Description = "Make Materials Requisition"
        })

        listpermission.Add(New Permission() With { _
            .ID = "0a2d199c-ae86-4def-852d-3ff9abc6ce2d",
            .Description = "Make Purchase Orders"
        })

        listpermission.Add(New Permission() With { _
            .ID = "26a24bf4-f2a6-4a14-ac9d-61485f37eb70",
            .Description = "Make Stock Transfers"
        })

        listpermission.Add(New Permission() With { _
            .ID = "2853ff55-e4e3-4456-a7dd-1032b39b6c26",
            .Description = "Print Project Budget"
        })

        listpermission.Add(New Permission() With { _
            .ID = "7d8bb10a-e136-45ce-8dc1-434a8680d908",
            .Description = "Add/Edit Remarks"
        })

        listpermission.Add(New Permission() With { _
            .ID = "8315f14f-e104-4c88-973a-46201e95c6cd",
            .Description = "Set/Change Task Dependency"
        })

        listpermission.Add(New Permission() With { _
            .ID = "253d12f5-e6f0-4a0a-bf70-a5b266cd0902",
            .Description = "Set/Change Task Duration"
        })

        listpermission.Add(New Permission() With { _
            .ID = "710704d9-c6fd-4767-8254-c80528ee6954",
            .Description = "Set/Change Task Date Started/Completed"
        })

        listpermission.Add(New Permission() With { _
            .ID = "fdc6c1ef-96b0-4594-834b-10164f08afa8",
            .Description = "Set/Change Task Importance"
        })

        listpermission.Add(New Permission() With { _
            .ID = "bd1a95cb-6f34-4825-b87f-3c6f1871f14c",
            .Description = "Set/Change Task Urgency"
        })

        listpermission.Add(New Permission() With { _
            .ID = "7d4197b7-9c23-43df-a639-ac2f95ac8123",
            .Description = "Set/Change Material Unit Of Measure"
        })

        listpermission.Add(New Permission() With { _
            .ID = "cc114170-ea4a-4160-a24c-c1e3f7e21a8f",
            .Description = "Add/Edit Material Budgeted Qty"
        })

        listpermission.Add(New Permission() With { _
            .ID = "fee121dc-08a9-46d7-ae4c-ea0c609959ec",
            .Description = "Add/Edit Material Requested Qty"
        })

        listpermission.Add(New Permission() With { _
            .ID = "62c7a348-c725-4e43-b879-b2a405405c05",
            .Description = "Add/Edit Material Used Up Qty"
        })

        listpermission.Add(New Permission() With { _
            .ID = "4f8f7d6d-d970-4437-bcf8-38549be5edb3",
            .Description = "Add/Edit Labor Payment"
        })

        listpermission.Add(New Permission() With { _
            .ID = "58942c8a-2fa1-4c90-81e0-a2b86bd1234e",
            .Description = "Allow to enter/change date and time"
        })

        For Each lst As Object In listpermission
            pProjectPermission = New ProjectPermission
            With pProjectPermission
                .ID = lst.ID
                .Description = lst.Description
                .ProData = vProData
                .Commit()
            End With
        Next lst

    End Function

    Private Class SecurityModuleMain
        Public ID As String
        Public Name As String
        Public SortBefore As String
        Public ProData As ProData

        Dim mModules As SecurityModules

        Public Sub New()

            mModules = New SecurityModules(Me)

        End Sub

        Public ReadOnly Property Modules As SecurityModules
            Get

                Return mModules

            End Get
        End Property

        Public Sub Commit()

            Dim pIU As InsertUpdate
            Dim pDR As DataRow
            Dim pSql As String
            Dim pSortOrder As Integer

            ProData.BeginTransaction()
            Try
                pSql = "SELECT MainModuleName" &
                " FROM LibrarySecurityModuleMain" & vbCrLf &
                " WHERE ID = " & ReadyString(ID)
                pDR = ProData.ExecuteRow(pSql)
                pIU = New InsertUpdate("LibrarySecurityModuleMain")
                pIU("MainModuleName") = ReadyString(Name)
                If pDR Is Nothing Then
                    pIU("ID") = ReadyString(ID)
                    pIU("SortOrder") = GetMaxSortOrder() + 1
                    pSql = pIU.GetInsert()
                Else
                    pSql = pIU.GetUpdate("ID = " & ReadyString(ID))
                End If
                pIU("LastUpdated") = "GETDATE()"
                ProData.ExecuteSQL(pSql)

                For Each pModule As SecurityModule In mModules
                    pSortOrder += 1
                    pModule.SortOrder = pSortOrder
                    pModule.Commit()
                Next pModule
#If DEBUG Then
                'ProData.RollbackTransaction()
                ProData.CommitTransaction()
#Else
            ProData.CommitTransaction()
#End If
            Catch ex As Exception
                ProData.RollbackTransaction(True, True)
                Throw
            End Try

        End Sub

        Private Function GetMaxSortOrder() As Integer

            Dim pSql As String

            pSql = "SELECT MAX(SortOrder)" & vbCrLf &
                " FROM LibrarySecurityModuleMain"
            Return CInt2(ProData.ExecuteScalar(pSql))

        End Function
    End Class

    Private Class SecurityModules
        Inherits List(Of SecurityModule)

        Dim mParent As SecurityModuleMain

        Public Sub New(vParent As SecurityModuleMain)

            mParent = vParent

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="vID"></param>
        ''' <param name="vName"></param>
        ''' <param name="vDescription"></param>
        ''' <param name="SortOrder"></param>
        ''' <param name="vLabel">if provided, user will see this instead of vName</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Add(vID As String, vName As String, Optional vDescription As String = "", Optional SortOrder As Integer = 0, Optional vLabel As String = Nothing) As SecurityModule

            Dim p As SecurityModule

            p = New SecurityModule
            p.ID = vID
            p.Name = vName
            p.Label = vLabel
            p.Description = vDescription
            p.Parent = mParent
            p.SortOrder = SortOrder

            MyBase.Add(p)

            Return p

        End Function
    End Class

    Private Class SecurityModule
        Public ID As String
        Public Name As String
        Public Label As String
        Public Description As String
        Public IsHeader As Boolean
        Public SortOrder As Integer
        Public Parent As SecurityModuleMain
        Public Header As SecurityModule
        Public DeleteMode As Boolean

        Public Sub Commit()

            Dim pDR As DataRow
            Dim pSql As String
            Dim pIU As InsertUpdate

            If DeleteMode Then
                pSql = "DELETE FROM LibrarySecurityModule" & vbCrLf &
                    " WHERE ModuleName = " & ReadyString(Name) & vbCrLf &
                    " AND MainModuleID = " & ReadyString(Parent.ID)
            Else
                If String.IsNullOrWhiteSpace(ID) Then
                    pSql = "SELECT ID, ModuleName" & vbCrLf &
                        " FROM LibrarySecurityModule" &
                        " WHERE ModuleName = " & ReadyString(Name)
                    pDR = Parent.ProData.ExecuteRow(pSql)
                    If pDR Is Nothing Then
                        ID = Guid.NewGuid().ToString()
                    Else
                        ID = CStr2(pDR("ID"))
                    End If
                Else
                    pSql = "SELECT ModuleName" & vbCrLf &
                        " FROM LibrarySecurityModule" &
                        " WHERE ID = " & ReadyString(ID)
                    pDR = Parent.ProData.ExecuteRow(pSql)
                End If
                pIU = New InsertUpdate("LibrarySecurityModule")
                pIU("LastUpdated") = "GETDATE()"
                pIU("SortOrder") = SortOrder
                pIU("ModuleName") = ReadyString(Name)
                If String.IsNullOrWhiteSpace(Label) Then
                    pIU("Label") = ReadyString(Name)
                Else
                    pIU("Label") = ReadyString(Label)
                End If
                pIU("ModuleDescription") = ReadyString(Description)
                pIU("MainModuleID") = ReadyString(Parent.ID)
                If Header IsNot Nothing Then
                    pIU("HeaderID") = ReadyString(Header.ID)
                End If
                If pDR Is Nothing Then
                    pIU("ID") = ReadyString(ID)
                    pIU("IsHeader") = If(IsHeader, 1, 0)
                    pSql = pIU.GetInsert()
                Else
                    pSql = pIU.GetUpdate("ID = " & ReadyString(ID))

                End If
            End If

            Parent.ProData.ExecuteSQL(pSql)

        End Sub
    End Class

    Function SetDefaultAccessToAllSecRole(ByVal defaultvalue As Integer, ByVal moduleid As String, Optional vProData As ProData = Nothing)
        Dim pProData As ProData
        Dim pSql As String

        pProData = vProData
        If vProData Is Nothing Then
            pProData = Common.WebRequest.ProData
        End If

        Dim pDR As Object = pProData.Execute("SELECT * FROM LibrarySecurityRole").Rows

        For Each role As Object In pDR
            If pProData.Execute("SELECT ID FROM LibrarySecurityRoleModules WHERE RoleID = " & ReadyString(role("ID").ToString()) &
                    " AND ModuleID = " & ReadyString(moduleid)).Rows.Count = 0 Then

                Dim p As New InsertUpdate("LibrarySecurityRoleModules")

                p("ID") = ReadyString(Guid.NewGuid.ToString())
                p("RoleID") = ReadyString(role("ID").ToString())
                p("ModuleID") = ReadyString(moduleid)
                p("IsHasAccess") = defaultvalue
                p("LastUpdated") = "getdate()"

                pProData.Execute(p.GetInsert())
            End If
        Next
    End Function


    ''' <summary>
    ''' requires private database
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GrantAllToSysAdmin(Optional vProData As ProData = Nothing) As Integer

        Dim pProData As ProData
        Dim pSql As String
        Dim pDenied As New List(Of String)
        Dim pIDs As String = ""
        Dim pCount As Integer

        pProData = vProData
        If vProData Is Nothing Then
            pProData = Common.WebRequest.ProData
        End If
        AddSecurityModules(pProData)

        pDenied.Add("49557584-41E9-4BB9-B4AB-D7849AEDEFE8") ' "Ask for Approval"
        pDenied.Add("5fbbc110-8ba8-419d-8069-66401853cb4d")
        pDenied.Add("b53fe93c-7b2d-42a1-8ec7-eff71de8d83a")
        pDenied.Add("a41ed516-04b1-4a66-a7f4-222943741434")
        pDenied.Add("f3f5078e-e85f-4830-b9fb-d931337398d0")
        pDenied.Add("11b381e3-f914-4b50-bacd-c953f6a2a1d6")
        pDenied.Add("1b846b0f-7464-446c-94e3-4a3990975c15")
        pDenied.Add("cec2dad9-a1f1-4a5f-a4c0-2964dd5e64e3")
        pDenied.Add("8709e903-89ab-4d84-90a8-0168e6b194b5")
        pDenied.Add("d28f7c56-e6d7-478f-bf83-05273d84536c")
        pDenied.Add("ccea36e3-7e56-4688-b3df-7d3a973158dd")
        pDenied.Add("e663929c-8d6a-4da4-aa0d-1fb6ae8b912c")
        pDenied.Add("f826ccdc-52ab-4578-9845-d65fcfe17774")
        pDenied.Add("80421c8d-0122-4372-8d96-2405b9ea434a")
        pDenied.Add("966ca410-c176-42cf-95a1-42cd15ace195")
        pDenied.Add("e9f0dfc2-6f2c-4346-91e8-627d45e282e9")
        pDenied.Add("6b86eaec-d7bc-45e9-a8b6-d04bdb2a0d6e")
        pDenied.Add("370da412-f423-435f-981e-96d055c33fca")
        pDenied.Add("a2b31bca-4eb4-4ae8-93ad-3f63feae4337")
        pDenied.Add("714eb1bf-d544-4477-ab9d-b7a9f01f576b")
        pDenied.Add("96a90071-15dd-4f5f-afe8-928d45aae9ff")
        pDenied.Add("e962cae4-be2a-4544-bb12-a9ec322ab5bc")
        pDenied.Add("d436c19e-5c32-402c-a73b-aa56eef704c3")
        pDenied.Add("88cc6898-95c6-4bc4-9651-369944186d31")
        pDenied.Add("add7233f-bceb-42bc-95f3-d7b0228b06ba")
        pDenied.Add("23d6c2ce-cf26-46b9-a47e-08c12f5c158c")
        pDenied.Add("add0cc4a-b7db-45e9-a752-b36966ee7fc2")
        pDenied.Add("61561882-e5b7-4af8-90b7-c200a8ac8aaf")
        pDenied.Add("63612dc1-5428-4c28-85f0-87df5da3c027")
        pDenied.Add("996ebee3-54b9-4144-879b-05cd7f303dbb")
        pDenied.Add("3e312330-b886-422a-8e59-38327a0129b6")
        pDenied.Add("20dc9711-5cd0-480b-bf7a-aac6ca62ff48")
        pDenied.Add("e39ec10a-a10a-4638-9763-5f0063f2d900")

        For Each pStr As String In pDenied
            If pIDs.Length > 0 Then pIDs &= ", "
            pIDs &= ReadyString(pStr)
        Next pStr

        ' add "allowed" modules to role modules that does not exist in role modules
        pSql = " INSERT INTO LibrarySecurityRoleModules" & vbCrLf &
            " (ID, RoleID, ModuleID, IsHasAccess)" & vbCrLf &
            " SELECT NEWID()" & vbCrLf &
            " ," & ReadyString(SysAdminRoleID) & vbCrLf &
            " ,LibrarySecurityModule.ID" & vbCrLf &
            " ,1" & vbCrLf &
            " FROM LibrarySecurityModule" & vbCrLf &
            " WHERE NOT EXISTS (" & vbCrLf &
            " 	SELECT *" & vbCrLf &
            "     FROM LibrarySecurityRoleModules" & vbCrLf &
            "     WHERE LibrarySecurityRoleModules.ModuleID = LibrarySecurityModule.ID" & vbCrLf &
            "     AND LibrarySecurityRoleModules.RoleID = " & ReadyString(SysAdminRoleID) & vbCrLf &
            " )" & vbCrLf &
            " AND LibrarySecurityModule.ID NOT IN (" & pIDs & ")"
        pCount += pProData.ExecuteSQL(pSql)

        ' this is not needed
        '' add "denied" modules to role modules that does not exist in role modules
        'pSql = " INSERT INTO LibrarySecurityRoleModules" & vbCrLf &
        '    " (ID, RoleID, ModuleID, IsHasAccess)" & vbCrLf &
        '    " SELECT NEWID()" & vbCrLf &
        '    " ," & ReadyString(pRoleID) & vbCrLf &
        '    " ,LibrarySecurityModule.ID" & vbCrLf &
        '    " ,0" & vbCrLf &
        '    " FROM LibrarySecurityModule" & vbCrLf &
        '    " WHERE NOT EXISTS (" & vbCrLf &
        '    " 	SELECT *" & vbCrLf &
        '    "     FROM LibrarySecurityRoleModules" & vbCrLf &
        '    "     WHERE LibrarySecurityRoleModules.ModuleID = LibrarySecurityModule.ID" & vbCrLf &
        '    "     AND LibrarySecurityRoleModules.RoleID = " & ReadyString(pRoleID) & vbCrLf &
        '    " )" & vbCrLf &
        '    " AND LibrarySecurityModule.ID IN (" & pIDs & ")"
        'pProData.ExecuteSQL(pSql)

        ' update modules to "allow"
        pSql = " UPDATE LibrarySecurityRoleModules" & vbCrLf &
            " SET IsHasAccess = 1" & vbCrLf &
            " WHERE LibrarySecurityRoleModules.ModuleID NOT IN (" & pIDs & ")" & vbCrLf &
            " AND LibrarySecurityRoleModules.RoleID = " & ReadyString(SysAdminRoleID) & vbCrLf &
            " AND ISNULL(IsHasAccess, 0) = 0"
        pCount += pProData.ExecuteSQL(pSql)

        ' update modules to "denied"
        pSql = " UPDATE LibrarySecurityRoleModules" & vbCrLf &
            " SET IsHasAccess = 0" & vbCrLf &
            " WHERE LibrarySecurityRoleModules.ModuleID IN (" & pIDs & ")" & vbCrLf &
            " AND LibrarySecurityRoleModules.RoleID = " & ReadyString(SysAdminRoleID) & vbCrLf &
            " AND ISNULL(IsHasAccess, 0) = 1"
        pCount += pProData.ExecuteSQL(pSql)

        Return pCount

    End Function

    Private Class ProjectPermission
        Public ID As String
        Public Description As String
        Public ProData As ProData

        Public Sub Commit()

            Dim pIU As InsertUpdate
            Dim pDR As DataRow
            Dim pSql As String
            Dim pSortOrder As Integer

            ProData.BeginTransaction()
            Try
                pSql = "SELECT Description" &
                " FROM LibraryProjectPermission" & vbCrLf &
                " WHERE ID = " & ReadyString(ID)
                pDR = ProData.ExecuteRow(pSql)
                pIU = New InsertUpdate("LibraryProjectPermission")
                pIU("Description") = ReadyString(Description)
                pIU("LastUpdated") = "GETDATE()"
                If pDR Is Nothing Then
                    pIU("ID") = ReadyString(ID)
                    pSql = pIU.GetInsert()
                Else
                    pSql = pIU.GetUpdate("ID = " & ReadyString(ID))
                End If
                ProData.ExecuteSQL(pSql)
#If DEBUG Then
                'ProData.RollbackTransaction()
                ProData.CommitTransaction()
#Else
            ProData.CommitTransaction()
#End If
            Catch ex As Exception
                ProData.RollbackTransaction(True, True)
                Throw
            End Try
        End Sub
    End Class

    Private Class Permission
        Private DescID As String
        Private Desc As String

        Public Property ID() As String
            Get
                Return DescID
            End Get
            Set(value As String)
                DescID = value
            End Set
        End Property


        Public Property Description() As String
            Get
                Return Desc
            End Get
            Set(value As String)
                Desc = value
            End Set
        End Property
    End Class
End Class
