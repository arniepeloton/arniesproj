Public Class LibraryPIMFeature

    Dim mProData As ProData
    Dim mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Sub AddRequiredFeatures()

        GetOrAddFeatureByLabel("Product Vendor", "4F2CF1F2-FB38-4F54-9244-EF3D4878D3E2")
        GetOrAddFeatureByLabel("Rentable/PPE Asset?", "426b72c3-b0cd-4c6e-b762-85c8f556e921")
        'GetOrAddFeatureByLabel("Product No", "F9CB775D-5E60-4D92-98A3-2A8444D5F32A")
        GetOrAddFeatureByLabel("Cost From Vendor", "E531191E-203E-41CC-BA9B-C46A27D10CD8")
        GetOrAddFeatureByLabel("Shipping Cost", "2254751E-E1FC-4704-B494-A39FDBA2A245")
        GetOrAddFeatureByLabel("Landed Cost", "1EB487FC-8B91-4D0C-901F-16BF4B80A9A3")
        GetOrAddFeatureByLabel("Retail Price", "C5BD7B11-369D-404A-AD18-C53566744C90")
        GetOrAddFeatureByLabel("Upcoming Retail Price", "3ec54904-f1b9-4be4-8ecb-6a28355f2d18", "Textbox (Decimal)")
        GetOrAddFeatureByLabel("Upcoming Retail Price Effectivity Date", "6de1c205-8d7d-499c-9a28-a1db0a7e2762", "Date Only")
        GetOrAddFeatureByLabel("Fries", "6de1c205-8d7d-499c-9a28-a1db0a7e2763")
        GetOrAddFeatureByLabel("Flavor", "6de1c205-8d7d-499c-9a28-a1db0a7e2764")
        GetOrAddFeatureByLabel("Delivery Method", "6de1c205-8d7d-499c-9a28-a1db0a7e2765")

    End Sub

    Public Function GetOrAddFeatureByLabel(vLabel As String, vNewFeatureID As String, Optional ByVal featuretype As String = "Textbox") As String

        Dim pOut = GetFeatureByLabel(vLabel)
        If String.IsNullOrWhiteSpace(pOut) Then
            pOut = vNewFeatureID
            Dim pIU = New InsertUpdate("MobizLibraryPIMFeature")
            pIU("ID") = ReadyString(pOut)
            Dim pFeatureLabelID = GetOrAddCountriesLanguageLabel(vLabel)
            pIU("FeatureLabelID") = ReadyString(pFeatureLabelID)
            pIU("FeatureNameID") = ReadyString(pFeatureLabelID)
            pIU("FeatureType") = ReadyString(featuretype)
            pIU("IsActive") = "1"
            pIU("UserID") = ReadyString(modClient.EmployeeWebGuidID)
            pIU("LastUpdated") = "getdate()"
            Dim pSql = pIU.GetInsert()
            mProData.ExecuteSQL(pSql)
        End If

        Return pOut

    End Function

    Private Function GetFeatureByLabel(vLabel As String) As String

        Dim pSql = "SELECT MobizLibraryPIMFeature.ID" & vbCrLf &
            " FROM MobizLibraryPIMFeature" & vbCrLf &
            " JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMFeature.FeatureLabelID" & vbCrLf &
            " WHERE MobizLibraryCountriesLanguageLabel.Label = " & ReadyString(vLabel)
        Dim pOut = CStr2(mProData.ExecuteScalar(pSql))

        Return pOut

    End Function

    Private Function GetOrAddCountriesLanguageLabel(vLabel As String) As String

        Dim pOut = GetCountriesLanguageLabel(vLabel)
        If String.IsNullOrWhiteSpace(pOut) Then
            pOut = AddCountriesLanguageLabel(vLabel)
        End If

        Return pOut

    End Function

    Private Function GetCountriesLanguageLabel(vLabel As String) As String

        Dim pSql = "SELECT ID" & vbCrLf &
          " FROM MobizLibraryCountriesLanguageLabel" & vbCrLf &
          " WHERE Label = " & ReadyString(vLabel)
        Dim pOut = CStr2(mProData.ExecuteScalar(pSql))

        Return pOut

    End Function

    Private Function AddCountriesLanguageLabel(vLabel As String) As String

        Dim pOut = Guid.NewGuid().ToString()
        Dim pIU = New InsertUpdate("MobizLibraryCountriesLanguageLabel")
        pIU("ID") = ReadyString(pOut)
        pIU("Label") = ReadyString(vLabel)
        Dim pSql = pIU.GetInsert()
        mProData.ExecuteSQL(pSql)

        Return pOut

    End Function
End Class