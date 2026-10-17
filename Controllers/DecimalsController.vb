Namespace WebProj
    Public Class DecimalsController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /Decimals

        Function Index() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If
            Return View()
        End Function

        Function GetDecimalSettings() As String
            Dim pSql As String
            Dim pFinder As Finder
            Dim pDt As DataTable

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pFinder = New Finder

            With pFinder
                .ProData = Common.WebRequest.ProData
                .Distinct = True

                With .Fields
                    With .Add("ID", SqlDbType.UniqueIdentifier)
                        .Searchable = False
                    End With

                    .Add("IsAllowDecimal", SqlDbType.Bit)
                    .Add("AllowDecimalMode", SqlDbType.Int)
                End With

                .SqlFrom = "FROM LibraryOrganizationDecimalSetting "

                pDt = .Requery

                Return JsonDictionary.Serialize("Results", pDt)
            End With

        End Function

        Function GetAllProductFeatures() As String
            Dim pSql As String
            Dim pFinder As Finder
            Dim pDt As DataTable

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pFinder = New Finder

            With pFinder
                .ProData = Common.WebRequest.ProData
                .Distinct = True

                With .Fields
                    'With .Add("ID", SqlDbType.UniqueIdentifier)
                    '    .Searchable = False
                    '    .Expression = "MobizLibraryPIMProductVariableFeatures001.ID"
                    'End With

                    With .Add("Label", SqlDbType.VarChar)
                        .Expression = "ISNULL(CASE WHEN dbo.MobizLibraryPIMProductVariableFeatures001.c0020 = '' THEN 'N/A' ELSE dbo.MobizLibraryPIMProductVariableFeatures001.c0020 END,'N/A')"
                    End With

                    .Add("AllowDecimal", SqlDbType.Bit)
                End With

                .SqlFrom = "FROM dbo.MobizLibraryPIMProductVariableFeatures001 INNER JOIN " & vbCrLf &
                            "LibraryOrganizationPIM on LibraryOrganizationPIM.ProductID = MobizLibraryPIMProductVariableFeatures001.ProductID "

                '.SqlWhere = "WHERE MobizLibraryPIMTemplate.OrganizationID = " & ReadyString(Common.WebRequest.Data("OrgID"))

                .SqlOrder = "Order By Label"

                pDt = .Requery

                Return JsonDictionary.Serialize("Results", pDt)
            End With
        End Function

        Function SaveFeaturesWithDecimal() As String
            Dim p As New InsertUpdate("LibraryOrganizationDecimalSetting")
            Dim pSql As String
            Dim Features() As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try

                Common.WebRequest.ProData.BeginTransaction()

                p("IsAllowDecimal") = CInt(WebRequest.Instance.Data("IsAllowDecimal"))
                p("AllowDecimalMode") = CInt(WebRequest.Instance.Data("DecimalMode"))

                If Trim(WebRequest.Instance.Data("ID").Length) > 0 Then
                    pSql = p.GetUpdate("ID =" & ReadyString(WebRequest.Instance.Data("ID")))
                Else
                    pSql = p.GetInsert()
                End If

                Common.WebRequest.ProData.Execute(pSql)


                If CBool(WebRequest.Instance.Data("IsAllowDecimal")) Then
                    If (CInt(WebRequest.Instance.Data("DecimalMode")) = 1) Then

                        'reset flag from libraryorgpim
                        pSql = "Update LibraryOrganizationPIM set AllowDecimal=0"
                        Common.WebRequest.ProData.Execute(pSql)

                        'clear table
                        pSql = "delete from LibraryOrganizationDecimalFeatures "

                        Common.WebRequest.ProData.Execute(pSql)

                        'replace with new features
                        p = New InsertUpdate("LibraryOrganizationDecimalFeatures")

                        Features = Split(WebRequest.Instance.Data("FeatureID"), "|")

                        Dim i As Integer
                        Dim pSqlWhere As String
                        Dim pColumnName As String

                        While i < UBound(Features)

                            'p("FeatureID") = ReadyString(Features(i))
                            'pSql = p.GetInsert()

                            'Common.WebRequest.ProData.Execute(pSql)

                            'pSql = "Select MobizLibraryPIMFeature.PIMFeatureColumn from MobizLibraryPIMFeature where MobizLibraryPIMFeature.ID = " & ReadyString(Features(i))
                            'pColumnName = CStr(Common.WebRequest.ProData.ExecuteScalar(pSql))

                            'If Trim(pColumnName).Length > 0 Then

                            '    pSqlWhere += " OR (" & pColumnName & " <>'' OR " & pColumnName & " IS NOT NULL) "
                            'End If

                            If i = 0 Then
                                pSqlWhere = " Where (c0020 = " & ReadyString(Features(i)) & ")"
                            Else
                                pSqlWhere += " OR (c0020 = " & ReadyString(Features(i)) & ") "
                            End If

                            i += 1
                        End While

                        If i > 0 Then
                            pSql = "Update LibraryOrganizationPIM set AllowDecimal=1 WHERE LibraryOrganizationPIM.ID IN (select LibraryOrganizationPIM.ID from LibraryOrganizationPIM " & vbCrLf &
                                "inner join MobizLibraryPIMProductVariableFeatures001 on " & vbCrLf &
                                "MobizLibraryPIMProductVariableFeatures001.productid = libraryorganizationpim.ProductID " & vbCrLf &
                                pSqlWhere & " )"

                            Common.WebRequest.ProData.Execute(pSql)
                        End If
                        

                    Else
                        pSql = "Update LibraryOrganizationPIM set AllowDecimal=1"
                        Common.WebRequest.ProData.Execute(pSql)
                    End If

                Else
                    pSql = "Update LibraryOrganizationPIM set AllowDecimal=0"
                    Common.WebRequest.ProData.Execute(pSql)
                End If

                Common.WebRequest.ProData.CommitTransaction()
                Return JsonDictionary.Serialize("Results", "True")

            Catch ex As Exception
                Common.WebRequest.ProData.RollbackTransaction()
                Return JsonDictionary.Serialize("Results", "False")
            End Try

        End Function
    End Class
End Namespace