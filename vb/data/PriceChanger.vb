Public Class PriceChanger
    Public DestinationOrgID As String
    Public DestinationPriceLevelID As String
    Public SourceOrgID As String
    Public SourcePriceLevelID As String

    Dim mProData As ProData
    Dim mBusiness As Business

    Public Function Verify() As VerifyResult

        Dim pOut As VerifyResult
        Dim pDR As DataRow

        pOut = New VerifyResult
        Try


            mBusiness = New Business(mProData)

            pDR = mBusiness.LibraryOrganization.ByID(DestinationOrgID)
            If pDR Is Nothing Then
                pOut.ErrorMessage = "Destination organization, " & DestinationOrgID & ", does not exist"
                Return pOut
            End If
            pOut.DestinationOrgName = CStr2(pDR("CompanyName"))

            If CStr2(pDR("Database")).Trim() = "" Then
                pOut.ErrorMessage = "Destination organization's, " & DestinationOrgID & ", database field is blank"
                Return pOut
            End If
            pOut.DestinationOrgDbName = pDR("Database")

            pOut.DestinationPricelLevelName = GetPriceLevelName(DestinationPriceLevelID)
            If String.IsNullOrWhiteSpace(pOut.DestinationPricelLevelName) Then
                pOut.ErrorMessage = "Destination price level does not exist or is blank"
                Return pOut
            End If

            ' source
            pDR = mBusiness.LibraryOrganization.ByID(SourceOrgID)
            If pDR Is Nothing Then
                pOut.ErrorMessage = "Source organization, " & SourceOrgID & ", does not exist"
                Return pOut
            End If
            pOut.SourceOrgName = CStr2(pDR("CompanyName"))

            If CStr2(pDR("Database")).Trim() = "" Then
                pOut.ErrorMessage = "Source organization's, " & SourceOrgID & ", database field is blank"
                Return pOut
            End If
            pOut.SourceOrgDbName = pDR("Database")

            pOut.SourcePricelLevelName = GetPriceLevelName(SourcePriceLevelID)
            If String.IsNullOrWhiteSpace(pOut.SourcePricelLevelName) Then
                pOut.ErrorMessage = "Source price level does not exist or is blank"
                Return pOut
            End If
        Catch ex As Exception
            pOut.ErrorMessage = ex.Message
        End Try
        Return pOut

    End Function

    Public Class VerifyResult

        Public DestinationOrgName As String
        Public DestinationOrgDbName As String
        Public DestinationPricelLevelName As String

        Public SourceOrgName As String
        Public SourceOrgDbName As String
        Public SourcePricelLevelName As String

        Public ErrorMessage As String
    End Class

    Private Function GetPriceLevelName(vID As String) As String

        Dim pSql As String
        Dim pDR As DataRow

        pSql = " SELECT Pricelevel" & vbCrLf &
            " FROM MobizLibraryPIMPriceLevel" & vbCrLf &
            " WHERE ID = " & ReadyString(vID)
        pDR = mProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            Return pDR("PriceLevel")
        End If

    End Function

    Public Function Update() As UpdateResult

        Dim pSql As String
        Dim pDT As DataTable
        Dim pVerifyResult As VerifyResult
        Dim pOut As UpdateResult

        pOut = New UpdateResult()
        pVerifyResult = Verify()
        pOut.VerifyResult = pVerifyResult
        If String.IsNullOrWhiteSpace(pVerifyResult.ErrorMessage) = False Then
            Return pOut
        End If

        pSql = " DECLARE @DestinationOrgID UNIQUEIDENTIFIER" & vbCrLf &
            " DECLARE @DestinationPriceLevelID UNIQUEIDENTIFIER" & vbCrLf &
            " DECLARE @SourceOrgID UNIQUEIDENTIFIER" & vbCrLf &
            " DECLARE @SourcePriceLevelID UNIQUEIDENTIFIER" & vbCrLf &
            " SET @DestinationOrgID = " & ReadyString(DestinationOrgID) & vbCrLf &
            " SET @DestinationPriceLevelID = " & ReadyString(DestinationPriceLevelID) & vbCrLf &
            " SET @SourceOrgID = " & ReadyString(SourceOrgID) & vbCrLf &
            " SET @SourcePriceLevelID = " & ReadyString(SourcePriceLevelID) & vbCrLf &
            ";WITH CTE AS (" & vbCrLf &
            " SELECT LibraryOrganizationPIM_Destination.ProductID" & vbCrLf &
            " ,LibraryPIMProduct.ProductNo" & vbCrLf &
            " ,LibraryPIMProduct.ProductName" & vbCrLf &
            " ,(SELECT TOP 1 LibraryPIMProductPriceLevel.Price" & vbCrLf &
            " 	FROM LibraryPIMProductPriceLevel" & vbCrLf &
            "     WHERE LibraryPIMProductPriceLevel.ProductID = LibraryOrganizationPIM_Destination.ProductID" & vbCrLf &
            "     AND LibraryPIMProductPriceLevel.OrganizationID = @DestinationOrgID" & vbCrLf &
            "     AND LibraryPIMProductPriceLevel.PriceLevelID = @DestinationPriceLevelID" & vbCrLf &
            "     ORDER BY LibraryPIMProductPriceLevel.LastUpdated DESC" & vbCrLf &
            " ) AS DestinationPrice" & vbCrLf &
            " ,(SELECT TOP 1 LibraryPIMProductPriceLevel.Price" & vbCrLf &
            " 	FROM LibraryPIMProductPriceLevel" & vbCrLf &
            "     WHERE LibraryPIMProductPriceLevel.ProductID = LibraryOrganizationPIM_Destination.ProductID" & vbCrLf &
            "     AND LibraryPIMProductPriceLevel.OrganizationID = @SourceOrgID" & vbCrLf &
            "     AND LibraryPIMProductPriceLevel.PriceLevelID = @SourcePriceLevelID" & vbCrLf &
            "     ORDER BY LibraryPIMProductPriceLevel.LastUpdated DESC" & vbCrLf &
            " ) AS SourcePrice" & vbCrLf &
            " FROM " & EscapeSqlName(pVerifyResult.DestinationOrgDbName) & ".dbo.LibraryOrganizationPIM LibraryOrganizationPIM_Destination" & vbCrLf &
            " LEFT JOIN LibraryPIMProduct ON LibraryPIMProduct.ID = LibraryOrganizationPIM_Destination.ProductID" & vbCrLf &
            " WHERE EXISTS (" & vbCrLf &
            " 	SELECT *" & vbCrLf &
            "     FROM " & EscapeSqlName(pVerifyResult.SourceOrgDbName) & ".dbo.LibraryOrganizationPIM LibraryOrganizationPIM_Source" & vbCrLf &
            "     JOIN LibraryPIMProductPriceLevel ON LibraryPIMProductPriceLevel.ProductID = LibraryOrganizationPIM_Source.ProductID" & vbCrLf &
            "     AND LibraryPIMProductPriceLevel.OrganizationID = @SourceOrgID" & vbCrLf &
            "     AND LibraryPIMProductPriceLevel.PriceLevelID = @SourcePriceLevelID" & vbCrLf &
            "     AND LibraryPIMProductPriceLevel.Price IS NOT NULL" & vbCrLf &
            "     WHERE LibraryOrganizationPIM_Source.ProductID = LibraryOrganizationPIM_Destination.ProductID" & vbCrLf &
            " )" & vbCrLf &
            ") -- END CTE" & vbCrLf

        pSql &= " SELECT *" & vbCrLf &
            " FROM CTE" & vbCrLf &
            " WHERE DestinationPrice <> SourcePrice" & vbCrLf &
            " OR DestinationPrice IS NULL"
        pDT = mProData.Execute(pSql)

        Dim pIU As InsertUpdate

        For Each pDR As DataRow In pDT.Rows
            pIU = New InsertUpdate("LibraryPIMProductPriceLevel")
            pIU("Price") = pDR("SourcePrice")
            pIU("LastUpdated") = "GETDATE()"
            pSql = pIU.GetUpdate("ProductID = " & ReadyString(pDR("ProductID")) &
                " AND OrganizationID = " & ReadyString(DestinationOrgID) &
                " AND PriceLevelID = " & ReadyString(DestinationPriceLevelID))
            If mProData.ExecuteSQL(pSql) = 0 Then
                pIU("OrganizationID") = ReadyString(DestinationOrgID)
                pIU("ProductID") = ReadyString(pDR("ProductID"))
                pIU("PriceLevelID") = ReadyString(DestinationPriceLevelID)
                pIU("UserID") = ReadyString(modClient.EmployeeWebGuidID)
                pIU("DeactivateDiscount") = 0
                pSql = pIU.GetInsert()
                mProData.ExecuteSQL(pSql)
                pOut.UpdatedCount += 1
            Else
                pOut.InsertedCount += 1
            End If
        Next pDR

        Return pOut

    End Function

    Public Class UpdateResult
        Public VerifyResult As VerifyResult
        Public UpdatedCount As Integer
        Public InsertedCount As Integer
    End Class

    Public Property ProData As ProData
        Get

            Return mProData

        End Get
        Set(value As ProData)

            mProData = value

        End Set
    End Property
End Class
