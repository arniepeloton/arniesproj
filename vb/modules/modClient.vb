Public Module modClient
    Public Const EmployeeWebNumericID As Integer = 554 ' EmployeeProfiles.ID
    Public Const EmployeeWebGuidID As String = "F0F54F3F-DFB8-4C47-BFC6-8C4EA17AFA14"
    Public Const DarlingtonTrailOrgID As String = "f6a0d46d-d238-4484-93bc-2e4e6b4c964c"
    Public Const DucOrgID As String = "9d4b1500-13c0-49f9-a8e5-a16c13ec899e"

    ''' <summary>
    ''' dev server
    ''' </summary>
    ''' <remarks></remarks>
    Public Const AbcOrgID As String = "cfe39df6-8809-4973-9515-810df75aed12"
    Public Const MobizTestDataOrgID As String = "7C794971-1CD2-4AD4-922F-36A93EDDE09D" ' Mobiz Data Test
    Public Const CashOrgID As String = "CD71A0F7-08A9-4601-876E-5F13C064F873"
    Public Const CafeFranceOrgID As String = "8C1C2898-0965-4A27-A4A8-9BCC1915568E"
    Public Const DemoCorpOrgID As String = "9262FC46-DE29-4497-BDF2-8832F5E7576D" ' demo corp. db: MobizHungryApe
    Public Const CincoOrgID As String = "d362ffdb-15d1-451f-97dc-c567b94fb410" ' cinco / potato corner
    Public Const SportsTownOrgID As String = "F2D5C353-DF14-46C8-A613-C6847FFFDA82" ' 
    Public Const PotatoCornerFranchiseOrgID As String = "892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba" ' MobizPotato_Corner_Franchise. Potato Corner Franchise
    Public Const PotatoCornerFranchise2OrgID As String = "6c9d472b-9cd2-4cd6-b485-cfaed87c6011" ' MobizPotato_Corner_Franchise_2. Potato Corner Franchise 2
    ' 9DA344F3-DADB-4675-B723-15F0C591DA3C ' Nike
    ''' <summary>
    ''' southern metal in prod
    ''' </summary>
    ''' <remarks></remarks>
    Public Const DsmeOrgID As String = "61be6909-f4c2-4ab2-8df3-d4841cc2f318"
    Public Const TaxPercent As Decimal = 12
    Public Const SrpID = "EEF716C6-B290-4B28-B2A6-0A4F52CF4989"
    Public Const WholesalePriceID = "01000012-0004-0000-0000-000000000000"
    Public Const RetailPriceID = "01000012-0005-0000-0000-000000000000"

    Public ServerIdentity As String = ""

    Public Sub AddToTempLogs(vText As String)
        If Now.Date > #9/12/2023# Then
            Return
        End If

        AppendToLog(vText,, "InvLedger vbTHN8QPVZP4")

    End Sub
End Module
